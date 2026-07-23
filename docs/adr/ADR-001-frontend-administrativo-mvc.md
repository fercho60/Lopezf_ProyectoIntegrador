# ADR-001: Frontend administrativo en ASP.NET Core MVC (no JSF)

- **Estado:** Aceptado
- **Fecha:** 2026-07-22
- **Decisores:** Equipo UTN GolMundial 2026 (Personas 1–4)

## Contexto

El enunciado exige dos frontends con tecnologías distintas: uno en JSF y otro en C#
(Blazor o ASP.NET Core MVC/Razor). El plan interno sugería JSF + PrimeFaces para el
panel administrativo y ASP.NET Core MVC para el frontend público.

## Decisión y desviación aceptada

El frontend administrativo (Persona 3 — Joel) se implementa en **ASP.NET Core MVC**,
no en JSF. El frontend público (Persona 4 — Fernando) permanece en **ASP.NET Core MVC**
con dos aplicaciones (estadísticas invitado y apuestas).

La solución implementada queda así:

| Componente | Tecnología |
|---|---|
| Backend Estadísticas | Jakarta EE (Java) |
| Backend UTNGolCoin | ASP.NET Core Web API (C#) |
| Frontend administrativo | ASP.NET Core MVC (C#) |
| Frontend público | ASP.NET Core MVC (C#) |

Esta decisión **no cumple** la regla explícita del enunciado que exige un frontend
JSF y otro C#. El equipo la registra como una desviación consciente, no como
equivalencia tecnológica ni como cumplimiento de la restricción.

## Alternativa descartada

JSF + PrimeFaces para el administrativo.

## Justificación

1. El equipo ya domina el stack C#/.NET en el administrativo y en UTNGolCoin; JSF
   habría duplicado curva de aprendizaje (Faces, CDI, WildFly UI) sin aportar un
   requisito de negocio distinto.
2. El administrativo es un panel CRUD/reportes: MVC + Razor cumple el diseño sobrio
   tipo panel de control sin necesidad de componentes JSF.
3. La heterogeneidad sí se conserva entre los **backends** (Jakarta EE y .NET),
   pero no entre los frontends. El equipo prioriza demostrar integración REST real,
   persistencia separada y liquidación completa sobre una migración tardía del panel.
4. Migrar a JSF ahora retrasaría registro de resultados → liquidación (flujo crítico
   de la Semana 4) sin mejorar la experiencia del administrador.

## Consecuencias

- Hay que declarar esta ADR en la defensa y en Modelamiento de Software.
- El docente puede descontar la parte correspondiente a heterogeneidad de frontends;
  esta ADR explica la decisión, pero no elimina ese riesgo.
- No se mantiene una carpeta JSF funcional en el monorepo de Fernando: el admin
  vive en el repositorio de Joel y se integra por HTTP, no por copia de código.
- Si el docente exige JSF de forma inflexible, quedaría como deuda: reimplementar
  el panel o un subconjunto (login + registrar resultado) en JSF.
