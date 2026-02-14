# 🏗️ Architecture du Product Service

## Vue d'Ensemble

Le Product Service suit une architecture en **4 couches** avec une séparation claire des préoccupations et des principes SOLID.

```
┌─────────────────────────────────────────┐
│          API Layer (Controllers)         │
│  ProductsController, CategoriesController│
└────────┬────────────────────────────────┘
         │ HTTP Requests
┌────────▼────────────────────────────────┐
│      Application Layer (Services)       │
│  ProductService, CategoryService        │
│  DTOs, Interfaces, Commands, Queries    │
└────────┬────────────────────────────────┘
         │ Business Logic
┌────────▼────────────────────────────────┐
│    Infrastructure Layer (Repositories)  │
│  ProductRepository, CategoryRepository   │
│  MongoDbContext, EventPublisher         │
└────────┬────────────────────────────────┘
         │ Data Access & Messaging
┌────────▼────────────────────────────────┐
│       Domain Layer (Entities)            │
│  Product, Category, Domain Events       │
└─────────────────────────────────────────┘
         │
    ┌────┴────┬──────────┐
    ▼         ▼          ▼
  MongoDB  RabbitMQ   External
                       APIs
```

## Détails des Couches

### 1. Domain Layer 🎯

**Responsabilité:** Contenir la logique métier pure et les entités

**Composants:**

- **Entities/**
  - `Product.cs` - Entité produit avec logique métier
  - `Category.cs` - Entité catégorie

- **Enums/**
  - `ProductStatus.cs` - Constantes de statut

- **Events/**
  - `DomainEvents.cs` - Événements de domaine publiés via RabbitMQ

**Règles:**
- Pas de dépendances externes (pas de NuGet, pas de base de données)
- Logique pure et testable
- Utilise des Value Objects et Aggregates

**Exemple d'entité avec logique:**
```csharp
public class Product
{
    public void DecrementStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0");
        
        if (Stock < quantity)
            throw new InvalidOperationException("Insufficient stock");
        
        Stock -= quantity;
        if (Stock == 0)
            Status = "OutOfStock";
    }
}
```

### 2. Application Layer 🔧

**Responsabilité:** Orchestrer les opérations métier et les cas d'usage

**Composants:**

- **DTOs/** - Data Transfer Objects
  - `ProductDto.cs` - DTO pour les réponses API
  - `CreateProductDto.cs` - DTO pour la création
  - `UpdateProductDto.cs` - DTO pour les mises à jour

- **Interfaces/**
  - `IProductRepository.cs` - Interface pour l'accès aux données
  - `IProductService.cs` - Interface pour la logique métier
  - `IEventPublisher.cs` - Interface pour la publication d'événements

- **Services/**
  - `ProductService.cs` - Logique métier des produits
  - `CategoryService.cs` - Logique métier des catégories

- **Commands/** (Optional pour CQRS futur)
  - `CreateProduct/`
  - `UpdateProduct/`
  - `DeleteProduct/`

- **Queries/** (Optional pour CQRS futur)
  - `GetProducts/`
  - `GetProductById/`
  - `SearchProducts/`

**Flux d'une requête:**
```
Controller → Service → Repository → Database
     ↓                                  ↓
   Mapping                          Mapping
(DTO → Entity)                    (Entity → DTO)
```

### 3. Infrastructure Layer 🔌

**Responsabilité:** Implémenter les interfaces de persistence et messaging

**Composants:**

- **Data/**
  - `MongoDbContext.cs` - Contexte MongoDB avec collections
  - `MongoDbSettings.cs` - Configuration MongoDB

- **Repositories/**
  - `ProductRepository.cs` - Implémentation IProductRepository
  - `CategoryRepository.cs` - Implémentation ICategoryRepository

- **Messaging/**
  - `EventPublisher.cs` - Implémentation IEventPublisher (RabbitMQ)
  - `RabbitMqConfiguration.cs` - Configuration RabbitMQ

**Pattern Repository:**
```csharp
public class ProductRepository : IProductRepository
{
    private readonly MongoDbContext _context;
    
    public async Task<Product> GetByIdAsync(string id)
    {
        return await _context.Products
            .Find(p => p.Id == id)
            .FirstOrDefaultAsync();
    }
}
```

### 4. API Layer 🌐

**Responsabilité:** Exposer les endpoints REST

**Composants:**

- **Controllers/**
  - `ProductsController.cs` - Endpoints pour les produits
  - `CategoriesController.cs` - Endpoints pour les catégories

- **Middleware/**
  - `ExceptionMiddleware.cs` - Gestion centralisée des exceptions
  - `LoggingMiddleware.cs` - Journalisation des requêtes

- `Program.cs` - Configuration de l'application

**Convention REST:**
```
GET    /api/products          → GetAllAsync()
GET    /api/products/:id      → GetByIdAsync(:id)
POST   /api/products          → CreateAsync(dto)
PUT    /api/products/:id      → UpdateAsync(:id, dto)
DELETE /api/products/:id      → DeleteAsync(:id)
```

## Flux de Données

### Création de Produit

```
1. Client
   ↓ POST /api/products {CreateProductDto}
2. ProductsController.CreateProduct()
   ↓ map DTO → Entity
3. ProductService.CreateProductAsync()
   ↓ business logic
4. ProductRepository.CreateAsync()
   ↓ MongoDB insert
5. Database
   ↓ returns created entity
6. EventPublisher.PublishAsync()
   ↓ RabbitMQ publish
7. Message Queue
   ↓ map Entity → DTO
8. Controller response
   ↓ 201 Created {ProductDto}
9. Client
```

## Patterns Utilisés

### 1. Repository Pattern
- Abstraction de l'accès aux données
- Interface: `IProductRepository`
- Implémentation: `ProductRepository`

### 2. Dependency Injection
- Enregistrement dans `Program.cs`
- Injection via constructeurs
- Pas de Service Locator

### 3. Data Transfer Objects (DTOs)
- Isolation entre couches
- Validation au niveau de l'API
- Mapping avec AutoMapper

### 4. Observer Pattern (Events)
- Publication d'événements RabbitMQ
- Découplage entre services
- Communication asynchrone

### 5. Middleware Pattern
- Gestion centralisée des exceptions
- Journalisation transversale
- Pipeline HTTP

## Dépendances et Versions

```
.NET                  10.0
MongoDB.Driver        2.24.0
RabbitMQ.Client       6.6.0
AutoMapper            13.0.0
MediatR               12.0.0 (optionnel pour CQRS)
xUnit                 2.6.0 (tests)
Moq                   4.20.0 (mocking)
```

## Gestion des Erreurs

### Exceptions de Domaine

```csharp
// InvalidOperationException - pour les règles métier violées
if (Stock < quantity)
    throw new InvalidOperationException("Insufficient stock");

// ArgumentException - pour les paramètres invalides
if (quantity <= 0)
    throw new ArgumentException("Quantity must be greater than 0");
```

### Gestion au niveau API

```csharp
public async Task<IActionResult> UpdateProduct(string id, UpdateProductDto dto)
{
    try
    {
        var result = await _productService.UpdateProductAsync(id, dto);
        return NoContent();
    }
    catch (KeyNotFoundException)
    {
        return NotFound(...);
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error updating product");
        return StatusCode(500, ...);
    }
}
```

## Sécurité

### Points à Implémenter

1. **Authentification**
   ```csharp
   [Authorize]
   public async Task<IActionResult> DeleteProduct(string id)
   ```

2. **Autorisation**
   ```csharp
   [Authorize(Roles = "Admin")]
   public async Task<IActionResult> DeleteProduct(string id)
   ```

3. **Validation**
   ```csharp
   [Range(0.01, 9999.99)]
   public decimal Price { get; set; }
   ```

4. **HTTPS**
   - Enforcer en production
   - Configuration dans `appsettings.json`

5. **CORS**
   ```csharp
   app.UseCors(builder => builder
       .AllowAnyOrigin()
       .AllowAnyMethod()
       .AllowAnyHeader());
   ```

## Performance

### Optimisations Implémentées

1. **Index MongoDB**
   ```csharp
   var indexModel = new CreateIndexModel<Product>(
       Builders<Product>.IndexKeys.Text(x => x.Name)
   );
   await Products.Indexes.CreateOneAsync(indexModel);
   ```

2. **Async/Await** - Opérations non-bloquantes
3. **Repository Pattern** - Requêtes optimisées

### Optimisations À Implémenter

1. **Redis Cache**
   - Cache produits fréquemment consultés
   - Invalidation intelligente

2. **Pagination**
   - Skip/Take sur les requêtes
   - Limitation des résultats

3. **Compression HTTP**
   - GZip pour les réponses

4. **Connection Pooling**
   - MongoDB et RabbitMQ

## Testing Strategy

### Unit Tests

```csharp
[Fact]
public async Task CreateProductAsync_ShouldPublishEvent()
{
    // Arrange
    var createDto = new CreateProductDto { ... };
    _mockRepository.Setup(x => x.CreateAsync(...))
        .ReturnsAsync(product);
    
    // Act
    var result = await _productService.CreateProductAsync(createDto);
    
    // Assert
    _mockEventPublisher.Verify(
        x => x.PublishAsync(It.IsAny<ProductCreatedEvent>()),
        Times.Once);
}
```

### Integration Tests (À Implémenter)

```csharp
[Collection("Integration Tests")]
public class ProductApiTests : IAsyncLifetime
{
    private readonly MongoDbFixture _mongoFixture;
    private readonly RabbitMqFixture _rabbitmqFixture;
    
    public async Task InitializeAsync()
    {
        await _mongoFixture.InitializeAsync();
        await _rabbitmqFixture.InitializeAsync();
    }
}
```

## Déploiement

### Development
- Docker Compose local
- Hot reload avec dotnet watch

### Staging/Production
- Docker container
- Kubernetes avec Helm
- CI/CD avec GitHub Actions ou Jenkins

## Monitoring

### Health Checks (À Implémenter)

```csharp
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = WriteResponse
});
```

### Métriques (À Implémenter)

- Prometheus pour les métriques
- Grafana pour la visualisation
- Application Insights pour APM

---

**Architecture Version:** 1.0
**Last Updated:** 2024-02-14
