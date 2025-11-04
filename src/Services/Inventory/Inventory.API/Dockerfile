# ==============================================
# INVENTORY.API DOCKERFILE
# Оптимизированная сборка для продакшена
# ==============================================

# --- STAGE 1: Base Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Создаем non-root пользователя для безопасности
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --ingroup appgroup appuser

# --- STAGE 2: Build ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем только .csproj для кеширования зависимостей
COPY ["src/Services/Inventory/Inventory.API/Inventory.API.csproj", "Inventory.API/"]
COPY ["src/Services/Inventory/Inventory.Domain/Inventory.Domain.csproj", "Inventory.Domain/"]
COPY ["src/Services/Inventory/Inventory.Infrastructure/Inventory.Infrastructure.csproj", "Inventory.Infrastructure/"]

# Restore зависимостей (слой кешируется!)
RUN dotnet restore "Inventory.API/Inventory.API.csproj"

# Копируем остальной код
COPY src/Services/Inventory/ .

# Сборка проекта
WORKDIR /src/Inventory.API
RUN dotnet build "Inventory.API.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

# --- STAGE 3: Publish ---
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Inventory.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# --- STAGE 4: Final Runtime ---
FROM base AS final
WORKDIR /app

# Копируем собранное приложение
COPY --from=publish /app/publish .

# Переключаемся на non-root пользователя
USER appuser

# Health check для Docker/Kubernetes
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Inventory.API.dll"]
