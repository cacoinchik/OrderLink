# ==============================================
# ORDERLINK MAKEFILE
# Удобные команды для работы с Docker
# ==============================================

.PHONY: help up down build logs clean restart health db-backup db-restore

# Цвета для вывода
GREEN  := \033[0;32m
YELLOW := \033[0;33m
RED    := \033[0;31m
NC     := \033[0m # No Color

help: ## Показать эту справку
	@echo "$(GREEN)═══════════════════════════════════════$(NC)"
	@echo "$(GREEN)  OrderLink - Docker Commands$(NC)"
	@echo "$(GREEN)═══════════════════════════════════════$(NC)"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | \
		awk 'BEGIN {FS = ":.*?## "}; {printf "$(YELLOW)%-20s$(NC) %s\n", $$1, $$2}'
	@echo ""

up: ## Запустить все сервисы
	@echo "$(GREEN)🚀 Запуск OrderLink...$(NC)"
	docker compose up -d
	@echo "$(GREEN)✅ Готово! Swagger: http://localhost:5267/swagger$(NC)"

up-build: ## Запустить с пересборкой
	@echo "$(GREEN)🔨 Сборка и запуск...$(NC)"
	docker compose up --build -d
	@echo "$(GREEN)✅ Готово!$(NC)"

down: ## Остановить все сервисы (данные сохраняются)
	@echo "$(YELLOW)⏸️  Остановка сервисов...$(NC)"
	docker compose down
	@echo "$(GREEN)✅ Остановлено$(NC)"

down-clean: ## Остановить + удалить volumes (⚠️ УДАЛИТ ВСЕ ДАННЫЕ!)
	@echo "$(RED)⚠️  УДАЛЕНИЕ ВСЕХ ДАННЫХ!$(NC)"
	@read -p "Вы уверены? [y/N]: " confirm && [ "$$confirm" = "y" ] || exit 1
	docker compose down -v
	@echo "$(GREEN)✅ Очищено$(NC)"

build: ## Пересобрать образы
	@echo "$(GREEN)🔨 Сборка образов...$(NC)"
	docker compose build

build-no-cache: ## Пересобрать без кеша
	@echo "$(GREEN)🔨 Сборка без кеша...$(NC)"
	docker compose build --no-cache

logs: ## Показать логи всех сервисов
	docker compose logs -f

logs-api: ## Логи Orders API
	docker compose logs -f orders-api

logs-inventory: ## Логи Inventory API
	docker compose logs -f inventory-api

logs-kafka: ## Логи Kafka
	docker compose logs -f kafka

ps: ## Статус всех сервисов
	@echo "$(GREEN)📊 Статус сервисов:$(NC)"
	docker compose ps

restart: ## Перезапустить все сервисы
	@echo "$(YELLOW)🔄 Перезапуск...$(NC)"
	docker compose restart
	@echo "$(GREEN)✅ Готово$(NC)"

restart-api: ## Перезапустить Orders API
	@echo "$(YELLOW)🔄 Перезапуск Orders API...$(NC)"
	docker compose restart orders-api

restart-inventory: ## Перезапустить Inventory API
	@echo "$(YELLOW)🔄 Перезапуск Inventory API...$(NC)"
	docker compose restart inventory-api

health: ## Проверить здоровье сервисов
	@echo "$(GREEN)🏥 Health Checks:$(NC)"
	@docker compose ps | grep "healthy" && echo "$(GREEN)✅ Все сервисы здоровы$(NC)" || echo "$(RED)⚠️  Проблемы обнаружены$(NC)"

shell-api: ## Зайти в контейнер Orders API
	docker exec -it orderlink-orders-api bash

shell-db: ## Подключиться к PostgreSQL Orders
	docker exec -it orderlink-postgres-orders psql -U postgres -d OrdersDb

shell-redis: ## Подключиться к Redis
	docker exec -it orderlink-redis redis-cli -a redispass

db-backup: ## Backup базы Orders
	@echo "$(GREEN)💾 Создание backup...$(NC)"
	docker exec orderlink-postgres-orders pg_dump -U postgres OrdersDb > backup_$$(date +%Y%m%d_%H%M%S).sql
	@echo "$(GREEN)✅ Backup создан$(NC)"

db-restore: ## Restore базы Orders (укажи файл: make db-restore FILE=backup.sql)
	@echo "$(YELLOW)📥 Восстановление из $(FILE)...$(NC)"
	cat $(FILE) | docker exec -i orderlink-postgres-orders psql -U postgres -d OrdersDb
	@echo "$(GREEN)✅ Восстановлено$(NC)"

clean: ## Очистить неиспользуемые ресурсы Docker
	@echo "$(YELLOW)🧹 Очистка...$(NC)"
	docker system prune -f
	@echo "$(GREEN)✅ Готово$(NC)"

clean-all: ## Полная очистка (⚠️ удалит всё!)
	@echo "$(RED)⚠️  ПОЛНАЯ ОЧИСТКА DOCKER!$(NC)"
	@read -p "Вы уверены? [y/N]: " confirm && [ "$$confirm" = "y" ] || exit 1
	docker system prune -a --volumes -f
	@echo "$(GREEN)✅ Всё удалено$(NC)"

stats: ## Показать использование ресурсов
	docker stats --no-stream

urls: ## Показать все URL сервисов
	@echo "$(GREEN)🌐 Доступные URL:$(NC)"
	@echo "  Orders API Swagger:    $(YELLOW)http://localhost:5267/swagger$(NC)"
	@echo "  Inventory API Swagger: $(YELLOW)http://localhost:5258/swagger$(NC)"
	@echo "  Kafka UI:              $(YELLOW)http://localhost:8080$(NC)"
	@echo "  PostgreSQL Orders:     $(YELLOW)localhost:5432$(NC)"
	@echo "  PostgreSQL Inventory:  $(YELLOW)localhost:5433$(NC)"
	@echo "  Redis:                 $(YELLOW)localhost:6379$(NC)"

# Быстрые команды для разработки
dev: up urls ## Запустить в dev режиме и показать URLs

prod: ## Запустить в production режиме
	@echo "$(GREEN)🚀 Production mode...$(NC)"
	docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d

migrate-orders: ## Применить миграции Orders
	docker exec orderlink-orders-api dotnet ef database update

migrate-inventory: ## Применить миграции Inventory
	docker exec orderlink-inventory-api dotnet ef database update
