# 🏗️ Lani Infrastructure & Deployment Assets

Repositorio central de infraestructura, especificaciones Docker, configuraciones de proxy inverso (YARP) y colecciones de prueba para el ecosistema de microservicios de **Lani Platform** en **Railway**.

---

## 📁 Estructura del Repositorio

```text
lani-infra/
├── dockerfiles/                     # Plantillas Dockerfile multi-stage por microservicio (puerto 7070)
│   ├── Lani.API.Gateway/
│   ├── Lani.Services.Catalog/
│   ├── Lani.Services.Commerce/
│   ├── Lani.Services.Companies/
│   ├── Lani.Services.Identity/
│   ├── Lani.Services.Onboarding/
│   └── Lani.Services.Products/
├── gateway/                         # Definición y ruteo del API Gateway YARP
│   ├── Program.cs                   # Pipeline con validación JWT simétrica (HMAC-SHA256)
│   └── appsettings.json             # Ruteo y destinos hacia *.railway.internal:7070
├── Lani_Platform_Postman_Collection.json # Colección de Postman preconfigurada
├── nuget.config                     # Plantilla de registro para GitHub Packages
└── README.md
```

---

## 🚂 Arquitectura en Railway (`devlaniapp`)

* **Perímetro de Seguridad:** El único punto de entrada público es el `API Gateway` (`https://gateway-production-a6b6.up.railway.app`).
* **Red Privada Interna:** Todos los microservicios se comunican en la red interna privada `*.railway.internal:7070` sin exposición pública.
* **Autenticación:** Validación de JWT en frontera con clave simétrica en memoria antes de reenviar el tráfico downstream.
* **Optimización de Costos:** Modo Serverless / App-Sleeping habilitado en los 6 microservicios internos.
