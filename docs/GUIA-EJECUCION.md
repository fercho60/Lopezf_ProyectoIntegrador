# Guía de ejecución real

La integración usa procesos nativos, bases persistentes y HTTP en la red local.
No requiere Docker ni ngrok y no usa datos simulados.

## Repositorios

Clona los repositorios como carpetas hermanas:

```text
fern/
├── Lopezf_ProyectoIntegrador/
├── GuacalesA_ProyectoIntegrador/
├── SantacruzMayra_Proyecto_Integrador/
└── PumaJoel_ProyectoIntegrador/
```

Cada dueño recibe cambios en su repositorio. Las copias históricas de backends
que existen dentro de Fernando no intervienen en el arranque.

## Requisitos nativos en macOS

```bash
brew install postgresql@16 mysql maven openjdk@21
dotnet --version
```

WildFly y el driver JDBC se descargan automáticamente dentro de `.stack/tools`
la primera vez. No se instalan contenedores.

## Todo local en una máquina

```bash
cd Lopezf_ProyectoIntegrador
cp equipo.env.example equipo.env
make stack
```

Servicios locales:

- Guacales: `http://localhost:18080/demo/api/v1/salud`
- UTNGolCoin: `http://localhost:5001/api/salud`
- Portal de estadísticas: `http://localhost:5080`
- Portal de apuestas: `http://localhost:5081`
- Administración: `http://localhost:5203`

Comandos:

```bash
make stack-status
make stack-logs
make prueba-integracion
make stack-stop
```

`make prueba-integracion` crea registros reales en las dos bases, apuesta y
publica un resultado para confirmar que Guacales ordena la liquidación en
UTNGolCoin.

## Carga manual

No se carga RF28. Sigue
[`CARGA-MANUAL-OCTAVOS.md`](CARGA-MANUAL-OCTAVOS.md) para registrar sede,
16 selecciones y ocho partidos de octavos por API/panel administrativo.

## Ejecución distribuida en la LAN

Cada responsable ejecuta el comando nativo documentado en su propio repo. En la
máquina de Fernando copia `equipo.env.example` a `equipo.env` y cambia únicamente
las URLs:

```dotenv
URL_GUACALES=http://IP_DE_ANDREA:8080/demo/api/v1/
URL_UTNGOLCOIN=http://IP_DE_MAYRA:5001/api/
URL_ADMIN=http://IP_DE_JOEL:5203
USAR_SIMULADO=false
```

En Joel se configuran las mismas IP mediante variables estándar:

```bash
ApiEstadisticas__BaseUrl=http://IP_DE_ANDREA:8080/demo/api/v1/
ApiUTNGolCoin__BaseUrl=http://IP_DE_MAYRA:5001/api/
```

Andrea configura `UTNGOLCOIN_BASE_URL` con la IP de Mayra. Mayra configura
`ServicioEstadisticas__BaseUrl` con la IP de Andrea.

Obtén la IP de cada Mac con:

```bash
ipconfig getifaddr en0
```

Si usa Ethernet puede ser `en1`. Las aplicaciones escuchan en `0.0.0.0`; las
URLs anunciadas sí deben contener la IP LAN estable de cada máquina.

## Por qué no se usa ngrok

Los cuatro equipos estarán en la misma LAN y pueden comunicarse directamente.
ngrok añade dominios temporales, advertencias/cabeceras especiales, latencia y
dependencia de Internet sin aportar nada al escenario local. `appsettings.json`
solo conserva defaults `localhost`; cada IP se inyecta por entorno o
`equipo.env`, por lo que cambiar de red no requiere editar ni volver a compilar.

## Panel de Joel, no placeholder JSF

El panel real se ejecuta desde `PumaJoel_ProyectoIntegrador`. Copiarlo dentro del
repo de Fernando produciría dos versiones que se desincronizan. La desviación de
JSF se declara honestamente en
[`adr/ADR-001-frontend-administrativo-mvc.md`](adr/ADR-001-frontend-administrativo-mvc.md);
un placeholder JSF no implementaría funcionalidad y tampoco corregiría la
desviación.
