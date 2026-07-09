# Servicio de Estadísticas

Copia de integración del backend de **Persona 1** ([GuacalesA_ProyectoIntegrador](https://github.com/Andrea25102025/GuacalesA_ProyectoIntegrador.git)).

- Tecnología: Jakarta EE (JAX-RS, JPA) sobre WildFly + PostgreSQL
- Código fuente: carpeta [`demo/`](demo/)
- Guía de instalación: [`README-SETUP.md`](README-SETUP.md)

## Dirección base (local)

```
http://localhost:8080/demo/api/v1
```

Los frontends públicos de este monorepo (`frontend-estadisticas-mvc` y `frontend-publico-mvc`) consumen esa URL para calendario, posiciones, estadísticas y autenticación.

## Endpoints principales

- `POST /autenticacion/registro` y `POST /autenticacion/sesion`
- `GET /selecciones`, `/grupos`, `/sedes`, `/partidos`, `/partidos/{id}`
- `PUT /partidos/{id}/resultado`
- `GET /estadisticas/selecciones`

Swagger: `http://localhost:8080/demo/swagger-ui.html`
