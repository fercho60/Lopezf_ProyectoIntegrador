#!/usr/bin/env bash
set -Eeuo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck source=stack-common.sh
source "${SCRIPT_DIR}/stack-common.sh"

load_config
ensure_layout

printf '=== Procesos administrados ===\n'
print_process_status PostgreSQL "${RUN_DIR}/postgresql.pid"
print_process_status MySQL "${RUN_DIR}/mysql.pid"
print_process_status WildFly "${RUN_DIR}/wildfly.pid"
print_process_status UTNGolCoin "${RUN_DIR}/utn-golcoin.pid"
print_process_status "Frontend estadísticas" "${RUN_DIR}/frontend-estadisticas.pid"
print_process_status "Frontend apuestas" "${RUN_DIR}/frontend-apuestas.pid"
print_process_status "Frontend admin" "${RUN_DIR}/frontend-admin.pid"

printf '\n=== Salud ===\n'
if command -v brew >/dev/null 2>&1; then
    PG_PREFIX="$(brew --prefix postgresql@16 2>/dev/null || true)"
    if [[ -x "${PG_PREFIX}/bin/pg_isready" ]] &&
        "${PG_PREFIX}/bin/pg_isready" --host=127.0.0.1 \
            --port="${PG_PORT}" --username="${PG_USER}" >/dev/null 2>&1; then
        printf '%-24s saludable  127.0.0.1:%s/%s\n' PostgreSQL "${PG_PORT}" "${PG_DATABASE}"
    else
        printf '%-24s sin respuesta  127.0.0.1:%s\n' PostgreSQL "${PG_PORT}"
    fi

    MYSQL_PREFIX="$(brew --prefix mysql 2>/dev/null || true)"
    if [[ -x "${MYSQL_PREFIX}/bin/mysqladmin" ]] &&
        "${MYSQL_PREFIX}/bin/mysqladmin" --no-defaults --protocol=socket \
            --socket="${MYSQL_SOCKET}" --user=root ping >/dev/null 2>&1; then
        printf '%-24s saludable  127.0.0.1:%s/%s\n' MySQL "${MYSQL_PORT}" "${MYSQL_DATABASE}"
    else
        printf '%-24s sin respuesta  127.0.0.1:%s\n' MySQL "${MYSQL_PORT}"
    fi
else
    printf '%-24s no verificable (falta brew)\n' PostgreSQL
    printf '%-24s no verificable (falta brew)\n' MySQL
fi

print_http_status Guacales \
    "http://127.0.0.1:${WILDFLY_HTTP_PORT}/demo/api/v1/salud"
print_http_status UTNGolCoin "http://127.0.0.1:5001/api/salud"
print_http_status "Frontend estadísticas" "http://127.0.0.1:${EST_PORT}/"
print_http_status "Frontend apuestas" "http://127.0.0.1:${APU_PORT}/"
print_http_status "Frontend admin" "http://127.0.0.1:${ADMIN_PORT}/Auth/Login"

printf '\nData dirs: %s\nLogs:      %s\n' "${DATA_DIR}" "${LOG_DIR}"
