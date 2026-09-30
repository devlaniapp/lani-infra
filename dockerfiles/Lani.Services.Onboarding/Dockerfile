# ═══════════════════════════════════════════════════════════════════════════
# Dockerfile — Lani.Services.Onboarding
# Puerto de escucha: 7070
# ═══════════════════════════════════════════════════════════════════════════

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Lani.Services.Onboarding.Api/Lani.Services.Onboarding.Api.csproj",               "src/Lani.Services.Onboarding.Api/"]
COPY ["src/Lani.Services.Onboarding.Application/Lani.Services.Onboarding.Application.csproj", "src/Lani.Services.Onboarding.Application/"]
COPY ["src/Lani.Services.Onboarding.Domain/Lani.Services.Onboarding.Domain.csproj",           "src/Lani.Services.Onboarding.Domain/"]
COPY ["src/Lani.Services.Onboarding.Infrastructure/Lani.Services.Onboarding.Infrastructure.csproj", "src/Lani.Services.Onboarding.Infrastructure/"]

RUN dotnet restore "src/Lani.Services.Onboarding.Api/Lani.Services.Onboarding.Api.csproj"

COPY . .

WORKDIR "/src/src/Lani.Services.Onboarding.Api"
RUN dotnet build "Lani.Services.Onboarding.Api.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "Lani.Services.Onboarding.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

LABEL org.opencontainers.image.title="Lani.Services.Onboarding"
LABEL org.opencontainers.image.description="Microservicio de Onboarding — Plataforma Lani"
LABEL org.opencontainers.image.vendor="devlaniapp"

WORKDIR /app
EXPOSE 7070
ENV ASPNETCORE_URLS=http://+:7070
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=publish /app/publish .

RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Lani.Services.Onboarding.Api.dll"]
