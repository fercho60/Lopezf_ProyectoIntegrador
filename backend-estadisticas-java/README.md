# Servicio de Estadísticas (pendiente)

Servicio backend API REST en **Jakarta EE (JAX-RS, JPA/Hibernate) sobre WildFly**, con base de datos **PostgreSQL** y despliegue obligatorio sobre **Linux**. Lidera la Persona 1.

Responsabilidades: autenticación de usuarios, selecciones, grupos, sedes, calendario de partidos, resultados, tablas de posiciones y estadísticas por selección.

## Contrato a implementar

El frontend público ya consume este contrato mediante servicios simulados:

- [`docs/contratos/estadisticas-openapi.yaml`](../docs/contratos/estadisticas-openapi.yaml)

Al registrarse un resultado oficial, este servicio debe notificar al Servicio UTNGolCoin para la liquidación de predicciones (RF12).
