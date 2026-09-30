# ═══════════════════════════════════════════════════════════════════════════
# Dockerfile — Lani.Services.Products
# Puerto de escucha: 7070
# ═══════════════════════════════════════════════════════════════════════════

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Lani.Services.Products.Api/Lani.Services.Products.Api.csproj",               "src/Lani.Services.Products.Api/"]
COPY ["src/Lani.Services.Products.Application/Lani.Services.Products.Application.csproj", "src/Lani.Services.Products.Application/"]
COPY ["src/Lani.Services.Products.Domain/Lani.Services.Products.Domain.csproj",           "src/Lani.Services.Products.Domain/"]
COPY ["src/Lani.Services.Products.Infrastructure/Lani.Services.Products.Infrastructure.csproj", "src/Lani.Services.Products.Infrastructure/"]

RUN dotnet restore "src/Lani.Services.Products.Api/Lani.Services.Products.Api.csproj"

COPY . .

WORKDIR "/src/src/Lani.Services.Products.Api"
RUN dotnet build "Lani.Services.Products.Api.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "Lani.Services.Products.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

LABEL org.opencontainers.image.title="Lani.Services.Products"
LABEL org.opencontainers.image.description="Microservicio de Productos — Plataforma Lani"
LABEL org.opencontainers.image.vendor="devlaniapp"

WORKDIR /app
EXPOSE 7070
ENV ASPNETCORE_URLS=http://+:7070
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=publish /app/publish .

RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "Lani.Services.Products.Api.dll"]
