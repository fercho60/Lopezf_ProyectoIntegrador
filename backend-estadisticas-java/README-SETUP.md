# Servicio de Estadísticas - UTN GolMundial 2026

Guía para clonar y levantar este backend en tu propia máquina y probar la integración con tu servicio (.NET / UTNGolCoin).

## Requisitos previos

- Java 21 (JDK)
- Maven
- WildFly 39 (descomprimido, no hace falta instalarlo como servicio)
- PostgreSQL instalado y corriendo
- Git

## 1. Clonar el repositorio

```bash
git clone https://github.com/Andrea25102025/GuacalesA_ProyectoIntegrador.git
cd GuacalesA_ProyectoIntegrador/demo
```

## 2. Crear la base de datos

```bash
sudo -i -u postgres psql
```

Dentro de la consola:
```sql
CREATE DATABASE guacales_db;
ALTER USER postgres PASSWORD 'TU_PASSWORD_AQUI';
\q
```

Si tu PostgreSQL usa autenticación `ident` en vez de `md5`/`scram-sha-256`, edita tu `pg_hba.conf` (ruta la da `SHOW hba_file;` dentro de psql) y cambia esas líneas a `md5`, luego:
```bash
sudo systemctl restart postgresql
```

## 3. Configurar el datasource en WildFly

Arranca WildFly:
```bash
/ruta/a/wildfly/bin/standalone.sh
```

En otra terminal, entra a la consola CLI:
```bash
/ruta/a/wildfly/bin/jboss-cli.sh --connect
```

Registra el driver de PostgreSQL (primero copia el `.jar` del driver a `modules/org/postgresql/main/` con su `module.xml` — ver la carpeta `docs/` del repo si se agrega, o pide el detalle):
```
/subsystem=datasources/jdbc-driver=postgresql:add(driver-name=postgresql,driver-module-name=org.postgresql,driver-class-name=org.postgresql.Driver)
```

Crea el datasource (debe llamarse exactamente `GuacalesDS`, el proyecto ya apunta a ese JNDI name):
```
data-source add --name=GuacalesDS --jndi-name=java:/GuacalesDS --driver-name=postgresql --connection-url=jdbc:postgresql://localhost:5432/guacales_db --user-name=postgres --password=TU_PASSWORD_AQUI --enabled=true
```

Prueba la conexión:
```
/subsystem=datasources/data-source=GuacalesDS:test-connection-in-pool
```

Debe responder `"outcome" => "success"`.

## 4. Compilar y desplegar

```bash
cd GuacalesA_ProyectoIntegrador/demo
mvn clean package
cp target/demo.war /ruta/a/wildfly/standalone/deployments/
```

Verifica que se creó `demo.war.deployed` (no `.failed`) en esa misma carpeta.

## 5. Cargar los datos iniciales (seed)

El archivo `seed-guacales.sql` está en la raíz del repo. Cárgalo así:

```bash
sudo -i -u postgres psql -d guacales_db -f /ruta/al/seed-guacales.sql
```

Verifica:
```bash
sudo -i -u postgres psql -d guacales_db -c "SELECT COUNT(*) FROM seleccion;"
```
Debe devolver `48`.

## 6. Probar que todo responde

Con WildFly corriendo:

```bash
curl http://localhost:8080/demo/api/v1/selecciones
curl http://localhost:8080/demo/api/v1/grupos
curl http://localhost:8080/demo/api/v1/sedes
curl http://localhost:8080/demo/api/v1/partidos
```

Documentación interactiva (Swagger):
```
http://localhost:8080/demo/swagger-ui.html
```

## Endpoints disponibles

### Autenticación
- `POST /api/v1/autenticacion/registro` → `{ nombre, correo, contrasena }` → `{ token, usuario }`
- `POST /api/v1/autenticacion/sesion` → `{ correo, contrasena }` → `{ token, usuario }`
- Token JWT (HS256), válido 24h.

### Datos del torneo
- `GET /api/v1/selecciones` — las 48 selecciones
- `GET /api/v1/grupos` — 12 grupos con tabla de posiciones
- `GET /api/v1/sedes` — 16 sedes
- `GET /api/v1/partidos` — calendario (filtros: `?estado=`, `?grupo=`, `?fase=`, `?fecha=`)
- `GET /api/v1/partidos/{id}` — detalle de partido, incluye cuotas
- `PUT /api/v1/partidos/{id}/resultado` → `{ golesLocal, golesVisitante }` — registra resultado y recalcula posiciones
- `GET /api/v1/estadisticas/selecciones` — estadísticas acumuladas por selección

## Notas

- CORS está habilitado (`Access-Control-Allow-Origin: *`), así que tu app puede llamar directo a estos endpoints sin bloqueos del navegador.
- Cuando levantes el proyecto en tu propia máquina, `localhost:8080` te sirve directo — no necesitas la IP de red de nadie más.
- Al **registrar un usuario**, este servicio notifica a UTNGolCoin (`POST http://localhost:5000/api/billeteras` con `{ "usuarioId": N }`) para crear la billetera y el bono de 10 UGC. Si UTNGolCoin no está arriba, el registro en Estadísticas igual se completa (solo se registra un warning en el log).
- Al **registrar un resultado**, notifica `POST /api/liquidaciones/{partidoId}` para liquidar predicciones.
- URL de UTNGolCoin configurable con variable de entorno `UTNGOLCOIN_BASE_URL` o propiedad JVM `-Dutngolcoin.baseUrl=...` (default `http://localhost:5000/api/`).

## Frontends públicos (espejo Persona 4)

Este repositorio incluye una copia de los dos frontends ASP.NET del equipo:

| App | Puerto | Rol |
|---|---|---|
| `frontend-estadisticas-mvc` | 5080 | Solo estadísticas, acceso invitado |
| `frontend-publico-mvc` | 5081 | Estadísticas + apuestas (registro obligatorio) |

```bash
cd frontend-estadisticas-mvc && dotnet run   # http://localhost:5080
cd frontend-publico-mvc && dotnet run       # http://localhost:5081
```

Ambos apuntan por defecto a esta API en `http://localhost:8080/demo/api/v1/`.
Con `Servicios:Estadisticas:UsarSimulado=true` (valor por defecto) usan datos en memoria sin WildFly.
Para desarrollo integrado: `make run` desde la raíz (si hay `Makefile`) o dos terminales.
