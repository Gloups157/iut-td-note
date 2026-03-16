# Makefile – tests de charge et spike (k6) via Docker, réseau host, métriques → InfluxDB
# Prérequis : AppHost Aspire démarré (FilmApi + InfluxDB), seed 50k ou 500k exécutée.
# Réseau host : sous Linux/WSL, k6 accède à localhost (API + InfluxDB). Sous Windows natif, utiliser WSL2.
#
# Usage :
#   make load-50k              # Load test, 50 000 films, sortie InfluxDB
#   make load-500k             # Load test, 500 000 films
#   make spike-50k             # Spike test, 50 000 films
#   make spike-500k            # Spike test, 500 000 films
#   make load-50k BASE_URL=http://localhost:5243

BASE_URL ?= http://localhost:5000
COMPOSE_K6 := docker compose -f docker-compose.k6.yml
K6_SCRIPT_DIR := /scripts

.PHONY: load-50k load-500k spike-50k spike-500k k6-build help

help:
	@echo "Cibles disponibles :"
	@echo "  load-50k    - Load test Docker (TOTAL_ITEMS=50000), sortie InfluxDB"
	@echo "  load-500k   - Load test Docker (TOTAL_ITEMS=500000)"
	@echo "  spike-50k   - Spike test Docker (TOTAL_ITEMS=50000)"
	@echo "  spike-500k  - Spike test Docker (TOTAL_ITEMS=500000)"
	@echo "  k6-build    - Construire l’image Docker k6 (xk6-influxdb)"
	@echo ""
	@echo "Variables : BASE_URL (défaut: http://localhost:5000)"
	@echo "InfluxDB : K6_INFLUXDB_ADDR=http://127.0.0.1:8086 (réseau host)"
	@echo "Exemple : make load-50k BASE_URL=http://localhost:5243"

k6-build:
	$(COMPOSE_K6) build k6

load-50k: k6-build
	$(COMPOSE_K6) run --rm -e BASE_URL="$(BASE_URL)" -e TOTAL_ITEMS=50000 k6 run $(K6_SCRIPT_DIR)/load.js -o xk6-influxdb

load-500k: k6-build
	$(COMPOSE_K6) run --rm -e BASE_URL="$(BASE_URL)" -e TOTAL_ITEMS=500000 k6 run $(K6_SCRIPT_DIR)/load.js -o xk6-influxdb

spike-50k: k6-build
	$(COMPOSE_K6) run --rm -e BASE_URL="$(BASE_URL)" -e TOTAL_ITEMS=50000 k6 run $(K6_SCRIPT_DIR)/spike.js -o xk6-influxdb

spike-500k: k6-build
	$(COMPOSE_K6) run --rm -e BASE_URL="$(BASE_URL)" -e TOTAL_ITEMS=500000 k6 run $(K6_SCRIPT_DIR)/spike.js -o xk6-influxdb
