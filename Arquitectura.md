# ARQUITECTURA DEL SISTEMA
# GlowSkin Store – Plataforma Web de Skincare
# Arquitectura Base para Desarrollo en Visual Studio

---

# 1. Objetivo de la Arquitectura

La presente arquitectura define la estructura oficial del proyecto para el desarrollo del sistema web de venta de productos skincare.

El objetivo es:

- Mantener una arquitectura limpia y escalable.
- Separar responsabilidades.
- Facilitar mantenimiento.
- Facilitar pruebas.
- Permitir crecimiento futuro.
- Mantener compatibilidad con SDD (Specific Driven Development).

La solución será desarrollada en Visual Studio utilizando ASP.NET Core MVC.

---

# 2. Tipo de Arquitectura

La solución utilizará una arquitectura basada en capas:

- Domain Layer
- Infrastructure Layer
- Presentation Layer (Web)
- Test Layer

Esta arquitectura sigue principios similares a:

- Clean Architecture
- Onion Architecture
- DDD simplificado

---

# 3. Estructura General de la Solución

```text
SalesSuite.sln
│
├── SalesSuite.Domain
├── SalesSuite.Infrastructure
├── SalesSuite.Web
└── SalesSuite.Test
```

---

# 4. Arquitectura de Dependencias

```text
SalesSuite.Web
    ↓
SalesSuite.Infrastructure
    ↓
SalesSuite.Domain

SalesSuite.Test
    ↓
Infrastructure + Domain + Web
```

---

# 5. Descripción de Cada Proyecto

# 5.1 SalesSuite.Domain

Contiene toda la lógica central del negocio.

Este proyecto NO debe depender de ningún otro proyecto.

Aquí se definen:

- Entidades
- Interfaces
- Reglas del negocio
- Contratos
- Modelos principales

---

## Estructura del Proyecto Domain

```text
SalesSuite.Domain
│
├── Entities
├── Interfaces
└── Common
```

---

## Carpeta: Entities

Contiene las entidades principales del sistema.

Ejemplos:

```text
Entities
│
├── User.cs
├── Product.cs
├── Category.cs
├── Cart.cs
├── CartItem.cs
├── Order.cs
├── OrderDetail.cs
├── Payment.cs
├── Review.cs
├── Promotion.cs
└── BlogPost.cs
```

---

## Responsabilidad de las Entidades

Las entidades representan los objetos principales del negocio.

Ejemplo:

- Producto
- Usuario
- Pedido
- Carrito
- Pago

Las entidades deben contener:

- Propiedades
- Relaciones
- Validaciones básicas
- Reglas del negocio simples

---

## Carpeta: Interfaces

Contiene contratos que serán implementados en Infrastructure.

Ejemplos:

```text
Interfaces
│
├── IRepository.cs
├── IProductRepository.cs
├── IUserRepository.cs
├── IOrderRepository.cs
├── ICategoryRepository.cs
├── IReviewRepository.cs
└── IUnitOfWork.cs
```

---

## Carpeta: Common

Contiene clases compartidas del dominio.

Ejemplos:

```text
Common
│
├── BaseEntity.cs
├── ResponseModel.cs
├── Constants.cs
└── Enums.cs
```

---

# 5.2 SalesSuite.Infrastructure

Contiene la implementación técnica del sistema.

Aquí se conecta:

- Base de datos
- Entity Framework
- Repositorios
- Migraciones
- Persistencia

Infrastructure depende de Domain.

---

## Estructura del Proyecto Infrastructure

```text
SalesSuite.Infrastructure
│
├── Data
├── Repositories
├── Migrations
├── Services
└── Configurations
```

---

## Carpeta: Data

Contiene el contexto de base de datos.

Ejemplo:

```text
Data
│
├── ApplicationDbContext.cs
└── SeedData.cs
```

---

## ApplicationDbContext

Responsable de:

- Configuración de tablas
- Relaciones
- Entity Framework Core
- Mapeo ORM

---

## Carpeta: Repositories

Implementa las interfaces del Domain.

Ejemplo:

```text
Repositories
│
├── Repository.cs
├── ProductRepository.cs
├── UserRepository.cs
├── OrderRepository.cs
├── CategoryRepository.cs
└── ReviewRepository.cs
```

---

## Responsabilidad de los Repositories

Los repositorios gestionan:

- Consultas
- Inserciones
- Actualizaciones
- Eliminaciones
- Acceso a base de datos

---

## Carpeta: Migrations

Contiene migraciones de Entity Framework.

Ejemplo:

```text
Migrations
│
├── InitialCreate.cs
├── AddOrders.cs
└── AddReviews.cs
```

---

## Carpeta: Services

Contendrá servicios internos del sistema.

Ejemplos:

```text
Services
│
├── EmailService.cs
├── PaymentService.cs
├── ImageService.cs
└── AuthService.cs
```

---

## Carpeta: Configurations

Configuraciones avanzadas de entidades.

Ejemplo:

```text
Configurations
│
├── ProductConfiguration.cs
├── OrderConfiguration.cs
└── UserConfiguration.cs
```

---

# 5.3 SalesSuite.Web

Es la capa de presentación.

Contiene:

- Interfaz gráfica
- Controladores
- Vistas
- DTOs
- Configuración web
- Archivos estáticos

Esta capa depende de:

- Infrastructure
- Domain

---

## Estructura del Proyecto Web

```text
SalesSuite.Web
│
├── Controllers
├── DTOs
├── Mappings
├── Models
├── Views
├── wwwroot
├── Properties
├── Middlewares
├── Helpers
├── Extensions
├── ViewModels
├── Filters
├── appsettings.json
└── Program.cs
```

---

# 6. Descripción de Carpetas Web

## Controllers

Controlan las solicitudes HTTP.

Ejemplos:

```text
Controllers
│
├── HomeController.cs
├── ProductController.cs
├── CartController.cs
├── OrderController.cs
├── AuthController.cs
├── AdminController.cs
├── ReviewController.cs
└── BlogController.cs
```

---

## DTOs

Objetos de transferencia de datos.

Ejemplos:

```text
DTOs
│
├── ProductDto.cs
├── RegisterDto.cs
├── LoginDto.cs
├── OrderDto.cs
└── ReviewDto.cs
```

---

## Mappings

Configuraciones AutoMapper.

Ejemplo:

```text
Mappings
│
└── AutoMapperProfile.cs
```

---

## Models

Modelos utilizados por MVC.

Ejemplo:

```text
Models
│
├── ErrorViewModel.cs
├── LoginViewModel.cs
└── RegisterViewModel.cs
```

---

## ViewModels

Modelos especializados para vistas.

Ejemplo:

```text
ViewModels
│
├── ProductViewModel.cs
├── CartViewModel.cs
├── CheckoutViewModel.cs
└── DashboardViewModel.cs
```

---

## Views

Contiene las vistas Razor.

Ejemplo:

```text
Views
│
├── Home
├── Product
├── Cart
├── Order
├── Auth
├── Admin
├── Shared
└── Blog
```

---

## wwwroot

Archivos estáticos del frontend.

Ejemplo:

```text
wwwroot
│
├── css
├── js
├── images
├── uploads
└── lib
```

---

## Middlewares

Middlewares personalizados.

Ejemplos:

```text
Middlewares
│
├── ExceptionMiddleware.cs
└── AuthenticationMiddleware.cs
```

---

## Helpers

Clases auxiliares reutilizables.

Ejemplo:

```text
Helpers
│
├── FileHelper.cs
├── ImageHelper.cs
└── PriceHelper.cs
```

---

## Extensions

Métodos de extensión para configuración.

Ejemplo:

```text
Extensions
│
├── ServiceExtensions.cs
└── MiddlewareExtensions.cs
```

---

## Filters

Filtros personalizados MVC.

Ejemplo:

```text
Filters
│
├── AdminAuthorizationFilter.cs
└── ValidationFilter.cs
```

---

# 7. Program.cs

Responsable de:

- Configuración de servicios
- Dependency Injection
- Middleware pipeline
- Configuración MVC
- Configuración Entity Framework
- Configuración Authentication
- Configuración Sessions

---

# 8. appsettings.json

Contendrá:

- Connection Strings
- Configuración JWT
- Configuración Email
- Configuración Storage
- Configuración Logging

Ejemplo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  }
}
```

---

# 9. SalesSuite.Test

Proyecto dedicado a pruebas.

Contendrá:

- Unit Tests
- Integration Tests
- Mock Services

---

## Estructura del Proyecto Test

```text
SalesSuite.Test
│
├── UnitTests
├── IntegrationTests
└── MockData
```

---

# 10. Arquitectura de Base de Datos

## Tablas Principales

```text
Users
Roles
Products
Categories
ProductImages
Cart
CartItems
Orders
OrderDetails
Payments
Reviews
Promotions
BlogPosts
Addresses
```

---

# 11. Flujo General del Sistema

```text
Usuario
   ↓
Views
   ↓
Controllers
   ↓
Services / Repositories
   ↓
DbContext
   ↓
Base de Datos
```

---

# 12. Patrón Repository

El sistema utilizará Repository Pattern.

Objetivo:

- Separar lógica de acceso a datos.
- Facilitar pruebas.
- Mejorar mantenibilidad.

---

# 13. Dependency Injection

Toda dependencia deberá inyectarse mediante constructor.

Ejemplo:

```csharp
public class ProductController : Controller
{
    private readonly IProductRepository _repository;

    public ProductController(IProductRepository repository)
    {
        _repository = repository;
    }
}
```

---

# 14. Convenciones del Proyecto

## Naming Convention

- PascalCase para clases.
- camelCase para variables.
- Interfaces con prefijo I.
- Sufijo Repository para repositorios.
- Sufijo Service para servicios.

---

## Convenciones de Carpetas

- Una responsabilidad por carpeta.
- Separación clara entre capas.
- No mezclar lógica de negocio con UI.

---

# 15. Recomendaciones de Desarrollo

## Backend

- Usar Entity Framework Core.
- Implementar LINQ.
- Usar DTOs.
- Validar modelos.
- Manejar excepciones globalmente.

---

## Frontend

- Bootstrap 5.
- Diseño responsive.
- Componentes reutilizables.
- Validaciones cliente-servidor.

---

# 16. Seguridad

El sistema deberá implementar:

- Authentication
- Authorization
- Password Hashing
- Protección CSRF
- Validaciones
- Roles y permisos

---

# 17. Escalabilidad

La arquitectura está preparada para:

- API REST futura.
- Aplicación móvil.
- Microservicios.
- Integraciones externas.
- IA y recomendaciones.
- Cloud deployment.

---

# 18. Objetivo Final de la Arquitectura

Esta arquitectura busca:

- Organización profesional.
- Escalabilidad.
- Bajo acoplamiento.
- Alta mantenibilidad.
- Desarrollo limpio.
- Compatibilidad con SDD.
- Facilidad de desarrollo con IA.
- Base sólida para crecimiento futuro.