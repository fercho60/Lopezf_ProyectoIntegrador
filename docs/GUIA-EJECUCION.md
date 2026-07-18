# Guía de ejecución y pruebas — UTN GolMundial 2026

Guía para todo el equipo: cómo levantar los frontends, cómo probar los backends
de cada integrante y cómo verificar la integración completa entre máquinas.

## 1. Requisitos

| Herramienta | Para qué | Quién la necesita |
|---|---|---|
| SDK de .NET 9+ | Frontends y UTNGolCoin | Todos |
| Make | Atajos de ejecución (`make run`) | Quien ejecute los frontends |
| WildFly + PostgreSQL | Backend de Estadísticas (Guacales) | Andrea |
| MySQL | Backend UTNGolCoin | Mayra |

En macOS/Linux, Make ya viene instalado. En Windows se puede usar
[Make para Windows](https://gnuwin32.sourceforge.net/packages/make.htm) o Git Bash,
o ejecutar los comandos `dotnet run` manualmente (ver sección 2.1).

## 2. Ejecución rápida (modo simulado, sin backends)

Los frontends traen datos simulados (`UsarSimulado: true` en `appsettings.json`),
así que funcionan solos sin bases de datos ni backends. Desde la **raíz del repo**:

```bash
make run     # levanta ambos portales
make stop    # los detiene y libera los puertos
make status  # muestra qué puertos están ocupados
make help    # lista completa de comandos
```

| Portal | URL | Acceso |
|---|---|---|
| Estadísticas (invitado) | http://localhost:5080 | Sin registro |
| Apuestas | http://localhost:5081 | Registro obligatorio para apostar |

En el portal de estadísticas, el botón **“Ir a apuestas y registrarme”**
abre el portal de apuestas en una pestaña nueva.

### Usuarios de prueba (portal de apuestas, modo simulado)

| Correo | Contraseña |
|---|---|
| `diego@utn.edu.ec` | `Clave123!` |
| `maria@utn.edu.ec` | `Clave123!` |

También puedes registrar un usuario nuevo: recibe el bono de bienvenida
de 10 UTNGolCoin.

### 2.1 Si no tienes Make

`dotnet run` bloquea la terminal, así que usa **dos terminales**:

```bash
# Terminal 1
cd frontend-estadisticas-mvc && dotnet run --urls http://localhost:5080

# Terminal 2
cd frontend-publico-mvc && dotnet run --urls http://localhost:5081
```

## 3. Levantar los backends reales

### 3.1 Estadísticas — Guacales (Andrea)

Instrucciones completas en `backend-estadisticas-java/README-SETUP.md`. Resumen:

1. PostgreSQL corriendo con la base y el datasource `GuacalesDS`.
2. Desplegar `demo.war` en WildFly.
3. **Importante para pruebas entre máquinas:** arrancar WildFly escuchando en
   todas las interfaces:
   - Windows: `standalone.bat -b 0.0.0.0`
   - Linux/Mac: `./standalone.sh -b 0.0.0.0`

La API queda en `http://<ip-de-andrea>:8080/demo/api/v1/`.

### 3.2 UTNGolCoin — Santacruz (Mayra)

Instrucciones completas en `backend-utngolcoin-dotnet/README.md`. Resumen:

1. MySQL corriendo con la cadena de conexión de `appsettings.json`.
2. **Importante para pruebas entre máquinas:** arrancar escuchando en todas
   las interfaces:

```bash
dotnet run --urls http://0.0.0.0:5000
```

La API queda en `http://<ip-de-mayra>:5000/api/`.

### 3.3 Averiguar la IP local de cada máquina

- Windows: `ipconfig` → campo “Dirección IPv4” (ej. `192.168.1.34`).
- Mac: `ipconfig getifaddr en0`.

Todos deben estar conectados a la **misma red Wi-Fi** y el firewall debe
permitir conexiones entrantes (en Windows, aceptar el aviso para “redes privadas”).

## 4. Comprobar los backends desde otra máquina

Ambos backends tienen un endpoint de salud. Desde cualquier máquina del equipo
(reemplaza las IPs por las reales):

```bash
# Guacales vivo → {"estado":"OK","servicio":"estadisticas",...}
curl http://192.168.1.34:8080/demo/api/v1/salud

# Datos reales de Guacales
curl http://192.168.1.34:8080/demo/api/v1/partidos
curl http://192.168.1.34:8080/demo/api/v1/grupos

# UTNGolCoin vivo → {"estado":"OK","servicio":"utngolcoin",...}
curl http://192.168.1.35:5000/api/salud
```

| Resultado del curl | Significado |
|---|---|
| JSON con `"estado":"OK"` | Backend accesible: todo bien |
| `connection refused` | El backend no está corriendo o no arrancó con `0.0.0.0` |
| Se queda colgado | Firewall de la máquina del backend bloqueando |
| 404 | La app corre pero la ruta/contexto está mal (revisar `/demo`) |

## 5. Conectar los frontends a los backends reales

Edita `appsettings.json` en **ambos** frontends
(`frontend-estadisticas-mvc/` y `frontend-publico-mvc/`):

```json
"Estadisticas": {
  "DireccionBase": "http://192.168.1.34:8080/demo/api/v1/",
  "UsarSimulado": false
},
"UTNGolCoin": {
  "DireccionBase": "http://192.168.1.35:5000/api/",
  "UsarSimulado": false
}
```

(El frontend de estadísticas solo tiene la sección `Estadisticas`.)

Luego reinicia:

```bash
make stop && make run
```

Prueba de integración completa:

1. `http://localhost:5080` debe mostrar los partidos reales de la base de Andrea.
2. Registrarse en `http://localhost:5081` debe crear la billetera en el
   servicio de Mayra (Guacales llama a `POST /api/billeteras`).
3. Hacer una predicción y registrar el resultado del partido debe liquidar
   las UTNGolCoin (`POST /api/liquidaciones/{id}`).

## 6. Si el equipo no está en la misma red

Quien tenga el backend puede exponerlo con un túnel, por ejemplo con
[ngrok](https://ngrok.com/):

```bash
ngrok http 8080   # Andrea (Guacales)
ngrok http 5000   # Mayra (UTNGolCoin)
```

ngrok entrega una URL pública tipo `https://xxxx.ngrok.io`. Esa URL se usa en
los `curl` de la sección 4 y en `DireccionBase` de la sección 5
(para Guacales: `https://xxxx.ngrok.io/demo/api/v1/`).

## 7. Problemas frecuentes

| Problema | Solución |
|---|---|
| `address already in use` en 5080/5081 | `make stop` y volver a `make run` |
| El segundo `dotnet run` nunca arranca | No usar `cd A && dotnet run` dos veces en la misma terminal; usar `make run` o dos terminales |
| Error de CORS en el navegador | Revisar `CorsFilter` en Guacales y que la URL configurada sea la misma que se está usando |
| Frontend muestra datos simulados con backends levantados | Verificar `UsarSimulado: false` y reiniciar los frontends |
| Curl funciona pero el frontend no carga datos | Revisar que `DireccionBase` termine en `/` y apunte a la IP correcta |
