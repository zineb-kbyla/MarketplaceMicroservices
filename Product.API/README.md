# 📦 Product Service - Marketplace Microservice

Service de gestion du catalogue de produits pour une plateforme de marketplace utilisant ASP.NET Core, MongoDB, et RabbitMQ.

## Architecture

L'application suit une architecture en 4 couches:

- **Domain Layer**: Entités, énumérations et événements de domaine
- **Application Layer**: DTOs, Services, Interfaces et logique métier
- **Infrastructure Layer**: Accès aux données (MongoDB), Messaging (RabbitMQ)
- **API Layer**: Contrôleurs REST, Middlewares

## Stack Technique

- **Framework**: ASP.NET Core 10.0
- **Base de données**: MongoDB
- **Message Broker**: RabbitMQ
- **Mapping**: AutoMapper
- **Tests**: xUnit, Moq

## Prérequis

- .NET 10.0 SDK
- Docker & Docker Compose (pour MongoDB et RabbitMQ)

## Installation et Démarrage

### 1. Démarrer les services dépendants

```bash
docker-compose up -d
```

Cela démarrera:
- **MongoDB** sur `mongodb://localhost:27017`
- **RabbitMQ** sur `localhost:5672` (Management UI sur http://localhost:15672)

### 2. Restaurer les dépendances

```bash
cd Product.API
dotnet restore
```

### 3. Compiler le projet

```bash
dotnet build
```

### 4. Lancer l'application

```bash
dotnet run
```

L'API sera disponible sur: `https://localhost:7243` ou `http://localhost:5007`

**Documentation de l'API** :
- **Interface Scalar UI** : `http://localhost:5007/scalar/v1`
- **Spécification OpenAPI (JSON)** : `http://localhost:5007/openapi/v1.json`

## API Endpoints

Base URL (local):

- http://localhost:5007
- https://localhost:7243

### Produits
| Méthode | Route | Description |
|---------|-------|-------------|
| GET | `/api/products` | Récupérer tous les produits |
| GET | `/api/products/{id}` | Récupérer un produit par ID |
| GET | `/api/products/category/{category}` | Récupérer les produits d'une catégorie |
| GET | `/api/products/search?q={query}` | Rechercher des produits |
| POST | `/api/products` | Créer un nouveau produit |
| PUT | `/api/products/{id}` | Mettre à jour un produit |
| DELETE | `/api/products/{id}` | Supprimer un produit |
| POST | `/api/products/{id}/decrement-stock` | Décrémenter le stock |

### Catégories
| Méthode | Route | Description |
|---------|-------|-------------|
| GET | `/api/categories` | Récupérer toutes les catégories |
| GET | `/api/categories/{id}` | Récupérer une catégorie par ID |
| POST | `/api/categories` | Créer une nouvelle catégorie |
| PUT | `/api/categories/{id}` | Mettre à jour une catégorie |
| DELETE | `/api/categories/{id}` | Supprimer une catégorie |

## Tests Postman (exemples)

Headers a utiliser:

- Content-Type: application/json

### Produits

POST `/api/products`

```json
{
  "name": "iPhone 15 Pro",
  "description": "Latest flagship smartphone",
  "category": "Electronics",
  "price": 999.99,
  "stock": 50,
  "imageUrl": "https://example.com/iphone.jpg"
}
```

PUT `/api/products/{id}`

```json
{
  "name": "iPhone 15 Pro Max",
  "description": "Updated description",
  "category": "Electronics",
  "price": 1099.99,
  "stock": 45,
  "imageUrl": "https://example.com/iphone-pro-max.jpg"
}
```

POST `/api/products/{id}/decrement-stock`

```json
{
  "quantity": 5
}
```

GET `/api/products/search?q=iphone`

### Categories

POST `/api/categories`

```json
{
  "name": "Electronics",
  "description": "Phones, laptops, accessories",
  "imageUrl": "https://example.com/electronics.jpg"
}
```

PUT `/api/categories/{id}`

```json
{
  "name": "Electronics",
  "description": "Updated category description",
  "imageUrl": "https://example.com/electronics-v2.jpg"
}
```

## Exemples de Requêtes

### Créer un produit

```bash
curl -X POST http://localhost:5007/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "iPhone 15 Pro",
    "description": "Latest flagship smartphone",
    "category": "Electronics",
    "price": 999.99,
    "stock": 50,
    "imageUrl": "https://example.com/iphone.jpg"
  }'
```

### Rechercher des produits

```bash
curl http://localhost:5007/api/products/search?q=iphone
```

### Décrémenter le stock

```bash
curl -X POST http://localhost:5007/api/products/607f1f77bcf86cd799439011/decrement-stock \
  -H "Content-Type: application/json" \
  -d '{"quantity": 5}'
```

## Événements Publiés

Le service publie les événements suivants via RabbitMQ:

- **ProductCreatedEvent**: Quand un produit est créé
- **ProductUpdatedEvent**: Quand un produit est mis à jour
- **ProductViewedEvent**: Quand un produit est consulté
- **StockUpdatedEvent**: Quand le stock change

## Configuration

### appsettings.json

```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "products_db"
  },
  "RabbitMq": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "ExchangeName": "products.exchange"
  }
}
```

## Tests Unitaires

Exécuter tous les tests:

```bash
dotnet test
```

Exécuter les tests avec couverture:

```bash
dotnet test /p:CollectCoverage=true
```

## Structure du Projet

```
Product.API/
├── Domain/
│   ├── Entities/
│   │   ├── Product.cs
│   │   └── Category.cs
│   ├── Enums/
│   │   └── ProductStatus.cs
│   └── Events/
│       └── DomainEvents.cs
│
├── Application/
│   ├── DTOs/
│   │   └── ProductDtos.cs
│   ├── Interfaces/
│   │   └── IRepositories.cs
│   ├── Services/
│   │   └── ProductService.cs
│   ├── Commands/
│   └── Queries/
│
├── Infrastructure/
│   ├── Data/
│   │   ├── MongoDbContext.cs
│   │   └── MongoDbSettings.cs
│   ├── Repositories/
│   │   ├── ProductRepository.cs
│   │   └── CategoryRepository.cs
│   └── Messaging/
│       └── EventPublisher.cs
│
├── API/
│   ├── Controllers/
│   │   ├── ProductsController.cs
│   │   └── CategoriesController.cs
│   ├── Middleware/
│   │   └── AppMiddleware.cs
│   └── Program.cs
│
└── Tests/
    └── Unit/
        └── ProductServiceTests.cs
```

## Docker

### Construire l'image Docker

Depuis la racine du projet (où se trouve le fichier `.sln`):

```bash
docker build -f Product.API/Dockerfile -t product-service:latest .
```

### Lancer le conteneur

```bash
docker run -p 8081:8080 -e MongoDb__ConnectionString="mongodb://host.docker.internal:27017" -e RabbitMq__HostName="host.docker.internal" product-service:latest
```

L'API sera accessible sur:
- **Base URL**: `http://localhost:8081`
- **Documentation Scalar**: `http://localhost:8081/scalar/v1`
- **Spécification OpenAPI**: `http://localhost:8081/openapi/v1.json`

> **Note**: 
> - Le port 8081 sur votre machine locale sera mappé vers le port 8080 du conteneur
> - `host.docker.internal` permet au conteneur d'accéder aux services (MongoDB, RabbitMQ) qui tournent sur votre machine hôte
> - Si vous utilisez Docker Compose, ces variables d'environnement ne sont pas nécessaires

## Intégration avec l'API Gateway

Ce service s'intègre avec un API Gateway (YARP) qui expose les endpoints publics. Le gateway est responsable du routage, de l'authentification et de la limitation de débit.

## Prochaines Étapes

- [ ] Implémenter les Queries avec CQRS si nécessaire
- [ ] Ajouter la validation des DTOs avec FluentValidation
- [ ] Implémenter la mise en cache avec Redis
- [ ] Ajouter des tests d'intégration complètes
- [ ] Configurer les logs structurés avec Serilog
- [ ] Implémenter la pagination
- [ ] Ajouter les métriques Prometheus

## Licence

Ce projet est part d'un exercice de formation.
