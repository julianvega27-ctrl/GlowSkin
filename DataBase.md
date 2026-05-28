# BASE DE DATOS DEL PROYECTO
# GlowSkin Store – Plataforma Web de Skincare
# SQL Server Database Design

---

# 1. Objetivo

Este documento define toda la estructura de base de datos necesaria para el sistema GlowSkin Store.

La base de datos estará diseñada para:

- Gestión de usuarios
- Gestión de productos
- Carrito de compras
- Gestión de pedidos
- Pagos
- Reseñas
- Promociones
- Blog informativo
- Administración del sistema

La base de datos será implementada en SQL Server.

---

# 2. Motor de Base de Datos

```text
SQL Server 2019 o superior
```

También compatible con:

- SQL Server Express
- SQL Server Developer
- Azure SQL Database

---

# 3. Configuración del Servidor SQL Server

## Servidor SQL Server

```text
LAPTOP-KB604S6R\MSSQLSERVER01
```

---

## Tipo de Autenticación

```text
Windows Authentication
```

---

# 4. Nombre de la Base de Datos

```sql
GlowSkinStoreDB
```

---

# 5. Creación de la Base de Datos

```sql
CREATE DATABASE GlowSkinStoreDB;
GO

USE GlowSkinStoreDB;
GO
```

---

# 6. Configuración de Conexión SQL Server

## Windows Authentication

```text
Server=LAPTOP-KB604S6R\MSSQLSERVER01;
Database=GlowSkinStoreDB;
Trusted_Connection=True;
TrustServerCertificate=True;
MultipleActiveResultSets=true;
```

---

# 7. Connection String para appsettings.json

## Windows Authentication

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=LAPTOP-KB604S6R\\MSSQLSERVER01;Database=GlowSkinStoreDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

---

# 8. Configuración en Program.cs

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
```

---

# 9. Diseño General de Tablas

```text
Users
Roles
UserRoles
Addresses
Categories
Products
ProductImages
Cart
CartItems
Orders
OrderDetails
Payments
Reviews
Promotions
ProductPromotions
BlogPosts
Wishlists
WishlistItems
```

---

# 10. Tabla Roles

```sql
CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE,
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO
```

---

# 11. Tabla Users

```sql
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    Phone NVARCHAR(20),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO
```

---

# 12. Tabla UserRoles

```sql
CREATE TABLE UserRoles (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,

    PRIMARY KEY (UserId, RoleId),

    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);
GO
```

---

# 13. Tabla Addresses

```sql
CREATE TABLE Addresses (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    AddressLine NVARCHAR(250) NOT NULL,
    City NVARCHAR(100),
    State NVARCHAR(100),
    PostalCode NVARCHAR(20),
    Country NVARCHAR(100),

    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO
```

---

# 14. Tabla Categories

```sql
CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    ImageUrl NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO
```

---

# 15. Tabla Products

```sql
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CategoryId INT NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Ingredients NVARCHAR(MAX),
    SkinType NVARCHAR(100),
    Price DECIMAL(18,2) NOT NULL,
    Stock INT NOT NULL DEFAULT 0,
    MainImageUrl NVARCHAR(500),
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);
GO
```

---

# 16. Tabla ProductImages

```sql
CREATE TABLE ProductImages (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductId INT NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    IsPrimary BIT DEFAULT 0,

    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
GO
```

---

# 17. Tabla Cart

```sql
CREATE TABLE Cart (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO
```

---

# 18. Tabla CartItems

```sql
CREATE TABLE CartItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CartId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,

    FOREIGN KEY (CartId) REFERENCES Cart(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
GO
```

---

# 19. Tabla Orders

```sql
CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    AddressId INT NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    PaymentStatus NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (AddressId) REFERENCES Addresses(Id)
);
GO
```

---

# 20. Tabla OrderDetails

```sql
CREATE TABLE OrderDetails (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    Subtotal DECIMAL(18,2) NOT NULL,

    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
GO
```

---

# 21. Tabla Payments

```sql
CREATE TABLE Payments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL,
    PaymentMethod NVARCHAR(100) NOT NULL,
    TransactionId NVARCHAR(250),
    Amount DECIMAL(18,2) NOT NULL,
    PaymentDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50),

    FOREIGN KEY (OrderId) REFERENCES Orders(Id)
);
GO
```

---

# 22. Tabla Reviews

```sql
CREATE TABLE Reviews (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductId INT NOT NULL,
    UserId INT NOT NULL,
    Rating INT NOT NULL,
    Comment NVARCHAR(1000),
    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO
```

---

# 23. Tabla Promotions

```sql
CREATE TABLE Promotions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000),
    DiscountPercentage DECIMAL(5,2),
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    IsActive BIT DEFAULT 1
);
GO
```

---

# 24. Tabla ProductPromotions

```sql
CREATE TABLE ProductPromotions (
    ProductId INT NOT NULL,
    PromotionId INT NOT NULL,

    PRIMARY KEY (ProductId, PromotionId),

    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    FOREIGN KEY (PromotionId) REFERENCES Promotions(Id)
);
GO
```

---

# 25. Tabla BlogPosts

```sql
CREATE TABLE BlogPosts (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(250) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    ImageUrl NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETDATE(),
    IsPublished BIT DEFAULT 1
);
GO
```

---

# 26. Tabla Wishlists

```sql
CREATE TABLE Wishlists (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,

    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO
```

---

# 27. Tabla WishlistItems

```sql
CREATE TABLE WishlistItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    WishlistId INT NOT NULL,
    ProductId INT NOT NULL,

    FOREIGN KEY (WishlistId) REFERENCES Wishlists(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
GO
```

---

# 28. Índices Recomendados

## Índice para búsqueda de productos

```sql
CREATE INDEX IX_Products_Name
ON Products(Name);
GO
```

---

## Índice para categorías

```sql
CREATE INDEX IX_Products_CategoryId
ON Products(CategoryId);
GO
```

---

## Índice para pedidos

```sql
CREATE INDEX IX_Orders_UserId
ON Orders(UserId);
GO
```

---

# 29. Datos Iniciales

## Roles

```sql
INSERT INTO Roles (Name)
VALUES
('Admin'),
('Customer');
GO
```

---

## Categorías Iniciales

```sql
INSERT INTO Categories (Name, Description)
VALUES
('Limpiadores', 'Productos para limpieza facial'),
('Serums', 'Serums hidratantes y reparadores'),
('Protectores Solares', 'Protección UV para la piel'),
('Hidratantes', 'Cremas y lociones hidratantes'),
('Mascarillas', 'Mascarillas faciales');
GO
```

---

# 30. Relaciones Principales

```text
Users 1:N Orders
Users 1:N Reviews
Users 1:N Addresses

Categories 1:N Products

Products 1:N ProductImages
Products 1:N Reviews

Orders 1:N OrderDetails

Cart 1:N CartItems

Promotions N:N Products
```

---

# 31. Reglas de Integridad

- Un producto debe pertenecer a una categoría.
- Un pedido debe pertenecer a un usuario.
- Un detalle de pedido debe pertenecer a un pedido.
- Un carrito debe pertenecer a un usuario.
- El stock no puede ser negativo.
- El precio debe ser mayor a 0.
- El rating debe estar entre 1 y 5.

---

# 32. Restricciones Recomendadas

## Restricción Rating

```sql
ALTER TABLE Reviews
ADD CONSTRAINT CK_Reviews_Rating
CHECK (Rating BETWEEN 1 AND 5);
GO
```

---

## Restricción Price

```sql
ALTER TABLE Products
ADD CONSTRAINT CK_Products_Price
CHECK (Price > 0);
GO
```

---

## Restricción Stock

```sql
ALTER TABLE Products
ADD CONSTRAINT CK_Products_Stock
CHECK (Stock >= 0);
GO
```

---

# 33. Recomendaciones para Entity Framework

## Configuración recomendada

- Usar Code First.
- Crear migraciones.
- Utilizar Fluent API.
- Configurar relaciones explícitas.
- Usar Repository Pattern.

---

## Comandos útiles

### Crear migración

```powershell
Add-Migration InitialCreate
```

---

### Aplicar migración

```powershell
Update-Database
```

---

# 34. Seguridad Recomendada

- Nunca guardar contraseñas en texto plano.
- Usar Password Hashing.
- Validar inputs.
- Usar parámetros SQL.
- Implementar roles y permisos.
- Proteger connection strings.

---

# 35. Escalabilidad Futura

La base de datos está preparada para:

- Aplicación móvil.
- API REST.
- IA para recomendaciones.
- Multi idioma.
- Multi moneda.
- Integración con pasarelas de pago.
- Integración logística.

---

# 36. Conclusión

La estructura de base de datos propuesta permite construir un sistema organizado, escalable y alineado con los requerimientos del negocio.

El diseño contempla:

- Gestión completa de usuarios.
- Gestión de productos.
- Compras online.
- Carrito.
- Pedidos.
- Pagos.
- Promociones.
- Blog informativo.
- Escalabilidad futura.

La arquitectura está preparada para integrarse correctamente con ASP.NET Core MVC y Entity Framework Core.