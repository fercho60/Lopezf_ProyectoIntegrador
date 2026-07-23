#!/usr/bin/env bash

# Utilidades compartidas por el orquestador nativo. Este archivo se carga
# desde los scripts ejecutables y no debe invocarse directamente.

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"
FERN_ROOT="$(cd "${ROOT}/.." && pwd)"
STACK_DIR="${ROOT}/.stack"
DATA_DIR="${STACK_DIR}/data"
TOOLS_DIR="${STACK_DIR}/tools"
DOWNLOADS_DIR="${STACK_DIR}/downloads"
RUN_DIR="${STACK_DIR}/run"
LOG_DIR="${ROOT}/logs"

log() {
    printf '[stack] %s\n' "$*"
}

warn() {
    printf '[stack] ADVERTENCIA: %s\n' "$*" >&2
}

die() {
    printf '[stack] ERROR: %s\n' "$*" >&2
    exit 1
}

load_config() {
    if [[ -f "${ROOT}/equipo.env" ]]; then
        # shellcheck disable=SC1091
        set -a
        source "${ROOT}/equipo.env"
        set +a
    fi

    GUACALES_REPO="${GUACALES_REPO:-${FERN_ROOT}/GuacalesA_ProyectoIntegrador}"
    UTNGOLCOIN_REPO="${UTNGOLCOIN_REPO:-${FERN_ROOT}/SantacruzMayra_Proyecto_Integrador}"
    ADMIN_REPO="${ADMIN_REPO:-${FERN_ROOT}/PumaJoel_ProyectoIntegrador/FrontendAdministrativo}"

    URL_GUACALES="${URL_GUACALES:-http://localhost:18080/demo/api/v1/}"
    URL_UTNGOLCOIN="${URL_UTNGOLCOIN:-http://localhost:5001/api/}"
    BIND_EST="${BIND_EST:-localhost:5080}"
    BIND_APU="${BIND_APU:-localhost:5081}"
    BIND_ADMIN="${BIND_ADMIN:-localhost:5203}"
    LISTEN="${LISTEN:-0.0.0.0}"

    PG_PORT="${PG_PORT:-15432}"
    PG_DATABASE="${PG_DATABASE:-guacales_db}"
    PG_USER="${PG_USER:-postgres}"
    PG_PASSWORD="${PG_PASSWORD:-postgres}"
    MYSQL_PORT="${MYSQL_PORT:-13306}"
    MYSQL_DATABASE="${MYSQL_DATABASE:-utngolcoin_db}"
    MYSQL_USER="${MYSQL_USER:-utngolcoin}"
    MYSQL_PASSWORD="${MYSQL_PASSWORD:-utngolcoin}"

    WILDFLY_VERSION="${WILDFLY_VERSION:-39.0.1.Final}"
    POSTGRES_JDBC_VERSION="${POSTGRES_JDBC_VERSION:-42.7.8}"
    WILDFLY_HTTP_PORT="${WILDFLY_HTTP_PORT:-18080}"
    WILDFLY_MANAGEMENT_PORT="${WILDFLY_MANAGEMENT_PORT:-19990}"

    PG_DATA="${DATA_DIR}/postgresql"
    MYSQL_DATA="${DATA_DIR}/mysql"
    MYSQL_SOCKET="${RUN_DIR}/mysql.sock"
    WILDFLY_HOME="${TOOLS_DIR}/wildfly-${WILDFLY_VERSION}"
    WILDFLY_URL="${WILDFLY_URL:-https://github.com/wildfly/wildfly/releases/download/${WILDFLY_VERSION}/wildfly-${WILDFLY_VERSION}.tar.gz}"
    POSTGRES_JDBC_URL="${POSTGRES_JDBC_URL:-https://repo1.maven.org/maven2/org/postgresql/postgresql/${POSTGRES_JDBC_VERSION}/postgresql-${POSTGRES_JDBC_VERSION}.jar}"

    EST_PORT="${BIND_EST##*:}"
    APU_PORT="${BIND_APU##*:}"
    ADMIN_PORT="${BIND_ADMIN##*:}"
    [[ "${EST_PORT}" == "${BIND_EST}" ]] && EST_PORT=5080
    [[ "${APU_PORT}" == "${BIND_APU}" ]] && APU_PORT=5081
    [[ "${ADMIN_PORT}" == "${BIND_ADMIN}" ]] && ADMIN_PORT=5203

    export GUACALES_REPO UTNGOLCOIN_REPO ADMIN_REPO
    export URL_GUACALES URL_UTNGOLCOIN BIND_EST BIND_APU BIND_ADMIN LISTEN
    export PG_PORT PG_DATABASE PG_USER PG_PASSWORD MYSQL_PORT
    export MYSQL_DATABASE MYSQL_USER MYSQL_PASSWORD WILDFLY_VERSION
    export POSTGRES_JDBC_VERSION WILDFLY_HTTP_PORT WILDFLY_MANAGEMENT_PORT
    export PG_DATA MYSQL_DATA MYSQL_SOCKET WILDFLY_HOME WILDFLY_URL
    export POSTGRES_JDBC_URL EST_PORT APU_PORT ADMIN_PORT
}

ensure_layout() {
    mkdir -p "${DATA_DIR}" "${TOOLS_DIR}" "${DOWNLOADS_DIR}" "${RUN_DIR}" "${LOG_DIR}"
}

require_command() {
    command -v "$1" >/dev/null 2>&1 || die "Falta el comando requerido: $1"
}

brew_prefix() {
    local formula=$1
    local prefix
    prefix="$(brew --prefix "${formula}" 2>/dev/null)" ||
        die "Falta Homebrew formula '${formula}'. Instálala antes de ejecutar make stack."
    [[ -d "${prefix}" ]] || die "Homebrew devolvió una ruta inválida para ${formula}: ${prefix}"
    printf '%s\n' "${prefix}"
}

validate_identifier() {
    local name=$1
    local value=$2
    [[ "${value}" =~ ^[A-Za-z0-9_]+$ ]] ||
        die "${name} solo admite letras, números y guion bajo."
}

validate_secret() {
    local name=$1
    local value=$2
    [[ "${value}" =~ ^[A-Za-z0-9._-]+$ ]] ||
        die "${name} solo admite letras, números, punto, guion y guion bajo."
}

validate_config() {
    [[ -f "${GUACALES_REPO}/demo/pom.xml" ]] ||
        die "No se encontró Guacales en ${GUACALES_REPO}"
    [[ -f "${UTNGOLCOIN_REPO}/UTNGolCoinApi.csproj" ]] ||
        die "No se encontró UTNGolCoin en ${UTNGOLCOIN_REPO}"
    [[ -f "${ADMIN_REPO}/FrontendAdministrativo.csproj" ]] ||
        die "No se encontró el frontend administrativo en ${ADMIN_REPO}"

    validate_identifier PG_DATABASE "${PG_DATABASE}"
    validate_identifier PG_USER "${PG_USER}"
    validate_secret PG_PASSWORD "${PG_PASSWORD}"
    validate_identifier MYSQL_DATABASE "${MYSQL_DATABASE}"
    validate_identifier MYSQL_USER "${MYSQL_USER}"
    validate_secret MYSQL_PASSWORD "${MYSQL_PASSWORD}"

    local port
    for port in "${PG_PORT}" "${MYSQL_PORT}" "${WILDFLY_HTTP_PORT}" \
        "${WILDFLY_MANAGEMENT_PORT}" "${EST_PORT}" "${APU_PORT}" "${ADMIN_PORT}"; do
        [[ "${port}" =~ ^[0-9]+$ ]] && ((port > 0 && port < 65536)) ||
            die "Puerto inválido: ${port}"
    done
}

pid_running() {
    local pid_file=$1
    local pid
    [[ -s "${pid_file}" ]] || return 1
    read -r pid <"${pid_file}"
    [[ "${pid}" =~ ^[0-9]+$ ]] && kill -0 "${pid}" 2>/dev/null
}

clear_stale_pid() {
    local pid_file=$1
    if [[ -f "${pid_file}" ]] && ! pid_running "${pid_file}"; then
        rm -f "${pid_file}"
    fi
}

assert_port_free() {
    local label=$1
    local port=$2
    if lsof -nP -iTCP:"${port}" -sTCP:LISTEN -t 2>/dev/null | grep -q .; then
        die "${label} no puede iniciar: el puerto ${port} ya está ocupado."
    fi
}

http_ready() {
    local url=$1
    curl --silent --show-error --fail --max-time 3 --output /dev/null "${url}" 2>/dev/null
}

wait_http() {
    local label=$1
    local url=$2
    local timeout=${3:-120}
    local started=${SECONDS}
    until http_ready "${url}"; do
        if ((SECONDS - started >= timeout)); then
            die "${label} no respondió en ${url} después de ${timeout}s. Revisa ${LOG_DIR}."
        fi
        sleep 2
    done
    log "${label} saludable: ${url}"
}

wait_until() {
    local label=$1
    local timeout=$2
    shift 2
    local started=${SECONDS}
    until "$@" >/dev/null 2>&1; do
        if ((SECONDS - started >= timeout)); then
            die "${label} no quedó disponible después de ${timeout}s. Revisa ${LOG_DIR}."
        fi
        sleep 1
    done
    log "${label} disponible"
}

download_file() {
    local label=$1
    local url=$2
    local destination=$3
    local temporary="${destination}.part"
    [[ -s "${destination}" ]] && return 0
    log "Descargando ${label}"
    rm -f "${temporary}"
    curl --fail --location --retry 3 --retry-delay 2 \
        --output "${temporary}" "${url}"
    mv "${temporary}" "${destination}"
}

dotnet_target_path() {
    local project=$1
    dotnet build "${project}" --nologo >&2
    dotnet msbuild "${project}" -nologo -getProperty:TargetPath | tail -n 1
}

start_dotnet() {
    local name=$1
    local project=$2
    local url=$3
    local health_url=$4
    shift 4
    local pid_file="${RUN_DIR}/${name}.pid"
    local log_file="${LOG_DIR}/${name}.log"
    local target_path

    if http_ready "${health_url}"; then
        log "${name} ya responde; no se reinicia."
        return 0
    fi
    clear_stale_pid "${pid_file}"
    [[ ! -f "${pid_file}" ]] || die "${name} tiene un PID activo pero no responde."
    assert_port_free "${name}" "${url##*:}"

    log "Compilando ${name}"
    target_path="$(dotnet_target_path "${project}")"
    [[ -f "${target_path}" ]] || die "No se encontró el ensamblado de ${name}: ${target_path}"

    log "Iniciando ${name} en ${url}"
    (
        cd "$(dirname "${project}")"
        nohup env "$@" dotnet "${target_path}" --urls "${url}" \
            >>"${log_file}" 2>&1 &
        printf '%s\n' "$!" >"${pid_file}"
    )
    wait_http "${name}" "${health_url}" 120
}

terminate_pid_file() {
    local name=$1
    local pid_file=$2
    local pid
    if ! pid_running "${pid_file}"; then
        rm -f "${pid_file}"
        log "${name}: detenido"
        return 0
    fi
    read -r pid <"${pid_file}"
    log "Deteniendo ${name} (PID ${pid})"
    kill -TERM "${pid}" 2>/dev/null || true
    local attempt
    for attempt in {1..20}; do
        kill -0 "${pid}" 2>/dev/null || break
        sleep 0.5
    done
    if kill -0 "${pid}" 2>/dev/null; then
        warn "${name} no terminó a tiempo; enviando KILL."
        kill -KILL "${pid}" 2>/dev/null || true
    fi
    rm -f "${pid_file}"
}

print_process_status() {
    local name=$1
    local pid_file=$2
    if pid_running "${pid_file}"; then
        local pid
        read -r pid <"${pid_file}"
        printf '%-24s activo (PID %s)\n' "${name}" "${pid}"
    else
        printf '%-24s detenido\n' "${name}"
    fi
}

print_http_status() {
    local name=$1
    local url=$2
    if http_ready "${url}"; then
        printf '%-24s saludable  %s\n' "${name}" "${url}"
    else
        printf '%-24s sin respuesta  %s\n' "${name}" "${url}"
    fi
}
