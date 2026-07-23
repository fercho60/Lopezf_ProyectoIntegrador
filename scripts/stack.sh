#!/usr/bin/env bash
set -Eeuo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck source=stack-common.sh
source "${SCRIPT_DIR}/stack-common.sh"

load_config
ensure_layout
validate_config

LOCK_DIR="${STACK_DIR}/start.lock"
mkdir "${LOCK_DIR}" 2>/dev/null ||
    die "Ya hay otro make stack en ejecución (${LOCK_DIR})."
trap 'rmdir "${LOCK_DIR}" 2>/dev/null || true' EXIT

require_command brew
require_command curl
require_command dotnet
require_command lsof
require_command tar

PG_PREFIX="$(brew_prefix postgresql@16)"
MYSQL_PREFIX="$(brew_prefix mysql)"
MAVEN_PREFIX="$(brew_prefix maven)"
JDK_PREFIX="$(brew_prefix openjdk@21)"

PG_BIN="${PG_PREFIX}/bin"
MYSQL_BIN="${MYSQL_PREFIX}/bin"
MAVEN_BIN="${MAVEN_PREFIX}/bin"
JAVA_HOME="${JDK_PREFIX}/libexec/openjdk.jdk/Contents/Home"
export JAVA_HOME
export PATH="${JAVA_HOME}/bin:${MAVEN_BIN}:${PG_BIN}:${MYSQL_BIN}:${PATH}"

start_postgresql() {
    local pid_file="${RUN_DIR}/postgresql.pid"
    local password_file="${RUN_DIR}/postgres-password"

    if [[ ! -s "${PG_DATA}/PG_VERSION" ]]; then
        [[ ! -e "${PG_DATA}" || -z "$(ls -A "${PG_DATA}" 2>/dev/null)" ]] ||
            die "${PG_DATA} existe pero no es un clúster PostgreSQL válido."
        log "Inicializando PostgreSQL 16 en ${PG_DATA}"
        mkdir -p "${PG_DATA}"
        umask 077
        printf '%s\n' "${PG_PASSWORD}" >"${password_file}"
        "${PG_BIN}/initdb" --pgdata="${PG_DATA}" --username="${PG_USER}" \
            --pwfile="${password_file}" --auth-local=trust --auth-host=scram-sha-256 \
            --encoding=UTF8 --no-locale >/dev/null
        rm -f "${password_file}"
    fi

    if "${PG_BIN}/pg_ctl" status --pgdata="${PG_DATA}" >/dev/null 2>&1; then
        log "PostgreSQL ya está iniciado."
    else
        assert_port_free PostgreSQL "${PG_PORT}"
        log "Iniciando PostgreSQL en 127.0.0.1:${PG_PORT}"
        "${PG_BIN}/pg_ctl" start --pgdata="${PG_DATA}" \
            --log="${LOG_DIR}/postgresql.log" \
            --options="-p ${PG_PORT} -h 127.0.0.1" --wait >/dev/null
    fi

    wait_until PostgreSQL 60 "${PG_BIN}/pg_isready" \
        --host=127.0.0.1 --port="${PG_PORT}" --username="${PG_USER}"
    awk 'NR == 1 { print; exit }' "${PG_DATA}/postmaster.pid" >"${pid_file}"

    if ! PGPASSWORD="${PG_PASSWORD}" "${PG_BIN}/psql" \
        --host=127.0.0.1 --port="${PG_PORT}" --username="${PG_USER}" \
        --dbname=postgres --tuples-only --no-align \
        --command="SELECT 1 FROM pg_database WHERE datname = '${PG_DATABASE}'" |
        grep -qx 1; then
        log "Creando base PostgreSQL ${PG_DATABASE}"
        PGPASSWORD="${PG_PASSWORD}" "${PG_BIN}/createdb" \
            --host=127.0.0.1 --port="${PG_PORT}" --username="${PG_USER}" \
            "${PG_DATABASE}"
    fi
}

start_mysql() {
    local pid_file="${RUN_DIR}/mysql.pid"

    if [[ ! -d "${MYSQL_DATA}/mysql" ]]; then
        [[ ! -e "${MYSQL_DATA}" || -z "$(ls -A "${MYSQL_DATA}" 2>/dev/null)" ]] ||
            die "${MYSQL_DATA} existe pero no es un data dir MySQL válido."
        log "Inicializando MySQL en ${MYSQL_DATA}"
        mkdir -p "${MYSQL_DATA}"
        "${MYSQL_BIN}/mysqld" --no-defaults --initialize-insecure \
            --basedir="${MYSQL_PREFIX}" --datadir="${MYSQL_DATA}" \
            --log-error="${LOG_DIR}/mysql-init.log"
    fi

    clear_stale_pid "${pid_file}"
    if pid_running "${pid_file}" &&
        "${MYSQL_BIN}/mysqladmin" --no-defaults --protocol=socket --socket="${MYSQL_SOCKET}" \
            --user=root ping >/dev/null 2>&1; then
        log "MySQL ya está iniciado."
    else
        rm -f "${pid_file}" "${MYSQL_SOCKET}"
        assert_port_free MySQL "${MYSQL_PORT}"
        log "Iniciando MySQL en 127.0.0.1:${MYSQL_PORT}"
        nohup "${MYSQL_BIN}/mysqld" --no-defaults \
            --basedir="${MYSQL_PREFIX}" --datadir="${MYSQL_DATA}" \
            --bind-address=127.0.0.1 --port="${MYSQL_PORT}" \
            --socket="${MYSQL_SOCKET}" --pid-file="${pid_file}" \
            --mysqlx=OFF --log-error="${LOG_DIR}/mysql.log" \
            >>"${LOG_DIR}/mysql-console.log" 2>&1 &
    fi

    wait_until MySQL 90 "${MYSQL_BIN}/mysqladmin" --no-defaults --protocol=socket \
        --socket="${MYSQL_SOCKET}" --user=root ping

    "${MYSQL_BIN}/mysql" --no-defaults --protocol=socket \
        --socket="${MYSQL_SOCKET}" --user=root <<SQL
CREATE DATABASE IF NOT EXISTS \`${MYSQL_DATABASE}\`
    CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER IF NOT EXISTS '${MYSQL_USER}'@'127.0.0.1'
    IDENTIFIED BY '${MYSQL_PASSWORD}';
ALTER USER '${MYSQL_USER}'@'127.0.0.1'
    IDENTIFIED BY '${MYSQL_PASSWORD}';
GRANT ALL PRIVILEGES ON \`${MYSQL_DATABASE}\`.* TO '${MYSQL_USER}'@'127.0.0.1';
FLUSH PRIVILEGES;
SQL
}

ensure_wildfly() {
    local archive="${DOWNLOADS_DIR}/wildfly-${WILDFLY_VERSION}.tar.gz"
    local extract_dir="${TOOLS_DIR}/.wildfly-extract-${WILDFLY_VERSION}"
    local jdbc_archive="${DOWNLOADS_DIR}/postgresql-${POSTGRES_JDBC_VERSION}.jar"
    local module_dir="${WILDFLY_HOME}/modules/org/postgresql/main"

    if [[ ! -x "${WILDFLY_HOME}/bin/standalone.sh" ]]; then
        download_file "WildFly ${WILDFLY_VERSION}" "${WILDFLY_URL}" "${archive}"
        tar -tzf "${archive}" >/dev/null ||
            die "El archivo descargado de WildFly no es un tar.gz válido."
        log "Extrayendo WildFly ${WILDFLY_VERSION}"
        rm -rf "${extract_dir}"
        mkdir -p "${extract_dir}"
        tar -xzf "${archive}" --strip-components=1 --directory="${extract_dir}"
        [[ -x "${extract_dir}/bin/standalone.sh" ]] ||
            die "La distribución descargada no contiene bin/standalone.sh."
        mv "${extract_dir}" "${WILDFLY_HOME}"
    fi

    download_file "JDBC PostgreSQL ${POSTGRES_JDBC_VERSION}" \
        "${POSTGRES_JDBC_URL}" "${jdbc_archive}"
    mkdir -p "${module_dir}"
    cp "${jdbc_archive}" "${module_dir}/postgresql.jar"
    cat >"${module_dir}/module.xml" <<'XML'
<?xml version="1.0" encoding="UTF-8"?>
<module xmlns="urn:jboss:module:1.9" name="org.postgresql">
    <resources>
        <resource-root path="postgresql.jar"/>
    </resources>
    <dependencies>
        <module name="java.sql"/>
        <module name="java.management"/>
        <module name="java.naming"/>
        <module name="jakarta.transaction.api"/>
    </dependencies>
</module>
XML
}

start_and_deploy_guacales() {
    local pid_file="${RUN_DIR}/wildfly.pid"
    local cli="${WILDFLY_HOME}/bin/jboss-cli.sh"
    local controller="127.0.0.1:${WILDFLY_MANAGEMENT_PORT}"
    local war="${GUACALES_REPO}/demo/target/demo.war"
    local ds_url="jdbc:postgresql://127.0.0.1:${PG_PORT}/${PG_DATABASE}"
    local cli_command

    clear_stale_pid "${pid_file}"
    if ! pid_running "${pid_file}"; then
        assert_port_free WildFly "${WILDFLY_HTTP_PORT}"
        assert_port_free "WildFly management" "${WILDFLY_MANAGEMENT_PORT}"
        log "Iniciando WildFly ${WILDFLY_VERSION} con Java 21"
        (
            cd "${WILDFLY_HOME}"
            nohup env JAVA_HOME="${JAVA_HOME}" \
                UTNGOLCOIN_BASE_URL="${URL_UTNGOLCOIN}" \
                "${WILDFLY_HOME}/bin/standalone.sh" \
                -Djboss.bind.address=127.0.0.1 \
                -Djboss.bind.address.management=127.0.0.1 \
                -Djboss.http.port="${WILDFLY_HTTP_PORT}" \
                -Djboss.management.http.port="${WILDFLY_MANAGEMENT_PORT}" \
                >>"${LOG_DIR}/wildfly.log" 2>&1 &
            printf '%s\n' "$!" >"${pid_file}"
        )
    else
        log "WildFly ya está iniciado."
    fi

    wait_until "WildFly management" 120 "${cli}" --connect \
        --controller="${controller}" --command=":read-attribute(name=server-state)"

    "${cli}" --connect --controller="${controller}" \
        --command="undeploy demo.war" >/dev/null 2>&1 || true

    if ! "${cli}" --connect --controller="${controller}" \
        --command="/subsystem=datasources/jdbc-driver=postgresql:read-resource" \
        >/dev/null 2>&1; then
        "${cli}" --connect --controller="${controller}" \
            --command="/subsystem=datasources/jdbc-driver=postgresql:add(driver-name=postgresql,driver-module-name=org.postgresql,driver-class-name=org.postgresql.Driver)"
    fi

    if "${cli}" --connect --controller="${controller}" \
        --command="/subsystem=datasources/data-source=GuacalesDS:read-resource" \
        >/dev/null 2>&1; then
        "${cli}" --connect --controller="${controller}" \
            --command="data-source remove --name=GuacalesDS" >/dev/null
    fi

    cli_command="data-source add --name=GuacalesDS --jndi-name=java:/GuacalesDS"
    cli_command+=" --driver-name=postgresql --connection-url=${ds_url}"
    cli_command+=" --user-name=${PG_USER} --password=${PG_PASSWORD} --enabled=true"
    "${cli}" --connect --controller="${controller}" --command="${cli_command}"
    "${cli}" --connect --controller="${controller}" \
        --command="/subsystem=datasources/data-source=GuacalesDS:test-connection-in-pool"

    log "Compilando Guacales con Maven y Java 21"
    (
        cd "${GUACALES_REPO}/demo"
        "${MAVEN_BIN}/mvn" clean package
    ) 2>&1 | tee "${LOG_DIR}/guacales-build.log"
    [[ -f "${war}" ]] || die "Maven no produjo ${war}"

    log "Desplegando demo.war en WildFly"
    "${cli}" --connect --controller="${controller}" \
        --command="deploy \"${war}\" --force"
    wait_http Guacales \
        "http://127.0.0.1:${WILDFLY_HTTP_PORT}/demo/api/v1/salud" 180
}

start_postgresql
start_mysql
ensure_wildfly
start_and_deploy_guacales

start_dotnet utn-golcoin \
    "${UTNGOLCOIN_REPO}/UTNGolCoinApi.csproj" \
    "http://${LISTEN}:5001" "http://127.0.0.1:5001/api/salud" \
    ASPNETCORE_ENVIRONMENT=Development \
    "ConnectionStrings__Default=server=127.0.0.1;port=${MYSQL_PORT};database=${MYSQL_DATABASE};user=${MYSQL_USER};password=${MYSQL_PASSWORD}" \
    "ServicioEstadisticas__BaseUrl=${URL_GUACALES}"

start_dotnet frontend-estadisticas \
    "${ROOT}/frontend-estadisticas-mvc/FrontendEstadisticas.csproj" \
    "http://${LISTEN}:${EST_PORT}" "http://127.0.0.1:${EST_PORT}/" \
    ASPNETCORE_ENVIRONMENT=Development \
    "Servicios__Estadisticas__DireccionBase=${URL_GUACALES}" \
    Servicios__Estadisticas__UsarSimulado=false \
    "Frontends__ApuestasUrl=http://${BIND_APU}"

start_dotnet frontend-apuestas \
    "${ROOT}/frontend-publico-mvc/FrontendPublico.csproj" \
    "http://${LISTEN}:${APU_PORT}" "http://127.0.0.1:${APU_PORT}/" \
    ASPNETCORE_ENVIRONMENT=Development \
    "Servicios__Estadisticas__DireccionBase=${URL_GUACALES}" \
    Servicios__Estadisticas__UsarSimulado=false \
    "Servicios__UTNGolCoin__DireccionBase=${URL_UTNGOLCOIN}" \
    Servicios__UTNGolCoin__UsarSimulado=false \
    "Frontends__EstadisticasUrl=http://${BIND_EST}"

start_dotnet frontend-admin \
    "${ADMIN_REPO}/FrontendAdministrativo.csproj" \
    "http://${LISTEN}:${ADMIN_PORT}" "http://127.0.0.1:${ADMIN_PORT}/Auth/Login" \
    ASPNETCORE_ENVIRONMENT=Development \
    "ApiEstadisticas__BaseUrl=${URL_GUACALES}" \
    "ApiUTNGolCoin__BaseUrl=${URL_UTNGOLCOIN}"

log "Pila completa y saludable. Procesos en background."
log "Fernando: http://${BIND_EST} | http://${BIND_APU}"
log "Administración: http://${BIND_ADMIN}"
log "Usa 'make stack-status', 'make stack-logs' o 'make stack-stop'."
