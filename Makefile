# UTN GolMundial 2026 — frontends públicos
# Uso: make help | make run | make estadisticas | make apuestas | make stop
#
# Por defecto (red del equipo):
#   estadísticas → 172.28.114.135:5080
#   apuestas     → 172.28.114.136:5081
# Solo local:
#   make run BIND_EST=localhost:5080 BIND_APU=localhost:5081
# Misma máquina en LAN:
#   make run BIND_EST=172.28.114.135:5080 BIND_APU=172.28.114.135:5081

ROOT := $(abspath $(dir $(lastword $(MAKEFILE_LIST))))
ESTADISTICAS := $(ROOT)/frontend-estadisticas-mvc
APUESTAS := $(ROOT)/frontend-publico-mvc

# host o host:puerto — defaults de red del equipo
BIND_EST ?= 172.28.114.135:5080
BIND_APU ?= 172.28.114.136:5081

# Parsea host y puerto desde BIND_* (acepta "host" o "host:puerto")
HOST_EST := $(shell echo "$(BIND_EST)" | sed 's/:.*//')
HOST_APU := $(shell echo "$(BIND_APU)" | sed 's/:.*//')
PORT_EST := $(shell echo "$(BIND_EST)" | awk -F: '{print (NF > 1 ? $$NF : "5080")}')
PORT_APU := $(shell echo "$(BIND_APU)" | awk -F: '{print (NF > 1 ? $$NF : "5081")}')

# Escucha en todas las interfaces (accesible por LAN y por localhost).
LISTEN ?= 0.0.0.0

URL_EST := http://$(HOST_EST):$(PORT_EST)
URL_APU := http://$(HOST_APU):$(PORT_APU)
LISTEN_EST := http://$(LISTEN):$(PORT_EST)
LISTEN_APU := http://$(LISTEN):$(PORT_APU)

.PHONY: help run estadisticas apuestas stop build clean status

help: ## Muestra los targets disponibles
	@echo "Frontends UTN GolMundial 2026"
	@echo ""
	@echo "  make run            Levanta estadísticas + apuestas en paralelo"
	@echo "  make estadisticas   Solo portal invitado → $(URL_EST)"
	@echo "  make apuestas       Solo portal apuestas → $(URL_APU)"
	@echo "  make stop           Detiene lo que escuche en $(PORT_EST) y $(PORT_APU)"
	@echo "  make build          Compila ambos proyectos"
	@echo "  make status         Muestra qué puertos están ocupados"
	@echo "  make clean          Limpia bin/obj de ambos frontends"
	@echo ""
	@echo "Defaults:       BIND_EST=$(BIND_EST)  BIND_APU=$(BIND_APU)"
	@echo "Links:          $(URL_EST)  |  $(URL_APU)"
	@echo "Escucha:        $(LISTEN):$(PORT_EST) / $(LISTEN):$(PORT_APU)"
	@echo "Solo local:     make run BIND_EST=localhost:5080 BIND_APU=localhost:5081"
	@echo ""
	@echo "Nota: no uses 'cd A && dotnet run' dos veces en la misma terminal."

run: ## Ambos frontends a la vez
	@echo "→ Estadísticas $(URL_EST)  (listen $(LISTEN_EST))"
	@echo "→ Apuestas     $(URL_APU)  (listen $(LISTEN_APU))"
	@echo "  (Ctrl+C para detener ambos)"
	@$(MAKE) -j2 estadisticas apuestas

estadisticas: ## Portal invitado (solo consulta)
	@echo "→ $(URL_EST)"
	cd "$(ESTADISTICAS)" && \
		Frontends__ApuestasUrl="$(URL_APU)" \
		dotnet run --no-launch-profile --urls "$(LISTEN_EST)"

apuestas: ## Portal de apuestas (registro para predecir)
	@echo "→ $(URL_APU)"
	cd "$(APUESTAS)" && \
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
