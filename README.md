# UTN GolMundial 2026

Plataforma web distribuida para el seguimiento estadístico y predicciones del Mundial de Fútbol 2026 con la moneda virtual académica **UTNGolCoin**.

> Proyecto Integrador — Universidad Técnica del Norte.
> Repositorio de **Fernando López (Persona 4 — Frontend Público)**, con copias de integración de los backends del equipo.

## Arquitectura

| Componente | Tecnología | Origen | Carpeta |
|---|---|---|---|
| Servicio de Estadísticas | Jakarta EE + PostgreSQL | [GuacalesA](https://github.com/Andrea25102025/GuacalesA_ProyectoIntegrador.git) | repo hermano `../GuacalesA_ProyectoIntegrador/` |
| Servicio UTNGolCoin | ASP.NET Core + MySQL | [SantacruzMayra](https://github.com/Rocio7514/SantacruzMayra_Proyecto_Integrador.git) | repo hermano `../SantacruzMayra_Proyecto_Integrador/` |
| Frontend Administrativo | ASP.NET Core MVC | [PumaJoel](https://github.com/JoelPuma2004/PumaJoel_ProyectoIntegrador.git) | repo hermano `../PumaJoel_ProyectoIntegrador/` |
| **Frontend estadísticas (invitado)** | ASP.NET Core MVC | Persona 4 | **`frontend-estadisticas-mvc/`** → `:5080` |
| **Frontend apuestas** | ASP.NET Core MVC | Persona 4 | **`frontend-publico-mvc/`** → `:5081` |

```
Invitado (:5080) ──consulta──► Guacales :18080/demo/api/v1
       │
       └──link (nueva pestaña)──► Apuestas (:5081) ──auth/stats──► Guacales
                                         │
                                         └──billetera/predicciones──► UTNGolCoin :5001/api
```

## Levantar todo local, real y sin Docker

> Guía completa: [`docs/GUIA-EJECUCION.md`](docs/GUIA-EJECUCION.md)
> Carga manual: [`docs/CARGA-MANUAL-OCTAVOS.md`](docs/CARGA-MANUAL-OCTAVOS.md)

Los cuatro repositorios deben estar clonados como carpetas hermanas. La primera
ejecución descarga WildFly en `.stack/`; PostgreSQL, MySQL, Java 21 y Maven son
procesos nativos instalados con Homebrew.

```bash
cp equipo.env.example equipo.env   # una vez
make stack                         # bases + APIs + admin + portales
make prueba-integracion            # predicción y liquidación reales
make stack-status
make stack-stop
```

| Portal | URL local |
|---|---|
| Estadísticas (invitado) | `http://localhost:5080` |
| Apuestas | `http://localhost:5081` |
| Administración | `http://localhost:5203` |

Mañana en LAN: solo cambias `URL_GUACALES`, `URL_UTNGOLCOIN` (y si aplica `BIND_*`)
en `equipo.env`.
### Por qué no funciona `cd A && dotnet run` seguido de `cd B && ...`

`dotnet run` **bloquea la terminal**. Si escribes las dos líneas en la misma sesión, la segunda nunca se ejecuta (o, si ya estabas dentro de una carpeta, el segundo `cd` falla y vuelves a levantar la misma app → “address already in use”).

Soluciones correctas:

1. **`make run`** (recomendado): levanta las dos apps en paralelo.
2. **Dos terminales**: en una `make estadisticas`, en la otra `make apuestas`.
3. Si un puerto quedó colgado: `make stop` y vuelve a `make run`.

Requisito: [Make](https://www.gnu.org/software/make/) (viene en macOS/Linux) y SDK de .NET 9+.

## APIs reales

`make stack` fuerza `UsarSimulado=false`: toda alta, predicción, transacción y
liquidación se persiste en PostgreSQL/MySQL. El código simulado antiguo no forma
parte del flujo integrado.

Guacales, al registrar usuario o resultado, llama a UTNGolCoin (`POST /api/billeteras` y `POST /api/liquidaciones/{id}`).

## Split del frontend público

El frontend público se dividió en **dos aplicaciones ASP.NET** independientes:

| App | Puerto | Acceso | Contiene |
|---|---|---|---|
| `frontend-estadisticas-mvc` | 5080 | Invitado (sin login) | Inicio, partidos, posiciones, estadísticas |
| `frontend-publico-mvc` | 5081 | Registro obligatorio para apostar | Lo anterior + cuenta, billetera, predicciones, ranking |

- En estadísticas, el CTA **“Ir a apuestas y registrarme”** abre el portal `:5081` en **nueva pestaña**.
- En apuestas, hay enlace de vuelta al portal de estadísticas (invitado).
- Ambas consumen Guacales/UTNGolCoin por HTTP durante la ejecución integrada.

Detalle del diseño: [`docs/plan/PLAN.md`](docs/plan/PLAN.md) y el plan de integración en Cursor.

## Repositorios de los dueños

- Estadísticas: `../GuacalesA_ProyectoIntegrador/` (Andrea).
- UTNGolCoin: `../SantacruzMayra_Proyecto_Integrador/` (Mayra).
- Administración: `../PumaJoel_ProyectoIntegrador/` (Joel).

Las carpetas de backend que aún existen aquí son copias históricas y no son las
que modifica ni levanta `make stack`.

## Reglas de UTNGolCoin

- Bono de bienvenida de **10 UTNGolCoin** al registrarse.
- Predicciones **1X2**, una por usuario y partido; cierran al inicio del partido.
- Bono anti-bancarrota de **1 UTNGolCoin** diaria con saldo cero.
- Moneda ficticia académica, sin valor monetario real.

## Planificación

[`docs/plan/PLAN.md`](docs/plan/PLAN.md) y [`docs/plan/STATUS.md`](docs/plan/STATUS.md).

Documento para la presentación final (frontends, tecnologías, BD, modo claro/oscuro):
[`docs/PRESENTACION-FINAL-FRONTENDS.md`](docs/PRESENTACION-FINAL-FRONTENDS.md).
