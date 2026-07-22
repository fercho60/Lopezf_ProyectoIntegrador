# UTN GolMundial 2026 — frontends públicos
# Uso: make help | make run | make estadisticas | make apuestas | make stop
#
# Por defecto escucha en la IP de red local (accesible desde otras máquinas).
# Para solo esta PC: make run BIND_EST=localhost BIND_APU=localhost

ROOT := $(abspath $(dir $(lastword $(MAKEFILE_LIST))))
ESTADISTICAS := $(ROOT)/frontend-estadisticas-mvc
APUESTAS := $(ROOT)/frontend-publico-mvc

# IPs/hosts al levantar (Fern / demo en red local)
BIND_EST ?= 172.28.114.135
BIND_APU ?= 172.28.114.136
PORT_EST ?= 5080
PORT_APU ?= 5081

URL_EST := http://$(BIND_EST):$(PORT_EST)
URL_APU := http://$(BIND_APU):$(PORT_APU)

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
	@echo "Hosts actuales: BIND_EST=$(BIND_EST)  BIND_APU=$(BIND_APU)"
	@echo "Localhost:      make run BIND_EST=localhost BIND_APU=localhost"
	@echo ""
	@echo "Nota: no uses 'cd A && dotnet run' dos veces en la misma terminal."

run: ## Ambos frontends a la vez
	@echo "→ Estadísticas $(URL_EST)"
	@echo "→ Apuestas     $(URL_APU)"
	@echo "  (Ctrl+C para detener ambos)"
	@$(MAKE) -j2 estadisticas apuestas

estadisticas: ## Portal invitado (solo consulta)
	@echo "→ $(URL_EST)"
	cd "$(ESTADISTICAS)" && \
		Frontends__ApuestasUrl="$(URL_APU)" \
		dotnet run --no-launch-profile --urls "$(URL_EST)"

apuestas: ## Portal de apuestas (registro para predecir)
	@echo "→ $(URL_APU)"
	cd "$(APUESTAS)" && \
		Frontends__EstadisticasUrl="$(URL_EST)" \
		dotnet run --no-launch-profile --urls "$(URL_APU)"

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
