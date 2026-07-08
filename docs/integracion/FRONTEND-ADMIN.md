# Frontend Administrativo — notas (Persona 3)

El panel administrativo vive en el repositorio de Joel (`PumaJoel_ProyectoIntegrador`).
Stack real: **ASP.NET Core MVC (.NET 10)**, no JSF.

Consume principalmente:

```
http://localhost:8080/demo/api/v1
```

(selecciones, partidos, registro de resultados, autenticación de administrador).

En local, `ApiEstadisticas:BaseUrl` debe apuntar a WildFly y no a túneles temporales.
