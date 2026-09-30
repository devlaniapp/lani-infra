# ═══════════════════════════════════════════════════════════════════════════
# Dockerfile — Lani.API.Gateway
# API Gateway basado en YARP (Yet Another Reverse Proxy) con validación JWT
# Puerto de escucha: 7070
# ═══════════════════════════════════════════════════════════════════════════

# ── Stage 1: Build ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solo los archivos de proyecto primero (mejor cache de capas)
COPY ["src/Lani.API.Gateway/Lani.API.Gateway.csproj", "src/Lani.API.Gateway/"]

# Restaurar dependencias NuGet
RUN dotnet restore "src/Lani.API.Gateway/Lani.API.Gateway.csproj"

# Copiar todo el código fuente
COPY . .

# Compilar en modo Release
WORKDIR "/src/src/Lani.API.Gateway"
RUN dotnet build "Lani.API.Gateway.csproj" -c Release -o /app/build --no-restore

# ── Stage 2: Publish ─────────────────────────────────────────────────────────
FROM build AS publish
RUN dotnet publish "Lani.API.Gateway.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ── Stage 3: Runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Metadatos
LABEL org.opencontainers.image.title="Lani.API.Gateway"
LABEL org.opencontainers.image.description="API Gateway YARP para la plataforma Lani"
LABEL org.opencontainers.image.vendor="devlaniapp"

WORKDIR /app

# Puerto estándar Railway / Lani
EXPOSE 7070
ENV ASPNETCORE_URLS=http://+:7070
ENV ASPNETCORE_ENVIRONMENT=Production

# Copiar artefactos publicados desde el stage anterior
COPY --from=publish /app/publish .

# Usuario no-root por seguridad
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Lani.API.Gateway.dll"]
