# Carga manual desde octavos

No se incluye el seed RF28. La base nueva contiene únicamente los roles técnicos
necesarios para registrar usuarios; selecciones, sedes y partidos se ingresan por
las APIs reales.

## 1. Levantar la pila

```bash
cp equipo.env.example equipo.env
make stack
make stack-status
```

## 2. Crear una sede

```bash
curl --fail-with-body \
  -H 'Content-Type: application/json' \
  -d '{"nombre":"Estadio 1","ciudad":"Ciudad","pais":"País","capacidad":40000}' \
  http://localhost:18080/demo/api/v1/sedes
```

Guarda el `id` de la respuesta.

## 3. Crear las 16 selecciones clasificadas

Ejecuta una petición por selección:

```bash
curl --fail-with-body \
  -H 'Content-Type: application/json' \
  -d '{"nombre":"Ecuador","codigoFifa":"ECU"}' \
  http://localhost:18080/demo/api/v1/selecciones
```

No se requiere `grupoId` porque la carga comienza en fase eliminatoria.

## 4. Crear los ocho partidos

Abre `http://localhost:5203/Partidos/Crear`, inicia sesión con las credenciales
de `equipo.env` y elige las selecciones y sede persistidas. La fase para este
alcance es `OCTAVOS`.

También se puede usar la API directamente:

```bash
curl --fail-with-body \
  -H 'Content-Type: application/json' \
  -d '{
    "numeroPartidoFifa": 89,
    "seleccionLocalId": 1,
    "seleccionVisitanteId": 2,
    "sedeId": 1,
    "fechaHora": "2026-07-30T20:00:00",
    "fase": "OCTAVOS",
    "estado": "PROGRAMADO",
    "cuotaLocal": 2.0,
    "cuotaEmpate": 3.0,
    "cuotaVisitante": 2.5
  }' \
  http://localhost:18080/demo/api/v1/partidos
```

Los números FIFA de octavos son 89–96. Repite la operación con equipos
diferentes. Los registros quedan en PostgreSQL; no son datos simulados.

## 5. Probar sincronización

```bash
make prueba-integracion
```

La prueba crea datos identificables, registra un usuario, verifica el bono de
10 UGC, apuesta 2 UGC al local, publica un resultado real en Guacales y confirma
que UTNGolCoin marque la predicción como ganada y acredite el premio.
