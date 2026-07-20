# UTN GolMundial 2026 — frontends públicos
# Uso: make help | make run | make estadisticas | make apuestas | make stop

ROOT := $(abspath $(dir $(lastword $(MAKEFILE_LIST))))
ESTADISTICAS := $(ROOT)/frontend-estadisticas-mvc
APUESTAS := $(ROOT)/frontend-publico-mvc

.PHONY: help run estadisticas apuestas stop build clean status

help: ## Muestra los targets disponibles
	@echo "Frontends UTN GolMundial 2026"
	@echo ""
	@echo "  make run            Levanta estadísticas (:5080) y apuestas (:5081) en paralelo"
	@echo "  make estadisticas   Solo portal de estadísticas (invitado) → http://localhost:5080"
	@echo "  make apuestas       Solo portal de apuestas → http://localhost:5081"
	@echo "  make stop           Detiene lo que escuche en 5080 y 5081"
	@echo "  make build          Compila ambos proyectos"
	@echo "  make status         Muestra qué puertos están ocupados"
	@echo "  make clean          Limpia bin/obj de ambos frontends"
	@echo "  make help           Esta ayuda"
	@echo ""
	@echo "Nota: no uses 'cd A && dotnet run' seguido de 'cd B && ...' en la misma terminal:"
	@echo "      el primero bloquea. Usa 'make run' o dos terminales / 'make -j2'."

run: ## Ambos frontends a la vez (Ctrl+C detiene los dos si Make los gestiona)
	@echo "→ Estadísticas :5080  |  Apuestas :5081"
	@echo "  (Ctrl+C para detener ambos)"
	@$(MAKE) -j2 estadisticas apuestas

estadisticas: ## Portal invitado (solo consulta)
	@echo "→ http://localhost:5080"
	cd "$(ESTADISTICAS)" && dotnet run --urls http://localhost:5080

apuestas: ## Portal de apuestas (registro para predecir)
	@echo "→ http://localhost:5081"
	cd "$(APUESTAS)" && dotnet run --urls http://localhost:5081

stop: ## Libera los puertos 5080 y 5081
	@-lsof -tiTCP:5080 -sTCP:LISTEN 2>/dev/null | xargs kill 2>/dev/null || true
	@-lsof -tiTCP:5081 -sTCP:LISTEN 2>/dev/null | xargs kill 2>/dev/null || true
	@echo "Puertos 5080 y 5081 liberados (si había procesos)."

status: ## Quién escucha en 5080/5081
	@echo "=== :5080 (estadísticas) ==="
	@-lsof -nP -iTCP:5080 -sTCP:LISTEN 2>/dev/null || echo "(libre)"
	@echo "=== :5081 (apuestas) ==="
	@-lsof -nP -iTCP:5081 -sTCP:LISTEN 2>/dev/null || echo "(libre)"

build: ## Compilar ambos
	cd "$(ESTADISTICAS)" && dotnet build
	cd "$(APUESTAS)" && dotnet build

clean: ## Limpiar artefactos de build
	cd "$(ESTADISTICAS)" && dotnet clean
	cd "$(APUESTAS)" && dotnet clean
	@rm -rf "$(ESTADISTICAS)/bin" "$(ESTADISTICAS)/obj" "$(APUESTAS)/bin" "$(APUESTAS)/obj"
	@echo "Limpieza lista."
