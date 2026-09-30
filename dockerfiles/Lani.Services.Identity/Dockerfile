# ═══════════════════════════════════════════════════════════════════════════
# Dockerfile — Lani.Services.Identity
# Puerto de escucha: 7070
# NOTA: Este servicio emite y valida JWT. Es el proveedor JWKS del Gateway.
# ═══════════════════════════════════════════════════════════════════════════

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Lani.Services.Identity.Api/Lani.Services.Identity.Api.csproj",               "src/Lani.Services.Identity.Api/"]
COPY ["src/Lani.Services.Identity.Application/Lani.Services.Identity.Application.csproj", "src/Lani.Services.Identity.Application/"]
COPY ["src/Lani.Services.Identity.Domain/Lani.Services.Identity.Domain.csproj",           "src/Lani.Services.Identity.Domain/"]
COPY ["src/Lani.Services.Identity.Infrastructure/Lani.Services.Identity.Infrastructure.csproj", "src/Lani.Services.Identity.Infrastructure/"]

RUN dotnet restore "src/Lani.Services.Identity.Api/Lani.Services.Identity.Api.csproj"

COPY . .

WORKDIR "/src/src/Lani.Services.Identity.Api"
RUN dotnet build "Lani.Services.Identity.Api.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "Lani.Services.Identity.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

LABEL org.opencontainers.image.title="Lani.Services.Identity"
LABEL org.opencontainers.image.description="Microservicio de Identidad y Auth — Plataforma Lani"
LABEL org.opencontainers.image.vendor="devlaniapp"

WORKDIR /app
EXPOSE 7070
ENV ASPNETCORE_URLS=http://+:7070
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=publish /app/publish .

RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Lani.Services.Identity.Api.dll"]
