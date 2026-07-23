# UTN GolMundial 2026 — frontends públicos
# Uso: make help | make run | make urls | make stop
#
# IPs y backends: editar equipo.env (copiar desde equipo.env.example).
# Local por defecto. Mañana en LAN: solo cambia URL_* y BIND_* en equipo.env.

ROOT := $(abspath $(dir $(lastword $(MAKEFILE_LIST))))
ESTADISTICAS := $(ROOT)/frontend-estadisticas-mvc
APUESTAS := $(ROOT)/frontend-publico-mvc

# Carga overrides locales (no versionados). Si no existe, usan defaults de abajo.
-include $(ROOT)/equipo.env

# Defaults = todo en esta máquina (probar en conjunto local)
URL_GUACALES ?= http://localhost:18080/demo/api/v1/
URL_UTNGOLCOIN ?= http://localhost:5001/api/
USAR_SIMULADO ?= false
BIND_EST ?= localhost:5080
BIND_APU ?= localhost:5081

# host o host:puerto
HOST_EST := $(shell echo "$(BIND_EST)" | sed 's/:.*//')
HOST_APU := $(shell echo "$(BIND_APU)" | sed 's/:.*//')
PORT_EST := $(shell echo "$(BIND_EST)" | awk -F: '{print (NF > 1 ? $$NF : "5080")}')
PORT_APU := $(shell echo "$(BIND_APU)" | awk -F: '{print (NF > 1 ? $$NF : "5081")}')

LISTEN ?= 0.0.0.0

URL_EST := http://$(HOST_EST):$(PORT_EST)
URL_APU := http://$(HOST_APU):$(PORT_APU)
LISTEN_EST := http://$(LISTEN):$(PORT_EST)
LISTEN_APU := http://$(LISTEN):$(PORT_APU)

.PHONY: help run estadisticas apuestas stop build clean status urls stack stack-stop stack-status stack-logs prueba-integracion

help: ## Muestra los targets disponibles
	@echo "Frontends UTN GolMundial 2026"
	@echo ""
	@echo "  make run            Levanta estadísticas + apuestas"
	@echo "  make urls           Muestra IPs/URLs activas (desde equipo.env)"
	@echo "  make estadisticas   Solo portal invitado → $(URL_EST)"
	@echo "  make apuestas       Solo portal apuestas → $(URL_APU)"
	@echo "  make stop           Libera $(PORT_EST) y $(PORT_APU)"
	@echo "  make build | status | clean"
	@echo ""
	@echo "Pila integrada nativa (sin Docker):"
	@echo "  make stack          Levanta bases, APIs y frontends en background"
	@echo "  make stack-status   Muestra procesos y salud de toda la pila"
	@echo "  make stack-logs     Sigue los logs (Ctrl+C para salir)"
	@echo "  make stack-stop     Detiene únicamente los procesos de la pila"
	@echo "  make prueba-integracion  Prueba persistencia y liquidación reales"
	@echo ""
	@echo "Config:  cp equipo.env.example equipo.env   # luego edita IPs"
	@echo "Ahora:   Guacales=$(URL_GUACALES)"
	@echo "         UTNGolCoin=$(URL_UTNGOLCOIN)"
	@echo "         Simulado=$(USAR_SIMULADO)  FE=$(URL_EST) | $(URL_APU)"

urls: ## Imprime la configuración efectiva
	@echo "=== equipo.env / defaults ==="
	@echo "URL_GUACALES   = $(URL_GUACALES)"
	@echo "URL_UTNGOLCOIN = $(URL_UTNGOLCOIN)"
	@echo "USAR_SIMULADO  = $(USAR_SIMULADO)"
	@echo "BIND_EST       = $(BIND_EST)  → $(URL_EST)  (listen $(LISTEN_EST))"
	@echo "BIND_APU       = $(BIND_APU)  → $(URL_APU)  (listen $(LISTEN_APU))"
	@echo ""
	@echo "Mañana en LAN: edita solo equipo.env y reinicia (make stop && make run)."

run: ## Ambos frontends a la vez
	@echo "→ Estadísticas $(URL_EST)  |  Apuestas $(URL_APU)"
	@echo "  Guacales=$(URL_GUACALES)  UTNGolCoin=$(URL_UTNGOLCOIN)  simulado=$(USAR_SIMULADO)"
	@echo "  (Ctrl+C para detener ambos)"
	@$(MAKE) -j2 estadisticas apuestas

estadisticas: ## Portal invitado (solo consulta)
	@echo "→ $(URL_EST)  ← Guacales $(URL_GUACALES)"
	cd "$(ESTADISTICAS)" && \
		Servicios__Estadisticas__DireccionBase="$(URL_GUACALES)" \
		Servicios__Estadisticas__UsarSimulado="$(USAR_SIMULADO)" \
		Frontends__ApuestasUrl="$(URL_APU)" \
		dotnet run --no-launch-profile --urls "$(LISTEN_EST)"

apuestas: ## Portal de apuestas (registro para predecir)
	@echo "→ $(URL_APU)  ← Guacales $(URL_GUACALES) | UTNGolCoin $(URL_UTNGOLCOIN)"
	cd "$(APUESTAS)" && \
		Servicios__Estadisticas__DireccionBase="$(URL_GUACALES)" \
		Servicios__Estadisticas__UsarSimulado="$(USAR_SIMULADO)" \
		Servicios__UTNGolCoin__DireccionBase="$(URL_UTNGOLCOIN)" \
		Servicios__UTNGolCoin__UsarSimulado="$(USAR_SIMULADO)" \
		Frontends__EstadisticasUrl="$(URL_EST)" \
		dotnet run --no-launch-profile --urls "$(LISTEN_APU)"

stop: ## Libera los puertos configurados
	@-lsof -tiTCP:$(PORT_EST) -sTCP:LISTEN 2>/dev/null | xargs kill 2>/dev/null || true
	@-lsof -tiTCP:$(PORT_APU) -sTCP:LISTEN 2>/dev/null | xargs kill 2>/dev/null || true
	@echo "Puertos $(PORT_EST) y $(PORT_APU) liberados (si había procesos)."

status: ## Quién escucha en los puertos
	@echo "=== :$(PORT_EST) (estadísticas) ==="
	@-lsof -nP -iTCP:$(PORT_EST) -sTCP:LISTEN 2>/dev/null || echo "(libre)"
	@echo "=== :$(PORT_APU) (apuestas) ==="
	@-lsof -nP -iTCP:$(PORT_APU) -sTCP:LISTEN 2>/dev/null || echo "(libre)"

build: ## Compilar ambos
	cd "$(ESTADISTICAS)" && dotnet build
	cd "$(APUESTAS)" && dotnet build

clean: ## Limpiar artefactos de build
	cd "$(ESTADISTICAS)" && dotnet clean
	cd "$(APUESTAS)" && dotnet clean
	@rm -rf "$(ESTADISTICAS)/bin" "$(ESTADISTICAS)/obj" "$(APUESTAS)/bin" "$(APUESTAS)/obj"
	@echo "Limpieza lista."

stack: ## Pila completa nativa en background
	@"$(ROOT)/scripts/stack.sh"

stack-stop: ## Detiene la pila completa
	@"$(ROOT)/scripts/stack-stop.sh"

stack-status: ## Estado y salud de la pila completa
	@"$(ROOT)/scripts/stack-status.sh"

stack-logs: ## Sigue los logs de la pila completa
	@"$(ROOT)/scripts/stack-logs.sh"

prueba-integracion: ## Verifica sincronización real Guacales ↔ UTNGolCoin
	@"$(ROOT)/scripts/prueba-integracion-real.sh"
