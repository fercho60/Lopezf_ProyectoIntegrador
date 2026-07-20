# Backlog de Implementación: UTN GolMundial 2026 — Frontend Público (Persona 4)

> **Versión**: 1.0 (MVP implementado)
> **Creado**: 2026-07-13
> **Estado**: MVP entregado — fases 1 a 6 implementadas, fase 7 parcial
> **Alcance**: Frontend público ASP.NET Core MVC/Razor con datos simulados y contratos OpenAPI para los backends.

---

## Contexto

UTN GolMundial 2026 es una plataforma distribuida de 4 componentes (2 backends REST y 2 frontends web). Este plan cubre el ámbito de la **Persona 4 — Líder del Frontend Público**: la aplicación ASP.NET Core MVC/Razor con temática mundialista que consumen los usuarios finales e invitados.

Estrategia **UI primero**: los backends aún no existen, por lo que el frontend consume tres interfaces de servicio con implementaciones simuladas en memoria. Los contratos OpenAPI en `docs/contratos/` son el entregable para que las Personas 1 y 2 implementen los backends; el cambio a clientes HTTP reales se hace únicamente en el registro de dependencias de `frontend-publico-mvc/Program.cs`.

**En alcance:**
- Reestructuración del repo a la organización recomendada (4 componentes + database/ + docs/).
- Frontend público MVC completo en español: consulta (invitado) + cuenta + billetera + predicciones.
- Contratos OpenAPI de los endpoints públicos de ambos servicios.

**Fuera de alcance:**
- Implementación de los backends y del frontend administrativo JSF (solo placeholders).
- Endpoints administrativos (gestión de usuarios, auditoría, reportes, notificación entre backends).
- Cuotas dinámicas, apuestas combinadas, dinero real, apps móviles.

**Documentos fuente:**
- `Proyecto Integrador UTN GolMundial 2026` (documento oficial del docente).
- `Plan de Organización y Distribución del Trabajo` (guía interna del equipo).
- Prototipo de alta fidelidad (React/Tailwind) usado como referencia visual.

---

## Decisiones bloqueadas

- **Stack**: ASP.NET Core MVC/Razor sobre .NET 9 (`RollForward=LatestMajor`), Bootstrap 5 + CSS propio (`wwwroot/css/tema.css`) replicando el prototipo, con tema adaptativo claro/oscuro.
- **Idioma**: código, carpetas, vistas, mensajes y commits en español. Excepciones técnicas: sufijo `Controller`, archivos reservados de Razor (`_ViewStart`, `_ViewImports`) y `Properties/launchSettings.json`.
- **Vistas en `/Vistas`**: configurado vía `RazorViewEngineOptions.ViewLocationFormats` en `Program.cs`.
- **Datos simulados**: `AlmacenDatosSimulados` (singleton) + `ServicioEstadisticasSimulado`, `ServicioAutenticacionSimulado`, `ServicioMonedasSimulado`, registrados por inyección de dependencias.
- **Autenticación**: cookies de ASP.NET Core (`UTNGolMundial.Sesion`); el contrato define `POST /api/v1/autenticacion/registro` y `/sesion` que devuelven JWT + usuario.
- **Contratos**: `docs/contratos/estadisticas-openapi.yaml` y `docs/contratos/utngolcoin-openapi.yaml`, solo endpoints del frontend público.

---

## Límites de propiedad por servicio

- **Servicio de Estadísticas** (Persona 1): usuarios/autenticación, selecciones, grupos, sedes, partidos, resultados, posiciones, estadísticas, auditoría. Notifica a UTNGolCoin al registrar un resultado (RF12).
- **Servicio UTNGolCoin** (Persona 2): billeteras, ledger de transacciones, predicciones, liquidación, bono diario, ranking.
- **Frontend público** (Persona 4, este repo): experiencia del usuario final; nunca accede a bases de datos; consume solo los contratos.

---

## Fases

### Fase 1 — Reestructuración del repo y esqueleto MVC — HECHA
- Eliminada la plantilla Web API; solución `UTNGolMundial2026.slnx` apunta a `frontend-publico-mvc/FrontendPublico.csproj`.
- Carpetas placeholder de los otros 3 componentes y `database/` con README.
- Layout `Vistas/Compartido/_Diseno.cshtml` con navegación, alertas, footer y conmutador de tema (`wwwroot/js/tema.js`).

### Fase 2 — Contratos OpenAPI, modelos y servicios simulados — HECHA
- 2 contratos OpenAPI 3.0 en `docs/contratos/`.
- Modelos en `Modelos/` (Seleccion, Grupo, Partido, Prediccion, Transaccion, EntradaRanking, etc.).
- Interfaces en `Servicios/` + implementaciones en `Servicios/Simulados/` con semilla de 4 grupos, 16 selecciones, 14 partidos y 6 usuarios demo (subconjunto: el seed oficial de 48/12/104 lo provee el docente, RF28).

### Fase 3 — Vistas públicas de consulta (RF04–RF09, RF26) — HECHA
- `InicioController`, `CalendarioController` (índice con filtros por estado/grupo + detalle con cuotas), `PosicionesController`, `EstadisticasController` (tabla + barras de goles), `RankingController`. Acceso de invitado sin registro.

### Fase 4 — Autenticación y sesión (RF01–RF03, RF25) — HECHA
- `CuentaController`: registro (bono de bienvenida de 10 UGC como transacción), inicio y cierre de sesión con cookies, validación cliente/servidor, contraseñas solo como hash.

### Fase 5 — Billetera, predicciones y bono diario (RF13–RF20, RF22) — HECHA
- `BilleteraController`: saldo + historial del ledger.
- `PrediccionesController`: crear 1X2 con validaciones (saldo, única por partido, cierre por hora de inicio) y listado con estados.
- Bono diario de 1 UGC al iniciar sesión con saldo cero.

### Fase 6 — Degradación controlada y errores (RNF01, RNF05, RNF10) — HECHA
- Manejo por servicio en Inicio (si un servicio cae, el resto de la página funciona).
- `ErroresController` + `UseStatusCodePagesWithReExecute("/Errores/Codigo/{0}")` para 404/errores.

### Fase 7 — Documentación (entregables Persona 4) — PENDIENTE (parcial)
- Hecho: README raíz, contratos, PLAN.md, STATUS.md.
- Pendiente: `docs/uml/diagrama-despliegue`, `docs/historias-usuario/backlog.md`, `docs/requisitos/registro-riesgos.md`, `docs/manuales/manual-usuario.md`, evidencias de ejecución.

---

## Matriz de dependencias

| Dependencia | Productor | Consumidor | Requerida antes de |
|---|---|---|---|
| Contratos OpenAPI | Este repo (Fase 2) | Backends (Personas 1 y 2) | Implementación de los backends |
| Backend Estadísticas real | Persona 1 | Swap de `ServicioEstadisticasSimulado` por cliente HTTP | Integración real |
| Backend UTNGolCoin real | Persona 2 | Swap de `ServicioMonedasSimulado` por cliente HTTP | Integración real |
| JWT del Servicio de Estadísticas | Persona 1 | Llamadas autenticadas a UTNGolCoin | Integración real |

---

## Ejecución local

```bash
cd frontend-publico-mvc
dotnet run          # http://localhost:5080
```

Usuarios de prueba: `diego@utn.edu.ec` / `Clave123!` (saldo alto), `pedro@utn.edu.ec` / `Clave123!` (saldo 0, dispara el bono diario).

Verificación manual realizada (2026-07-13): rutas públicas 200, rutas protegidas redirigen a login, registro acredita 10 UGC, predicción debita saldo, se rechazan predicción duplicada / saldo insuficiente / partido iniciado, bono diario acreditado con saldo 0, 404 personalizado.

---

## Trabajo futuro (siguientes historias)

1. Completar la documentación de la Fase 7.
2. Clientes HTTP reales (`Servicios/Http/`) implementando las mismas interfaces + resiliencia (timeouts, reintentos) cuando los backends existan.
3. Página de fases eliminatorias (RF09 ampliado) y estado de servicios.
4. Pruebas automatizadas (unitarias de servicios simulados e integración con `WebApplicationFactory`).

---

## Split en dos frontends (hecho — 2026-07)

El frontend público se separó en dos aplicaciones:

| App | Puerto | Quién entra | Qué hace |
|---|---|---|---|
| `frontend-estadisticas-mvc` | 5080 | Invitado | Solo consulta (partidos, posiciones, estadísticas). CTA a apuestas en nueva pestaña. |
| `frontend-publico-mvc` | 5081 | Usuario registrado para apostar | Consulta + cuenta + billetera + predicciones + ranking. |

**Cómo se hizo:** se clonó el MVC a `frontend-estadisticas-mvc`, se quitaron auth/apuestas, se movió el portal completo a `:5081`, se cablearon clientes HTTP a Guacales (`/demo/api/v1`) y UTNGolCoin (`/api`), y se dejó `UsarSimulado` como default.

**Cómo levantarlo:** desde la raíz del repo usar el `Makefile`:

```bash
make run    # ambos
make stop   # liberar 5080/5081
make help   # resto de targets
```

No encadenar `cd frontend-estadisticas-mvc && dotnet run` y luego `cd frontend-publico-mvc && ...` en la misma terminal: el primer `dotnet run` bloquea.