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

## 5. Conectar los frontends (IPs fáciles: `equipo.env`)

No hace falta editar `appsettings.json` cada vez. En la raíz del repo:

```bash
cp equipo.env.example equipo.env   # una sola vez
# edita equipo.env con las IPs del día
make urls                          # verifica qué va a usar
make stop && make run
```

### 5.1 Todo en una sola PC (probar en conjunto local)

1. Andrea/Mayra: Guacales en `:8080` y UTNGolCoin en `:5000` (escuchando `0.0.0.0` o localhost).
2. `equipo.env` deja los defaults de `equipo.env.example` (localhost).
3. `make run` → `:5080` y `:5081`.
4. Flujo: ver partidos → registrarse → apostar → (Joel o curl) registrar resultado → ver premio.

### 5.2 Mañana: cada uno en su máquina (misma Wi‑Fi)

En `equipo.env` de **quien corra los frontends**:

```bash
URL_GUACALES=http://IP_ANDREA:8080/demo/api/v1/
URL_UTNGOLCOIN=http://IP_MAYRA:5000/api/
USAR_SIMULADO=false
BIND_EST=localhost:5080
BIND_APU=localhost:5081
```

Si estadísticas y apuestas van en PCs distintas, cambia también `BIND_EST` / `BIND_APU`
a la IP de cada PC (para que los enlaces entre portales no apunten a localhost ajeno).

`make urls` muestra la configuración efectiva. Reinicia con `make stop && make run`.

Prueba de integración completa:

1. `http://localhost:5080` muestra partidos reales de Guacales.
2. Registrarse en `:5081` crea billetera en Mayra (Guacales → `POST /api/billeteras`).
3. Predicción + resultado oficial → liquidación (`POST /api/liquidaciones/{id}`).

## 6. Si el equipo no está en la misma red (último recurso)

Solo si no hay LAN común: túnel tipo [ngrok](https://ngrok.com/). **No** dejes
URLs de ngrok fijas en `appsettings.json`; ponlas en `equipo.env` ese día:

```bash
URL_GUACALES=https://xxxx.ngrok-free.dev/demo/api/v1/
URL_UTNGOLCOIN=https://yyyy.ngrok-free.dev/api/
```
## 7. Problemas frecuentes

| Problema | Solución |
|---|---|
| `address already in use` en 5080/5081 | `make stop` y volver a `make run` |
| El segundo `dotnet run` nunca arranca | No usar `cd A && dotnet run` dos veces en la misma terminal; usar `make run` o dos terminales |
| Error de CORS en el navegador | Revisar `CorsFilter` en Guacales y que la URL configurada sea la misma que se está usando |
| Frontend muestra datos simulados con backends levantados | Verificar `UsarSimulado: false` y reiniciar los frontends |
| Curl funciona pero el frontend no carga datos | Revisar que `DireccionBase` termine en `/` y apunte a la IP correcta |
