# Servicio UTNGolCoin

Copia de integración del backend de **Persona 2** ([SantacruzMayra_Proyecto_Integrador](https://github.com/Rocio7514/SantacruzMayra_Proyecto_Integrador.git)).

- Tecnología: ASP.NET Core Web API + EF Core + MySQL
- Dirección base local: `http://localhost:5000/api/`
- Consulta partidos en Guacales: `http://localhost:8080/demo/api/v1/`

## Configuración

Edita `appsettings.json`:

```json
"ConnectionStrings": {
  "Default": "server=localhost;port=3306;database=utngolcoin_db;user=root;password=TU_PASSWORD"
},
"ServicioEstadisticas": {
  "BaseUrl": "http://localhost:8080/demo/api/v1/"
}
```

## Ejecución

```bash
cd backend-utngolcoin-dotnet
dotnet restore
dotnet ef database update   # si aplica
dotnet run --urls http://localhost:5000
```

Swagger: `http://localhost:5000/swagger`

El frontend de apuestas (`frontend-publico-mvc`) consume esta API cuando `Servicios:UTNGolCoin:UsarSimulado` es `false`.
