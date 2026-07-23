#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
if [[ -f "$ROOT/equipo.env" ]]; then
  # shellcheck disable=SC1091
  source "$ROOT/equipo.env"
fi

URL_GUACALES="${URL_GUACALES:-http://127.0.0.1:18080/demo/api/v1/}"
URL_UTNGOLCOIN="${URL_UTNGOLCOIN:-http://127.0.0.1:5001/api/}"
URL_GUACALES="${URL_GUACALES%/}/"
URL_UTNGOLCOIN="${URL_UTNGOLCOIN%/}/"

marca="$(date +%s)"
codigo="$(printf '%04X' "$((marca % 65536))")"
fecha_partido="$(python3 - <<'PY'
from datetime import datetime, timedelta
print((datetime.now() + timedelta(days=1)).replace(microsecond=0).isoformat())
PY
)"

post_json() {
  local url="$1"
  local body="$2"
  curl --fail-with-body --silent --show-error \
    -H "Content-Type: application/json" \
    -X POST "$url" \
    -d "$body"
}

echo "1/8 Verificando APIs reales"
curl --fail --silent "${URL_GUACALES}salud" | jq -e '.estado == "OK"' >/dev/null
curl --fail --silent "${URL_UTNGOLCOIN}salud" | jq -e '.estado == "OK"' >/dev/null

echo "2/8 Creando sede persistente"
sede="$(
  post_json "${URL_GUACALES}sedes" \
    "{\"nombre\":\"Estadio Integracion $marca\",\"ciudad\":\"Ibarra\",\"pais\":\"Ecuador\",\"capacidad\":20000}"
)"
sede_id="$(jq -er '.id' <<<"$sede")"

echo "3/8 Creando dos selecciones persistentes"
local_json="$(
  post_json "${URL_GUACALES}selecciones" \
    "{\"nombre\":\"Local $marca\",\"codigoFifa\":\"L$codigo\"}"
)"
visitante_json="$(
  post_json "${URL_GUACALES}selecciones" \
    "{\"nombre\":\"Visitante $marca\",\"codigoFifa\":\"V$codigo\"}"
)"
local_id="$(jq -er '.id' <<<"$local_json")"
visitante_id="$(jq -er '.id' <<<"$visitante_json")"

echo "4/8 Creando partido real de octavos"
partido="$(
  post_json "${URL_GUACALES}partidos" \
    "{\"seleccionLocalId\":$local_id,\"seleccionVisitanteId\":$visitante_id,\"sedeId\":$sede_id,\"fechaHora\":\"$fecha_partido\",\"fase\":\"OCTAVOS\",\"estado\":\"PROGRAMADO\",\"cuotaLocal\":2.0,\"cuotaEmpate\":3.0,\"cuotaVisitante\":2.5}"
)"
partido_id="$(jq -er '.id' <<<"$partido")"

echo "5/8 Registrando usuario y bono de bienvenida"
correo="integracion.$marca@utn.edu.ec"
usuario="$(
  post_json "${URL_GUACALES}autenticacion/registro" \
    "{\"nombre\":\"Prueba Integracion\",\"correo\":\"$correo\",\"contrasena\":\"Clave123!\"}"
)"
usuario_id="$(jq -er '.usuario.id' <<<"$usuario")"

saldo_inicial="$(curl --fail --silent "${URL_UTNGOLCOIN}billeteras/$usuario_id" | jq -er '.saldo')"
[[ "$saldo_inicial" == "10" || "$saldo_inicial" == "10.0" || "$saldo_inicial" == "10.00" ]]

echo "6/8 Registrando predicción real"
post_json "${URL_UTNGOLCOIN}predicciones" \
  "{\"usuarioId\":$usuario_id,\"partidoId\":$partido_id,\"resultadoPronosticado\":\"LOCAL\",\"monto\":2}" \
  | jq -e '.estado == "PENDIENTE"' >/dev/null

echo "7/8 Registrando resultado y disparando liquidación"
curl --fail-with-body --silent --show-error \
  -H "Content-Type: application/json" \
  -X PUT "${URL_GUACALES}partidos/$partido_id/resultado" \
  -d '{"golesLocal":2,"golesVisitante":0}' \
  | jq -e '.estado == "FINALIZADO"' >/dev/null

echo "8/8 Verificando premio y sincronización"
prediccion="$(
  curl --fail --silent "${URL_UTNGOLCOIN}predicciones/usuario/$usuario_id" \
    | jq -er --argjson partido "$partido_id" '.[] | select(.partidoId == $partido)'
)"
jq -e '.estado == "GANADA"' <<<"$prediccion" >/dev/null
saldo_final="$(curl --fail --silent "${URL_UTNGOLCOIN}billeteras/$usuario_id" | jq -er '.saldo')"

echo
echo "Sincronización real verificada"
echo "  usuarioId=$usuario_id partidoId=$partido_id fase=OCTAVOS"
echo "  saldo inicial=$saldo_inicial | apuesta=2 | cuota=2 | saldo final=$saldo_final"
