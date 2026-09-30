# ═══════════════════════════════════════════════════════════════════════════
# Dockerfile — Lani.Services.Companies
# Puerto de escucha: 7070
# ═══════════════════════════════════════════════════════════════════════════

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Lani.Services.Companies.Api/Lani.Services.Companies.Api.csproj",               "src/Lani.Services.Companies.Api/"]
COPY ["src/Lani.Services.Companies.Application/Lani.Services.Companies.Application.csproj", "src/Lani.Services.Companies.Application/"]
COPY ["src/Lani.Services.Companies.Domain/Lani.Services.Companies.Domain.csproj",           "src/Lani.Services.Companies.Domain/"]
COPY ["src/Lani.Services.Companies.Infrastructure/Lani.Services.Companies.Infrastructure.csproj", "src/Lani.Services.Companies.Infrastructure/"]

RUN dotnet restore "src/Lani.Services.Companies.Api/Lani.Services.Companies.Api.csproj"

COPY . .

WORKDIR "/src/src/Lani.Services.Companies.Api"
RUN dotnet build "Lani.Services.Companies.Api.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "Lani.Services.Companies.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

LABEL org.opencontainers.image.title="Lani.Services.Companies"
LABEL org.opencontainers.image.description="Microservicio de Empresas — Plataforma Lani"
LABEL org.opencontainers.image.vendor="devlaniapp"

WORKDIR /app
EXPOSE 7070
ENV ASPNETCORE_URLS=http://+:7070
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=publish /app/publish .

RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Lani.Services.Companies.Api.dll"]
