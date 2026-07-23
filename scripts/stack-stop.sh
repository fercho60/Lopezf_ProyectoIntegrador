#!/usr/bin/env bash
set -Eeuo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck source=stack-common.sh
source "${SCRIPT_DIR}/stack-common.sh"

load_config
ensure_layout

terminate_pid_file frontend-admin "${RUN_DIR}/frontend-admin.pid"
terminate_pid_file frontend-apuestas "${RUN_DIR}/frontend-apuestas.pid"
terminate_pid_file frontend-estadisticas "${RUN_DIR}/frontend-estadisticas.pid"
terminate_pid_file utn-golcoin "${RUN_DIR}/utn-golcoin.pid"

if [[ -x "${WILDFLY_HOME}/bin/jboss-cli.sh" ]] &&
    pid_running "${RUN_DIR}/wildfly.pid"; then
    log "Deteniendo WildFly"
    "${WILDFLY_HOME}/bin/jboss-cli.sh" --connect \
        --controller="127.0.0.1:${WILDFLY_MANAGEMENT_PORT}" \
        --command=":shutdown" >/dev/null 2>&1 || true
    for _attempt in {1..30}; do
        pid_running "${RUN_DIR}/wildfly.pid" || break
        sleep 0.5
    done
fi
if pid_running "${RUN_DIR}/wildfly.pid"; then
    terminate_pid_file WildFly "${RUN_DIR}/wildfly.pid"
else
    rm -f "${RUN_DIR}/wildfly.pid"
    log "WildFly: detenido"
fi

if command -v brew >/dev/null 2>&1; then
    MYSQL_PREFIX="$(brew --prefix mysql 2>/dev/null || true)"
    if [[ -x "${MYSQL_PREFIX}/bin/mysqladmin" ]] &&
        pid_running "${RUN_DIR}/mysql.pid"; then
        log "Deteniendo MySQL"
        "${MYSQL_PREFIX}/bin/mysqladmin" --no-defaults --protocol=socket \
            --socket="${MYSQL_SOCKET}" --user=root shutdown >/dev/null 2>&1 || true
        for _attempt in {1..30}; do
            pid_running "${RUN_DIR}/mysql.pid" || break
            sleep 0.5
        done
    fi
    if pid_running "${RUN_DIR}/mysql.pid"; then
        terminate_pid_file MySQL "${RUN_DIR}/mysql.pid"
    else
        rm -f "${RUN_DIR}/mysql.pid" "${MYSQL_SOCKET}"
        log "MySQL: detenido"
    fi

    PG_PREFIX="$(brew --prefix postgresql@16 2>/dev/null || true)"
    if [[ -x "${PG_PREFIX}/bin/pg_ctl" ]] &&
        [[ -s "${PG_DATA}/PG_VERSION" ]] &&
        "${PG_PREFIX}/bin/pg_ctl" status --pgdata="${PG_DATA}" >/dev/null 2>&1; then
        log "Deteniendo PostgreSQL"
        "${PG_PREFIX}/bin/pg_ctl" stop --pgdata="${PG_DATA}" \
            --mode=fast --wait >/dev/null
    fi
fi
rm -f "${RUN_DIR}/postgresql.pid"
log "PostgreSQL: detenido"
log "Pila detenida. Los data dirs locales se conservaron en ${DATA_DIR}."
