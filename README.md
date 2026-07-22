# UTN GolMundial 2026

Plataforma web distribuida para el seguimiento estadístico y predicciones del Mundial de Fútbol 2026 con la moneda virtual académica **UTNGolCoin**.

> Proyecto Integrador — Universidad Técnica del Norte.
> Repositorio de **Fernando López (Persona 4 — Frontend Público)**, con copias de integración de los backends del equipo.

## Arquitectura

| Componente | Tecnología | Origen | Carpeta |
|---|---|---|---|
| Servicio de Estadísticas | Jakarta EE + PostgreSQL | [GuacalesA](https://github.com/Andrea25102025/GuacalesA_ProyectoIntegrador.git) | `backend-estadisticas-java/` |
| Servicio UTNGolCoin | ASP.NET Core + MySQL | [SantacruzMayra](https://github.com/Rocio7514/SantacruzMayra_Proyecto_Integrador.git) | `backend-utngolcoin-dotnet/` |
| Frontend Administrativo | ASP.NET Core MVC | [PumaJoel](https://github.com/JoelPuma2004/PumaJoel_ProyectoIntegrador.git) | referencia externa (placeholder `frontend-administrativo-jsf/`) |
| **Frontend estadísticas (invitado)** | ASP.NET Core MVC | Persona 4 | **`frontend-estadisticas-mvc/`** → `:5080` |
| **Frontend apuestas** | ASP.NET Core MVC | Persona 4 | **`frontend-publico-mvc/`** → `:5081` |

```
Invitado (:5080) ──consulta──► Guacales :8080/demo/api/v1
       │
       └──link (nueva pestaña)──► Apuestas (:5081) ──auth/stats──► Guacales
                                         │
                                         └──billetera/predicciones──► UTNGolCoin :5000/api
```

## Cómo correr los frontends (Makefile)

> Guía completa paso a paso para todo el equipo (incluye pruebas entre máquinas):
> [`docs/GUIA-EJECUCION.md`](docs/GUIA-EJECUCION.md)

Por defecto el `Makefile` levanta en **localhost** (una sola PC):

| Portal | URL |
|---|---|
| Estadísticas | `http://localhost:5080` |
| Apuestas | `http://localhost:5081` |

```bash
make run
# En red local (misma máquina): usa TU IP en ambos
make run BIND_EST=172.28.114.135 BIND_APU=172.28.114.135
```

Equivalente manual:

```bash
dotnet run --project frontend-estadisticas-mvc --urls "http://localhost:5080"
dotnet run --project frontend-publico-mvc --urls "http://localhost:5081"
```

Desde la **raíz del repositorio** (donde está el `Makefile`):

```bash
make help          # lista de comandos
make run           # ambos a la vez: :5080 + :5081
make estadisticas  # solo invitado → http://localhost:5080
make apuestas      # solo apuestas → http://localhost:5081
make stop          # mata lo que use 5080 y 5081
make status        # ver si los puertos están ocupados
make build         # compilar ambos
```

### Por qué no funciona `cd A && dotnet run` seguido de `cd B && ...`

`dotnet run` **bloquea la terminal**. Si escribes las dos líneas en la misma sesión, la segunda nunca se ejecuta (o, si ya estabas dentro de una carpeta, el segundo `cd` falla y vuelves a levantar la misma app → “address already in use”).

Soluciones correctas:

1. **`make run`** (recomendado): levanta las dos apps en paralelo.
2. **Dos terminales**: en una `make estadisticas`, en la otra `make apuestas`.
3. Si un puerto quedó colgado: `make stop` y vuelve a `make run`.

Requisito: [Make](https://www.gnu.org/software/make/) (viene en macOS/Linux) y SDK de .NET 9+.

## Modo simulado vs APIs reales

Por defecto `UsarSimulado=true` (los frontends corren solos con `make run`).
Para acoplar con backends reales: WildFly Guacales en el puerto 8080 + UTNGolCoin en el puerto 5000, y pon `UsarSimulado: false` en ambos `appsettings.json`.

Guacales, al registrar usuario o resultado, llama a UTNGolCoin (`POST /api/billeteras` y `POST /api/liquidaciones/{id}`).

### Usuarios de prueba (modo simulado, portal de apuestas)

| Correo | Contraseña |
|---|---|
| `diego@utn.edu.ec` | `Clave123!` |
| `maria@utn.edu.ec` | `Clave123!` |

## Split del frontend público

El frontend público se dividió en **dos aplicaciones ASP.NET** independientes:

| App | Puerto | Acceso | Contiene |
|---|---|---|---|
| `frontend-estadisticas-mvc` | 5080 | Invitado (sin login) | Inicio, partidos, posiciones, estadísticas |
| `frontend-publico-mvc` | 5081 | Registro obligatorio para apostar | Lo anterior + cuenta, billetera, predicciones, ranking |

- En estadísticas, el CTA **“Ir a apuestas y registrarme”** abre el portal `:5081` en **nueva pestaña**.
- En apuestas, hay enlace de vuelta al portal de estadísticas (invitado).
- Ambas pueden consumir Guacales/UTNGolCoin por HTTP o usar simulados (`UsarSimulado`).

Detalle del diseño: [`docs/plan/PLAN.md`](docs/plan/PLAN.md) y el plan de integración en Cursor.

## Backends

- Estadísticas: ver `backend-estadisticas-java/README-SETUP.md` (WildFly + PostgreSQL).
- UTNGolCoin: ver `backend-utngolcoin-dotnet/README.md` (MySQL, puerto 5000).

## Reglas de UTNGolCoin

- Bono de bienvenida de **10 UTNGolCoin** al registrarse.
- Predicciones **1X2**, una por usuario y partido; cierran al inicio del partido.
- Bono anti-bancarrota de **1 UTNGolCoin** diaria con saldo cero.
- Moneda ficticia académica, sin valor monetario real.

## Planificación

[`docs/plan/PLAN.md`](docs/plan/PLAN.md) y [`docs/plan/STATUS.md`](docs/plan/STATUS.md).

Documento para la presentación final (frontends, tecnologías, BD, modo claro/oscuro):
[`docs/PRESENTACION-FINAL-FRONTENDS.md`](docs/PRESENTACION-FINAL-FRONTENDS.md).
