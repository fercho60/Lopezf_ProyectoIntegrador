# API UTNGolCoin — notas para el monorepo (Persona 2)

Base local:

```
http://localhost:5000/api
```

El portal de apuestas (`frontend-publico-mvc`) consume billeteras, predicciones, ranking y bono diario.
Health check:

```
GET /api/salud  →  { "estado": "OK", "servicio": "utngolcoin" }
```

Guacales debe poder alcanzar esta URL al registrar usuarios y resultados.
Configurar MySQL (`utngolcoin_db`) antes de `dotnet run`.
