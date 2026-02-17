# 🏪 Marketplace Microservices - Architecture Complète avec API Gateway

**Architecture microservices robuste et scalable pour une plateforme e-commerce moderne** utilisant **.NET 10.0**, avec un **API Gateway centralisé** (YARP), communication asynchrone (RabbitMQ), et multiples bases de données (MongoDB, Neo4j).

---

## 📑 Table des Matières

1. [Vue d'Ensemble](#vue-densemble)
2. [Architecture](#architecture)
3. [Outils et Technos](#outils-et-technos)
4. [Services](#services)
5. [API Gateway (YARP)](#api-gateway-yarp)
6. [Communication Inter-Services](#communication-inter-services)
7. [Installation](#installation)
8. [Endpoints API](#endpoints-api)
9. [Tests](#tests)
10. [Git & Deployment](#git--deployment)

---

## 🎯 Vue d'Ensemble

### Le Projet

Cette plateforme marketplace implémente une **architecture microservices complète** où:

✅ **Clients externes** ne voient QUE le **Gateway** (port 5000)  
✅ **Trois services indépendants** (Product, Order, Recommendation) déployables séparément  
✅ **Communication mixte**: synchrone (HTTP) + asynchrone (RabbitMQ)  
✅ **Scalabilité**: Chaque service peut avoir plusieurs instances  
✅ **Isolation des données**: Chaque service gère ses propres BD  
✅ **Clean Architecture**: Domain, Application, Infrastructure, API layers  

### Cas d'Usage Réels

**Chaîne de création de commande:**

```
1. Client → API Gateway /api/orders (POST)
2. Gateway → Order Service reçoit la commande
3. Order Service → RabbitMQ: "OrderCreated" event
4. Product Service écoute RabbitMQ: décrément le stock
5. Recommendation Service écoute RabbitMQ: met à jour profil utilisateur
6. Client ← Gateway retourne confirmation
```

---

## 🏗️ Architecture

### Diagramme Complet

```
┌────────────────────────────────────────────────────────────────────────┐
│                                                                         │
│              🌐 CLIENTS EXTERNES (Web, Mobile, Desktop)               │
│                     ↓                                                  │
│                     │ HTTP:5000                                        │
│                     ▼                                                  │
│   ┌──────────────────────────────────────────────────────────┐        │
│   │                                                          │        │
│   │         🚪 API GATEWAY (YARP)                          │        │
│   │     Routing • Logging • Health Checks • Timeouts       │        │
│   │                  Port 5000                              │        │
│   │                                                          │        │
│   └──────────────┬──────────────┬──────────────┬───────────┘        │
│                  │              │              │                     │
│                  │ HTTP         │ HTTP         │ HTTP                │
│                  │              │              │                     │
│        ┌─────────▼──────┐ ┌────▼──────────┐ ┌─▼────────────────┐    │
│        │                │ │               │ │                  │    │
│        │ Product API    │ │  Order API    │ │ Recommendation   │    │
│        │  (5001:5001)   │ │  (5002:5002)  │ │ (8004:8004)      │    │
│        │                │ │               │ │                  │    │
│        └────┬───────────┘ └──┬────────────┘ └────┬─────────────┘    │
│             │                │                   │                  │
│             └────────┬───────┴───────┬───────────┘                  │
│                      │               │                              │
│   ┌──────────────────▼───────────────▼──────────────────┐           │
│   │                                                     │           │
│   │  RabbitMQ Message Bus (5672)                       │           │
│   │  • ProductEvents • OrderEvents • RecommendEvents   │           │
│   │  Asynchronous Communication • Event Sourcing       │           │
│   │                                                     │           │
│   └─────────────────────────────────────────────────────┘           │
│             │                      │                                │
│             ▼                      ▼                                │
│   ┌──────────────────┐  ┌───────────────────┐                      │
│   │    MongoDB       │  │     Neo4j         │                      │
│   │    (27017)       │  │   (7687, 7474)    │                      │
│   │  • Products BD   │  │  • User Graphs    │                      │
│   │  • Orders BD     │  │  • Relationships  │                      │
│   └──────────────────┘  └───────────────────┘                      │
│                                                                         │
└────────────────────────────────────────────────────────────────────────┘
```

### Architecture Couches (Clean Architecture)

Chaque service suit cette structure:

```
Service.API/
│
├── Domain/                    # 🏛️ COUCHE MÉTIER
│   ├── Entities/              # Entités principales
│   ├── Enums/                 # Énumérations
│   ├── Events/                # Domain Events
│   └── ValueObjects/          # Value Objects
│
├── Application/               # 🚀 COUCHE APPLICATIVE
│   ├── Commands/              # CQRS - Mutations
│   ├── Queries/               # CQRS - Lectures
│   ├── DTOs/                  # Data Transfer Objects
│   ├── Interfaces/            # Contrats (IRepository, IService)
│   ├── MappingProfile.cs      # AutoMapper configuration
│   ├── Services/              # Business logic
│   └── Exceptions/            # Custom exceptions
│
├── Infrastructure/            # 🔧 COUCHE TECHNIQUE
│   ├── Data/                  # MongoDB context & config
│   ├── Repositories/          # Repository pattern impl
│   ├── Messaging/             # RabbitMQ publishers/consumers
│   ├── DataSeeder.cs          # Initialize sample data
│   └── ExternalServices/      # HTTP calls to other services
│
├── API/                       # 🌐 COUCHE PRÉSENTATION
│   ├── Controllers/           # REST endpoints
│   ├── Middleware/            # Custom middleware
│   └── Validators/            # Input validation
│
├── Tests/                     # 🧪 TESTS
│   ├── Unit/                  # Unit tests
│   └── Integration/           # Integration tests
│
├── Program.cs                 # Startup & DI container
├── appsettings.json           # Configuration
└── Dockerfile                 # Containerization
```

---

## 🛠️ Outils et Technos

### Backend Framework
- **ASP.NET Core 10.0** - Framework web moderne et haute performance
- **C# 12** - Langage de programmation robuste et type-safe

### Patterns & Architectures
| Pattern | Outil | Utilité |
|---------|-------|---------|
| **CQRS** | MediatR | Séparer Commands (mutations) et Queries (lectures) |
| **Repository** | Custom | Abstraction accès données |
| **Dependency Injection** | Service Collection | Gestion des dépendances |
| **Mapping DTO** | AutoMapper | Mapper entités ↔ DTOs |
| **Event-Driven** | RabbitMQ | Communication asynchrone |

### Bases de Données

#### MongoDB (NoSQL Document)
- **Driver**: `MongoDB.Driver` NuGet package
- **Services**: Product API, Order API
- **Databases**:
  - `products_db` → Products, Categories
  - `marketplace_order` → Orders, OrderItems
- **Avantages**: Flexible schema, scalabilité horizontale, agrégations puissantes
- **Port**: 27017

#### Neo4j (Graph Database)
- **Services**: Recommendation API
- **Utilité**: Modéliser relations utilisateurs pour recommandations IA
- **Database**: `neo4j`
- **Ports**: 7687 (Bolt protocol), 7474 (HTTP)
- **Client**: neo4j driver

### Message Bus (Asynchrone)
- **RabbitMQ** - Message broker AMQP
  - **Port**: 5672 (AMQP), 15672 (Management UI)
  - **Publisher**: Chaque service publie ses events
  - **Consumer**: Chaque service écoute les events pertinents
  - **Nuget**: `RabbitMQ.Client`

### API Gateway
- **YARP** (Yet Another Reverse Proxy)
  - **Package**: `Yarp.ReverseProxy`
  - **Fonctionnalités**:
    - Routage intelligent (basé sur path, méthode HTTP)
    - Load balancing
    - Health checks
    - Logging centralisé
    - Gestion de timeouts
  - **Configuration**: appsettings.json avec routes & clusters

### Containerization
- **Docker** - Containerization
- **Docker Compose** - Orchestration locale
  - Fichier: `docker-compose.yml`
  - Services: api-gateway, product-api, order-api, recommendation-api, mongodb, rabbitmq, neo4j

### Testing & Documentation
- **xUnit** - Framework de test .NET
- **Swagger/OpenAPI** - Documentation API auto-générée
- **Postman** - Collections fournie pour tester manuellement

---

## 📦 Services

### 1️⃣ Product Service (Port 5001)

**Responsabilité**: Gestion complet du catalogue de produits

```csharp
Product {
  id: ObjectId,
  name: string,
  description: string,
  category: string,
  price: decimal,
  stock: int,
  imageUrl: string,
  rating: double,
  reviewCount: int,
  status: "Available" | "OutOfStock" | "Discontinued",
  createdAt: DateTime,
  updatedAt: DateTime
}
```

**Events publiés sur RabbitMQ**:
- `ProductCreatedEvent` - Nouveau produit
- `ProductUpdatedEvent` - Mis à jour
- `StockChangedEvent` - Stock modifié
- `ProductDeletedEvent` - Supprimé

**Technos**:
- MongoDB (Products, Categories collections)
- RabbitMQ (Publisher)
- MediatR (Commands/Queries)

---

### 2️⃣ Order Service (Port 5002)

**Responsabilité**: Gestion des commandes, du paiement, et du suivi

```csharp
Order {
  id: ObjectId,
  userId: string,
  orderDate: DateTime,
  status: "Pending" | "Confirmed" | "Processing" | "Shipped" | "Delivered" | "Cancelled",
  items: OrderItem[],
  total: decimal,
  shippingAddress: string,
  paymentInfo: {
    cardNumber: string,
    cardHolder: string,
    cvv: string,
    expiryDate: string
  },
  tracking: {
    trackingNumber: string,
    estimatedDelivery: DateTime
  }
}
```

**Fonctionnalités**:
- Création de commandes
- Vérification stock auprès du Product Service (HTTP)
- Gestion statuts
- Paiements
- Suivi expédition

**Events publiés**:
- `OrderCreatedEvent`
- `OrderStatusChangedEvent`
- `OrderCancelledEvent`

**Events écoutés**:
- `StockChangedEvent` (de Product Service)

---

### 3️⃣ Recommendation Service (Port 8004)

**Responsabilité**: Recommandations personnalisées basées sur IA

```csharp
UserProfile {
  userId: string,
  viewedProducts: string[],
  purchasedProducts: string[],
  preferences: {
    categories: string[],
    priceRange: { min, max },
    brands: string[]
  },
  similarUsers: string[]  // Via Neo4j graph
}
```

**Fonctionnalités**:
- Profilitaire utilisateur
- Recommandations par collaborative filtering
- Utilisateurs similaires via graphe Neo4j
- Algorithmes d'apprentissage

**Events écoutés**:
- `ProductViewedEvent` (de Product Service)
- `OrderCreatedEvent` (de Order Service)

---

## 🚪 API Gateway (YARP)

### Pourquoi un Gateway?

**AVANT (sans Gateway):**
```
Client → connaît Product Service (5001), Order Service (5002), etc.
Problèmes:
- ❌ Clients doivent connaître tous les ports internes
- ❌ Pas de point centralisé pour logging/monitoring
- ❌ Sécurité: services exposés directement
- ❌ Impossible de changer ports sans casser les clients
```

**APRÈS (avec Gateway):**
```
Client → API Gateway (5000) → Route vers le bon service
Avantages:
✅ Point d'entrée unique (5000)
✅ Logging/Monitoring centralisé
✅ Sécurité renforcée (Firebase, OAuth2 peuvent se greffer)
✅ Services peuvent bouger de port sans impacter clients
✅ Load balancing & health checks
```

### Configuration Routage

```json
// appsettings.Docker.json
"ReverseProxy": {
  "Routes": {
    "productRoute": {
      "ClusterId": "productCluster",
      "Match": {
        "Path": "/api/products/{**catch-all}",
        "Methods": ["GET", "POST", "PUT", "DELETE"]
      },
      "Timeout": "00:00:30"
    },
    "orderRoute": {
      "ClusterId": "orderCluster",
      "Match": {
        "Path": "/api/orders/{**catch-all}",
        "Methods": ["GET", "POST", "PUT", "DELETE"]
      },
      "Timeout": "00:00:30"
    }
  },
  "Clusters": {
    "productCluster": {
      "Destinations": {
        "productService": {
          "Address": "http://product-api:5001"
        }
      }
    }
  }
}
```

### Middleware Stack

```csharp
app.UseHttpsRedirection();      // HTTPS redirect
app.UseCors("AllowAll");        // CORS
app.UseMiddleware<GatewayLoggingMiddleware>(); // Custom logging
app.UseRouting();               // Routing
app.UseRequestTimeouts();       // ⚠️ IMPORTANT pour YARP
app.UseAuthorization();         // Auth
app.MapControllers();           // Controllers
app.MapHealthChecks("/health"); // Health endpoint
app.MapReverseProxy();          // YARP reverse proxy
```

### Health Checks

```bash
# Tester la santé du gateway
curl http://localhost:5000/health

# Réponse:
{
  "status": "Healthy"
}
```

---

## 🔄 Communication Inter-Services

### 1️⃣ Synchrone (HTTP)

**Exemple**: Order Service appelle Product Service

```csharp
// OrderService.cs
public async Task<bool> VerifyStockAsync(string productId, int quantity)
{
    using (var client = new HttpClient())
    {
        // ⚠️ EN DOCKER: product-api:5001
        // ⚠️ EN LOCAL: localhost:5001
        var response = await client.GetAsync(
            $"http://product-api:5001/api/products/{productId}/stock"
        );
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<Product>(json);
            return product.Stock >= quantity;
        }
        
        return false;
    }
}
```

**Avantages**: Réponse immédiate  
**Inconvénients**: Couplage fort, si Product Service down → Order Service fail

### 2️⃣ Asynchrone (RabbitMQ Events)

**Exemple**: Product Service publie un événement, Order Service écoute

#### Publisher (Product Service)

```csharp
// ProductService.cs
public async Task PublishStockChangedEvent(string productId, int newStock)
{
    var connection = _connectionFactory.CreateConnection();
    var channel = connection.CreateModel();
    
    // Déclarer l'exchange
    channel.ExchangeDeclare(
        exchange: "products.exchange",
        type: ExchangeType.Topic,
        durable: true
    );
    
    var stockEvent = new StockChangedEvent
    {
        ProductId = productId,
        NewStock = newStock,
        Timestamp = DateTime.UtcNow
    };
    
    var message = JsonConvert.SerializeObject(stockEvent);
    var body = Encoding.UTF8.GetBytes(message);
    
    channel.BasicPublish(
        exchange: "products.exchange",
        routingKey: "stock.changed",
        basicProperties: null,
        body: body
    );
}
```

#### Consumer (Order Service)

```csharp
// OrderConsumer.cs - Écoute les événements
public class StockChangedEventConsumer : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connection = _connectionFactory.CreateConnection();
        var channel = connection.CreateModel();
        
        // S'abonner à l'événement
        channel.ExchangeDeclare("products.exchange", ExchangeType.Topic, durable: true);
        
        var queue = channel.QueueDeclare().QueueName;
        channel.QueueBind(queue, "products.exchange", "stock.changed");
        
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            var stockEvent = JsonConvert.DeserializeObject<StockChangedEvent>(message);
            
            // Traiter l'événement
            await _orderService.HandleStockChangeAsync(stockEvent);
        };
        
        channel.BasicConsume(queue, autoAck: true, consumer: consumer);
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
```

**Avantages**: Découplage des services, résilience  
**Inconvénients**: Latence, pas de garantie immédiate

### Diagramme Flux Commande

```
1. Client POST /api/orders
           ↓
2. Gateway route vers Order Service:5002
           ↓
3. OrderService.CreateOrder()
           ├─ HTTP://product-api:5001 ← Vérifier stock (SYNC)
           └─ RabbitMQ.Publish("OrderCreatedEvent") (ASYNC)
           ↓
4. Product Service écoute & décrément stock
5. Recommendation Service écoute & update profil utilisateur
           ↓
6. Gateway retourne 200 OK au Client
```

---

## 💻 Installation

### Prérequis

- ✅ Docker & Docker Compose
- ✅ .NET 10.0 SDK (optionnel si utilisation Docker)
- ✅ Git
- ✅ Postman (optionnel, pour tester)

### Étape 1: Cloner le Repo

```bash
git clone https://github.com/zineb-kbyla/MarketplaceMicroservices.git
cd ProjetMarktplace_Net
```

### Étape 2: Lancer avec Docker Compose

```bash
# Arrêter anciens containers
docker-compose down

# Lancer tous les services (reconstruit les images)
docker-compose up --build -d

# Vérifier les services
docker-compose ps
```

**Output attendu:**
```
NAME                   IMAGE                      STATUS
api_gateway            projetmarktplace_net-...   Up 30s (health: starting)
product_api            projetmarktplace_net-...   Up 30s
order_api              projetmarktplace_net-...   Up 30s
recommendation_api     projetmarktplace_net-...   Up 30s
marketplace_mongodb    mongo:7.0                  Up 30s (healthy)
marketplace_rabbitmq   rabbitmq:3.13-management  Up 30s (healthy)
marketplace_neo4j      neo4j:5.23                 Up 30s (healthy)
```

### Étape 3: Vérifier la Santé

```bash
# Health check du gateway
curl http://localhost:5000/health

# Devrait retourner:
# {"status":"Healthy"}
```

### Étape 4: Test Rapide

```bash
# Récupérer les produits (via Gateway!)
curl http://localhost:5000/api/products | jq '.'

# Récupérer les commandes
curl http://localhost:5000/api/orders | jq '.'
```

### Débogage

#### Voir les logs
```bash
# Logs du gateway
docker logs api_gateway -f

# Logs du product service
docker logs product_api -f

# Tous les logs
docker-compose logs -f
```

#### Accéder à RabbitMQ UI
```
Navigateur: http://localhost:15672
Username: guest
Password: guest
```

#### Accéder à Neo4j Browser
```
Navigateur: http://localhost:7474
Username: neo4j
Password: password
```

#### MongoDB
```bash
# Connecter avec mongosh
docker exec -it marketplace_mongodb mongosh -u root -p password

# Lister databases
> show dbs

# Utiliser products_db
> use products_db
> db.products.find() # Voir tous les produits
```

---

## 📡 Endpoints API

### 🌐 Via le Gateway (Utiliser PORT 5000)

#### Product API

```
GET    /api/products              # Lister tous les produits
GET    /api/products/{id}         # Détails d'un produit
POST   /api/products              # Créer un produit
PUT    /api/products/{id}         # Modifier un produit
DELETE /api/products/{id}         # Supprimer
GET    /api/products/search?q=iphone  # Chercher
GET    /api/products/{id}/stock   # Vérifier stock
POST   /api/products/{id}/decrement-stock # Décrémenter stock
```

**Exemple POST (créer produit):**
```bash
curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "iPhone 16 Pro",
    "description": "Latest Apple flagship",
    "category": "Electronics",
    "price": 1099.99,
    "stock": 50,
    "imageUrl": "https://example.com/iphone16.jpg"
  }'
```

#### Order API

```
GET    /api/orders                      # Lister les commandes
GET    /api/orders?userId={userId}     # Commandes d'un utilisateur
GET    /api/orders/{id}                # Détails d'une commande
POST   /api/orders                     # Créer une commande
PUT    /api/orders/{id}/status         # Modifier le statut
DELETE /api/orders/{id}                # Annuler une commande
GET    /api/orders/{id}/tracking       # Suivi
```

**Exemple POST (créer commande):**
```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user123",
    "items": [
      {
        "productId": "69936b01c100e42905809ac3",
        "quantity": 2,
        "price": 999.99
      }
    ],
    "shippingAddress": "123 Main St, NYC",
    "paymentInfo": {
      "cardNumber": "4111111111111111",
      "cardHolder": "John Doe",
      "cvv": "123",
      "expiryDate": "12/25"
    }
  }'
```

#### Recommendation API

```
GET /api/recommendations/{userId}              # Recommandations pour utilisateur
GET /api/recommendations/{userId}/similar-users # Utilisateurs similaires
POST /api/recommendations/refresh               # Rafraîchir algoritme
```

#### Gateway Health

```
GET /health  # Santé du gateway
```

---

## 🧪 Tests

### Tests Manuel via Postman

1. Importer les collections JSON:
   - `Order-Service.postman_collection.json`
   - `Product-Service.postman_collection.json`

2. Exécuter les requêtes

### Tests via cURL

```bash
# 1. Lister les produits
curl http://localhost:5000/api/products | jq '.'

# 2. Créer un produit
PRODUCT=$(curl -s -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Product",
    "price": 99.99,
    "stock": 100,
    "category": "Test"
  }')

PRODUCT_ID=$(echo $PRODUCT | jq -r '.id')
echo "Created Product: $PRODUCT_ID"

# 3. Créer une commande
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d "{
    \"userId\": \"user123\",
    \"items\": [{\"productId\": \"$PRODUCT_ID\", \"quantity\": 1, \"price\": 99.99}],
    \"shippingAddress\": \"123 Main St\"
  }"

# 4. Vérifier le stock après commande
curl http://localhost:5000/api/products/$PRODUCT_ID | jq '.stock'
```

### Tests avec GitHub Actions (CI/CD)

Voir `.github/workflows/` pour automatiser les tests et déploiement.

---

## 📤 Git & Deployment

### Étape 1: Commiter Changes

```bash
# Voir les changements
git status

# Ajouter tous les fichiers
git add .

# Commiter
git commit -m "feat: Add complete API Gateway with YARP routing"

# Ou messages spécifiques:
git commit -m "feat: Implement YARP API Gateway
- Centralized routing for all microservices
- Health checks and request timeouts
- Logging middleware for monitoring

docs: Update README with comprehensive guide
- Architecture explanation
- Service details
- API Gateway configuration
- Communication patterns (sync/async)
- Setup instructions"
```

### Étape 2: Pousser vers GitHub

```bash
# Vérifier la branche courante
git branch

# Push vers la branche courante
git push origin zineb

# Ou créer nouvelle branche si besoin:
git checkout -b feature/api-gateway
git push origin feature/api-gateway
```

### Étape 3: Lancer une Pull Request

Sur GitHub:
1. Aller à **Pull requests**
2. Cliquer **"New pull request"**
3. Sélectionner `feature/api-gateway` → `zineb`
4. Ajouter titre et description
5. Cliquer **"Create pull request"**

### Déploiement Docker

```bash
# Construire les images
docker-compose build

# Pousser vers Docker Hub (optionnel)
docker tag projetmarktplace_net-api-gateway:latest username/marketplace-gateway:latest
docker push username/marketplace-gateway:latest

# Déployer (simple)
docker-compose up -d
```

### Déploiement Azure / Kubernetes

```bash
# Azure Container Registry
az acr build --registry myregistry --image marketplace-gateway:latest .

# Kubernetes
kubectl apply -f k8s/api-gateway-deployment.yaml
kubectl apply -f k8s/product-service-deployment.yaml
kubectl apply -f k8s/order-service-deployment.yaml
```

---

## 📚 Fichiers Importants

| Fichier | Utilité |
|---------|---------|
| `docker-compose.yml` | Orchestration tous les services |
| `ProjetMarktplace_Net.sln` | Solution Visual Studio |
| `APIGateway/Program.cs` | Configuration du Gateway |
| `APIGateway/appsettings.json` | Routes du Gateway |
| `APIGateway/appsettings.Docker.json` | Config pour Docker |
| `Order.API/Program.cs` | Configuration Order Service |
| `Product.API/Program.cs` | Configuration Product Service |
| `Recommendation.API/Program.cs` | Configuration Recommendation Service |

---

## 📊 Structure Global

```
ProjetMarktplace_Net/
├── APIGateway/
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Docker.json
│   ├── appsettings.Development.json
│   ├── Dockerfile
│   └── APIGateway.csproj
├── Product.API/
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   ├── API/
│   ├── Tests/
│   ├── Program.cs
│   ├── Dockerfile
│   └── Product.API.csproj
├── Order.API/
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   ├── API/
│   ├── Tests/
│   ├── Program.cs
│   ├── Dockerfile
│   └── Order.API.csproj
├── Recommendation.API/
│   ├── Application/
│   ├── API/
│   ├── Program.cs
│   ├── Dockerfile
│   └── Recommendation.API.csproj
├── docker-compose.yml
├── ProjetMarktplace_Net.sln
├── README.md (ce fichier)
└── GATEWAY_TESTING.md
```

---

## 🔍 Troubleshooting Guide

### ❌ "Connection to product-api:5001 failed"

**Cause**: Services pas connectés au même réseau Docker  
**Solution**:
```bash
# Vérifier le réseau
docker network ls
docker network inspect projetmarktplace_net_marketplace_network

# Reconstruire
docker-compose down
docker-compose up --build -d
```

### ❌ "MongoDB connection timeout"

**Solution**:
```bash
# Vérifier MongoDB
docker logs marketplace_mongodb

# Attendre que MongoDB soit prêt
docker-compose exec mongodb mongosh -u root -p password admin
```

### ❌ "RabbitMQ unhealthy"

**Solution**:
```bash
# Vérifier RabbitMQ
docker logs marketplace_rabbitmq

# Accéder au Management UI
Browser: http://localhost:15672
Credentials: guest/guest
```

### ❌ "Gateway returns 500"

**Solution**:
```bash
# Voir les logs du gateway
docker logs api_gateway

# Vérifier appsettings.json routes
docker exec api_gateway cat /app/appsettings.json
```

---

## 📈 Métriques et Monitoring

### APM (Application Performance Monitoring)

Pour la production, intégrer:
- **Application Insights** (Azure)
- **Prometheus** & **Grafana**
- **ELK Stack** (Elasticsearch, Logstash, Kibana)
- **Jaeger** (Distributed Tracing)

---

## 🎓 Concepts Clés Expliqués (Pour l'Encadrant)

### 1. Microservices vs Monolithe

**Monolithe**: Tout dans une seule app → difficile à scaler  
**Microservices**: Services indépendants → chacun peut scaler  

### 2. API Gateway Pattern

**Problème**: Plusieurs services, chacun sur port différent  
**Solution**: Gateway unique route les requêtes  
**Bénéfice**: Clients ne voient qu'un port, sécurité renforcée  

### 3. CQRS Pattern

**Commands** = Actions qui changent l'état (Create, Update, Delete)  
**Queries** = Lectures (Get, List)  
**Benefit**: Optimiser séparément lecture vs écriture  

### 4. Event-Driven Architecture

**Avantage**: Services découplés, peuvent fonctionner indépendamment  
**Exemple**: Order Service publie "OrderCreated", Product Service écoute et décrément stock  
**RabbitMQ**: Bus de messages qui gère la communication  

### 5. Clean Architecture

**Règle**: Dépendances pointent VERS le centre  
```
API layer → Application layer → Domain layer
              ↑
Infrastructure layer
```

---

## 📞 Support & Questions

Pour toute question, vérifier:
1. `docker-compose ps` - Services running?
2. `docker logs` - Erreurs dans les logs?
3. Endpoints via `curl http://localhost:5000/api/products`
4. RabbitMQ UI: `http://localhost:15672`

---

## 📄 License

MIT License - Libre d'utilisation

---

## ✨ Résumé pour l'Encadrant

**Ce projet démontre:**

✅ Architecture microservices complète et fonctionnelle  
✅ API Gateway centralisé (YARP) comme point d'entrée unique  
✅ Communication mixte (HTTP synchrone + RabbitMQ asynchrone)  
✅ Clean Architecture avec Domain-Driven Design  
✅ Containerization complète avec Docker Compose  
✅ CQRS pattern avec MediatR  
✅ Repository Pattern pour accès données  
✅ Trois bases de données différentes (MongoDB, Neo4j)  
✅ Event-Driven Architecture  
✅ Logging centralisé  
✅ Health Checks  
✅ Code testable et maintenable  

**Scalabilité:**
- Chaque service peut avoir plusieurs instances
- Gateway distribue les requêtes
- RabbitMQ permet découplage
- Chaque service gère ses données

**Sécurité:**
- Services internes pas exposés
- Tout passe par le Gateway
- Timeouts et health checks

---

**Créé avec ❤️ pour démontrer une architecture microservices moderne en .NET 10.0**
