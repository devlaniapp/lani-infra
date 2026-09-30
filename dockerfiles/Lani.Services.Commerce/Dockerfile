# ═══════════════════════════════════════════════════════════════════════════
# Dockerfile — Lani.Services.Commerce
# Puerto de escucha: 7070
# ═══════════════════════════════════════════════════════════════════════════

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Lani.Services.Commerce.Api/Lani.Services.Commerce.Api.csproj",             "src/Lani.Services.Commerce.Api/"]
COPY ["src/Lani.Services.Commerce.Application/Lani.Services.Commerce.Application.csproj", "src/Lani.Services.Commerce.Application/"]
COPY ["src/Lani.Services.Commerce.Domain/Lani.Services.Commerce.Domain.csproj",         "src/Lani.Services.Commerce.Domain/"]
COPY ["src/Lani.Services.Commerce.Infrastructure/Lani.Services.Commerce.Infrastructure.csproj", "src/Lani.Services.Commerce.Infrastructure/"]

RUN dotnet restore "src/Lani.Services.Commerce.Api/Lani.Services.Commerce.Api.csproj"

COPY . .

WORKDIR "/src/src/Lani.Services.Commerce.Api"
RUN dotnet build "Lani.Services.Commerce.Api.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "Lani.Services.Commerce.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

LABEL org.opencontainers.image.title="Lani.Services.Commerce"
LABEL org.opencontainers.image.description="Microservicio de Comercio — Plataforma Lani"
LABEL org.opencontainers.image.vendor="devlaniapp"

WORKDIR /app
EXPOSE 7070
ENV ASPNETCORE_URLS=http://+:7070
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=publish /app/publish .

RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Lani.Services.Commerce.Api.dll"]
