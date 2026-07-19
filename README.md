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
       └──link──► Apuestas (:5081) ──auth/stats──► Guacales
                         │
                         └──billetera/predicciones──► UTNGolCoin :5000/api
```

## Ejecución de los frontends

```bash
# Portal de estadísticas (invitado)
cd frontend-estadisticas-mvc && dotnet run    # http://localhost:5080

# Portal de apuestas (registro obligatorio para predecir)
cd frontend-publico-mvc && dotnet run         # http://localhost:5081
```

Por defecto `UsarSimulado=true` en ambos `appsettings.json` (datos en memoria).
Para APIs reales:

```json
"Servicios": {
  "Estadisticas": { "DireccionBase": "http://localhost:8080/demo/api/v1/", "UsarSimulado": false },
  "UTNGolCoin":   { "DireccionBase": "http://localhost:5000/api/", "UsarSimulado": false }
}
```

### Usuarios de prueba (modo simulado)

| Correo | Contraseña |
|---|---|
| `diego@utn.edu.ec` | `Clave123!` |
| `maria@utn.edu.ec` | `Clave123!` |

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
