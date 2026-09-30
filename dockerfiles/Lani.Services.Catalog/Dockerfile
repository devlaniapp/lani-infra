# ═══════════════════════════════════════════════════════════════════════════
# Dockerfile — Lani.Services.Catalog
# Puerto de escucha: 7070
# ═══════════════════════════════════════════════════════════════════════════

# ── Stage 1: Build ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto por capa (Clean Architecture)
COPY ["Lani.Services.Catalog/Lani.Services.Catalog.API.csproj",                   "Lani.Services.Catalog/"]
COPY ["Lani.Services.Catalog.Application/Lani.Services.Catalog.Application.csproj", "Lani.Services.Catalog.Application/"]
COPY ["Lani.Services.Catalog.Domain/Lani.Services.Catalog.Domain.csproj",           "Lani.Services.Catalog.Domain/"]
COPY ["Lani.Services.Catalog.Infrastructure/Lani.Services.Catalog.Infrastructure.csproj", "Lani.Services.Catalog.Infrastructure/"]

RUN dotnet restore "Lani.Services.Catalog/Lani.Services.Catalog.API.csproj"

COPY . .

WORKDIR "/src/Lani.Services.Catalog"
RUN dotnet build "Lani.Services.Catalog.API.csproj" -c Release -o /app/build --no-restore

# ── Stage 2: Publish ─────────────────────────────────────────────────────────
FROM build AS publish
RUN dotnet publish "Lani.Services.Catalog.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ── Stage 3: Runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

LABEL org.opencontainers.image.title="Lani.Services.Catalog"
LABEL org.opencontainers.image.description="Microservicio de Catálogo — Plataforma Lani"
LABEL org.opencontainers.image.vendor="devlaniapp"

WORKDIR /app
EXPOSE 7070
ENV ASPNETCORE_URLS=http://+:7070
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=publish /app/publish .

RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Lani.Services.Catalog.API.dll"]
