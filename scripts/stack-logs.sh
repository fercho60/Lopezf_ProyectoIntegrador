#!/usr/bin/env bash
set -Eeuo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# shellcheck source=stack-common.sh
source "${SCRIPT_DIR}/stack-common.sh"

load_config
ensure_layout

log_names=(
    postgresql.log
    mysql-init.log
    mysql.log
    mysql-console.log
    wildfly.log
    guacales-build.log
    utn-golcoin.log
    frontend-estadisticas.log
    frontend-apuestas.log
    frontend-admin.log
)
logs=()
for log_name in "${log_names[@]}"; do
    [[ -f "${LOG_DIR}/${log_name}" ]] && logs+=("${LOG_DIR}/${log_name}")
done

if ((${#logs[@]} == 0)); then
    log "Todavía no hay logs. Ejecuta 'make stack' primero."
    exit 0
fi

log "Siguiendo ${#logs[@]} archivos en ${LOG_DIR} (Ctrl+C para salir)"
tail -n 80 -F "${logs[@]}"
