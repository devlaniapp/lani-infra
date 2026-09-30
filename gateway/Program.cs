using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────────────────────
// 1. YARP — Reverse Proxy
//    Lee la configuración de rutas y clusters desde appsettings.json
// ─────────────────────────────────────────────────────────────────────────────
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// ─────────────────────────────────────────────────────────────────────────────
// 2. AUTENTICACIÓN JWT CENTRALIZADA
//    El Gateway valida el JWT usando la clave secreta o el endpoint JWKS de
//    Lani.Services.Identity (http://identity.railway.internal:8080)
//    Las peticiones válidas se reenvían downstream; las inválidas son
//    rechazadas aquí, ANTES de llegar a cualquier microservicio.
// ─────────────────────────────────────────────────────────────────────────────
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Falta la configuración Jwt:Key en el Gateway.");
        var issuer = builder.Configuration["Jwt:Issuer"] ?? "Lani.Identity";
        var audience = builder.Configuration["Jwt:Audience"] ?? "Lani.Apps";

        options.RequireHttpsMetadata = false; // Red interna Railway
        options.SaveToken            = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidIssuer              = issuer,
            ValidateAudience         = true,
            ValidAudience            = audience,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew                = TimeSpan.FromSeconds(30),
            RoleClaimType            = "role" // O http://schemas.microsoft.com/ws/2008/06/identity/claims/role
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                ctx.Response.Headers["WWW-Authenticate"] = "Bearer error=\"invalid_token\"";
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// ─────────────────────────────────────────────────────────────────────────────
// 3. CORS
//    Permitir el origen del frontend Lani. En producción, limitar a dominios
//    específicos. Las variables de entorno sobreescriben la config base.
// ─────────────────────────────────────────────────────────────────────────────
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? ["*"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("LaniGatewayCors", policy =>
    {
        if (allowedOrigins.Contains("*"))
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

// ─────────────────────────────────────────────────────────────────────────────
// 4. HEALTH CHECK
// ─────────────────────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks();

// ─────────────────────────────────────────────────────────────────────────────
// PIPELINE
// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

// Health check — no requiere auth (para Railway health probe)
app.MapHealthChecks("/health");

// Swagger/OpenAPI solo en Development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("LaniGatewayCors");

// Autenticación y autorización ANTES de que YARP reenvíe la petición
app.UseAuthentication();
app.UseAuthorization();

// YARP: reenviar al cluster correcto según las rutas
app.MapReverseProxy();

app.Run();
