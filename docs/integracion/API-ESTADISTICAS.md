# API de Estadísticas — notas para el monorepo (Persona 1)

Base local del WAR Guacales:

```
http://localhost:8080/demo/api/v1
```

El frontend público y el administrativo deben usar esta base. Endpoint de humo:

```
GET /salud  →  { "estado": "OK", "servicio": "estadisticas" }
```

Tras registrar un usuario o un resultado oficial, este servicio notifica a UTNGolCoin
(`billeteras` / `liquidaciones`). Mantener `UTNGOLCOIN_BASE_URL` alineada en despliegue.
