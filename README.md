# UTN GolMundial 2026

Plataforma web distribuida para el seguimiento estadístico y predicciones del Mundial de Fútbol 2026 con la moneda virtual académica **UTNGolCoin**.

> Proyecto Integrador — Universidad Técnica del Norte, Facultad de Ingeniería en Ciencias Aplicadas, Carrera de Software.
> Repositorio personal de **Fernando López (Persona 4 — Líder del Frontend Público)**.

## Arquitectura

La solución se compone de cuatro componentes independientes que se comunican exclusivamente mediante API REST con JSON:

| Componente | Tecnología | Base de datos | Estado en este repositorio |
|---|---|---|---|
| Servicio de Estadísticas | Jakarta EE (JAX-RS, JPA) sobre WildFly/Linux | PostgreSQL | Pendiente — ver contrato en `docs/contratos/estadisticas-openapi.yaml` |
| Servicio UTNGolCoin | ASP.NET Core Web API | MySQL | Pendiente — ver contrato en `docs/contratos/utngolcoin-openapi.yaml` |
| Frontend Administrativo | JSF + PrimeFaces | Sin acceso directo | Pendiente (lidera Persona 3) |
| **Frontend Público** | **ASP.NET Core MVC/Razor + Bootstrap 5** | Sin acceso directo | **Implementado (MVP) en `frontend-publico-mvc/`** |

Estrategia **UI primero**: el frontend público consume interfaces de servicio (`IServicioEstadisticas`, `IServicioMonedas`, `IServicioAutenticacion`) con implementaciones simuladas en memoria. Los contratos OpenAPI en `docs/contratos/` definen exactamente lo que los backends deben implementar; al estar listos, el cambio se hace registrando clientes HTTP reales en la inyección de dependencias, sin tocar controladores ni vistas.

## Estructura del repositorio

```
Lopezf_ProyectoIntegrador/
|-- backend-estadisticas-java/      Servicio de Estadísticas (pendiente)
|-- backend-utngolcoin-dotnet/      Servicio UTNGolCoin (pendiente)
|-- frontend-administrativo-jsf/    Frontend administrativo (pendiente)
|-- frontend-publico-mvc/           Frontend público MVC/Razor (este MVP)
|-- database/
|   |-- postgresql/                 Scripts del Servicio de Estadísticas (pendiente)
|   `-- mysql/                      Scripts del Servicio UTNGolCoin (pendiente)
|-- docs/
|   |-- contratos/                  Contratos OpenAPI para los backends
|   |-- plan/                       PLAN.md y STATUS.md de la feature
|   |-- uml/  adr/  requisitos/  historias-usuario/  pruebas/  manuales/
|-- evidencias/                     Capturas y evidencias de funcionamiento
`-- README.md
```

## Requisitos de instalación

- SDK de .NET 9 o superior
- Navegador web moderno

## Ejecución del frontend público

```bash
cd frontend-publico-mvc
dotnet run
```

La aplicación queda disponible en `http://localhost:5080`.

### Usuarios de prueba

| Correo | Contraseña | Rol |
|---|---|---|
| `diego@utn.edu.ec` | `Clave123!` | usuario |
| `maria@utn.edu.ec` | `Clave123!` | usuario |

También puede registrarse un usuario nuevo: recibe automáticamente el bono de bienvenida de 10 UTNGolCoin. El acceso como invitado (sin sesión) permite consultar calendario, posiciones, estadísticas y ranking.

## Direcciones base de las APIs (objetivo)

| Servicio | Dirección base | Contrato |
|---|---|---|
| Estadísticas | `http://localhost:8080/api/v1` | `docs/contratos/estadisticas-openapi.yaml` |
| UTNGolCoin | `http://localhost:5200/api/v1` | `docs/contratos/utngolcoin-openapi.yaml` |

## Reglas de UTNGolCoin

- Bono de bienvenida de **10 UTNGolCoin** al registrarse (transacción del ledger).
- Predicciones **1X2** (gana local, empate, gana visitante), una única por usuario y partido.
- Cierre automático de predicciones a la hora de inicio del partido.
- Liquidación automática de premios: monto apostado por la cuota del resultado.
- Bono anti-bancarrota de **1 UTNGolCoin** diaria al iniciar sesión con saldo cero.
- Moneda exclusivamente ficticia y académica, sin valor monetario real.

## Integrantes y responsabilidades

| Integrante | Liderazgo principal |
|---|---|
| Persona 1 | Servicio de Estadísticas (Jakarta EE + PostgreSQL) |
| Persona 2 | Servicio UTNGolCoin (ASP.NET Core + MySQL) |
| Persona 3 | Frontend administrativo (JSF + PrimeFaces) |
| **Persona 4 — Fernando López** | **Frontend público (ASP.NET Core MVC/Razor)** |

## Planificación

El plan de implementación y el estado de avance viven en [`docs/plan/PLAN.md`](docs/plan/PLAN.md) y [`docs/plan/STATUS.md`](docs/plan/STATUS.md).
