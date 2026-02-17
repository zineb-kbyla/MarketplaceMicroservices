# 🚪 Configuration API Gateway comme Point d'Entrée Unique

## 📋 Nouvelle Architecture

```
┌─────────────────────────────────────────────────────┐
│                    FRONTEND / CLIENT                 │
└──────────────────────┬──────────────────────────────┘
                       │
                       ▼
        ┌──────────────────────────────┐
        │      API GATEWAY (YARP)      │
        │      (Port 5000)              │
        │  🚀 Point d'entrée unique    │
        └──────────────────────────────┘
                 │    │    │
      ┌──────────┘    │    └──────────┐
      │               │               │
      ▼               ▼               ▼
  ┌────────┐      ┌────────┐      ┌────────┐
  │Product │      │ Order  │      │  Rec.  │
  │ API    │      │  API   │      │  API   │
  │(5001)  │      │(5002)  │      │(8004)  │
  └────────┘      └────────┘      └────────┘
      │               │               │
      └───────────┬───┴───────────────┘
                  │
        ┌─────────┴──────────┐
        │   Infrastructure   │
        ├────────────────────┤
        │ • MongoDB          │
        │ • RabbitMQ         │
        │ • Neo4j            │
        └────────────────────┘
```

---

## 🛣️ Routes du Gateway

### 1️⃣ **Product Service**
```
GET    /api/products              → product-api:5001/api/products
GET    /api/products/{id}         → product-api:5001/api/products/{id}
POST   /api/products              → product-api:5001/api/products
PUT    /api/products/{id}         → product-api:5001/api/products/{id}
DELETE /api/products/{id}         → product-api:5001/api/products/{id}
GET    /api/products/category/{c} → product-api:5001/api/products/category/{c}
GET    /api/products/search       → product-api:5001/api/products/search
POST   /api/products/{id}/decrement-stock → product-api:5001/api/products/{id}/decrement-stock
```

### 2️⃣ **Order Service**
```
GET    /api/orders           → order-api:5002/api/orders
GET    /api/orders/{id}      → order-api:5002/api/orders/{id}
POST   /api/orders           → order-api:5002/api/orders
PUT    /api/orders/{id}      → order-api:5002/api/orders/{id}
DELETE /api/orders/{id}      → order-api:5002/api/orders/{id}
PUT    /api/orders/{id}/status → order-api:5002/api/orders/{id}/status
```

### 3️⃣ **Recommendation Service**
```
GET    /api/recommendations/{userId}    → recommendation-api:8004/api/recommendations/{userId}
GET    /api/recommendations/{userId}/similar-users → recommendation-api:8004/api/recommendations/{userId}/similar-users
POST   /api/recommendations/refresh     → recommendation-api:8004/api/recommendations/refresh
```

---

## 🐳 Docker Compose - Nouveau Setup

### Infrastructure
```yaml
services:
  mongodb              ← Port 27017 (Interne: Docker network)
  rabbitmq             ← Ports 5672, 15672 (Management)
  neo4j                ← Ports 7687, 7474

  api-gateway          ← Port 5000 (SEUL point d'entrée public)
  product-api          ← Port 5001 (Accessible via gateway)
  order-api            ← Port 5002 (Accessible via gateway)
  recommendation-api   ← Port 8004 (Accessible via gateway)
```

### Commandes Docker

```bash
# Build et lancer tout
docker-compose up --build

# Vérifier la santé
docker-compose ps

# Logs du gateway
docker logs api_gateway

# Logs d'un service
docker logs product_api
```

---

## ✅ Configuration des Services

### Product.API
- **Écoute internement sur:** `http://+:5001`
- **Via Gateway:** `http://localhost:5000/api/products`
- **Depuis Order.API:** `http://product-api:5001` (internal Docker DNS)

### Order.API
- **Écoute internement sur:** `http://+:5002`
- **Via Gateway:** `http://localhost:5000/api/orders`
- **Appel Product.API (interne):** `http://product-api:5001`

### Recommendation.API
- **Écoute internement sur:** `http://+:8004`
- **Via Gateway:** `http://localhost:5000/api/recommendations`
- **Appel Product.API (interne):** `http://product-api:5001`

---

## 🔧 Configuration YARP

**Fichier:** `APIGateway/appsettings.json` (Dev)
```json
{
  "ReverseProxy": {
    "Clusters": {
      "productCluster": {
        "Destinations": {
          "productService": { "Address": "http://localhost:5001" }
        }
      }
    }
  }
}
```

**Fichier:** `APIGateway/appsettings.Docker.json` (Docker)
```json
{
  "ReverseProxy": {
    "Clusters": {
      "productCluster": {
        "Destinations": {
          "productService": { "Address": "http://product-api:5001" }
        }
      }
    }
  }
}
```

---

## 🚀 Utilisation

### Mode Développement (Local)
```bash
# Lancer chaque service séparément
cd Product.API && dotnet run            # Port 5001
cd Order.API && dotnet run              # Port 5002
cd Recommendation.API && dotnet run     # Port 5003/8004
cd APIGateway && dotnet run             # Port 5000
```

**Test:**
```bash
# Via Gateway
curl http://localhost:5000/api/products
curl http://localhost:5000/api/orders
curl http://localhost:5000/api/recommendations/user123

# Direct (pour debug)
curl http://localhost:5001/api/products
curl http://localhost:5002/api/orders
```

### Mode Docker
```bash
docker-compose up --build

# Tests
curl http://localhost:5000/api/products
curl http://localhost:5000/api/orders

# Health check
curl http://localhost:5000/health
```

---

## 📊 Exemple: Flux d'une Requête

### 1️⃣ Client crée une commande
```
POST http://localhost:5000/api/orders
  ↓
API Gateway (route: /api/orders → orderCluster)
  ↓
Order.API:5002/api/orders (OrderService.CreateOrderAsync)
  ↓
Order.API appelle Product.API (inter-service)
  → http://product-api:5001/api/products/{id}
  ↓
Product.API retourne stock/prix
  ↓
Order.API crée commande
  ↓
Order.API publie OrderCreatedEvent → RabbitMQ
  ↓
Réponse au Client via Gateway
```

---

## 🔐 Avantages

✅ **Point d'entrée unique**
- Les clients ne connaissent que le Gateway (http://localhost:5000)
- Les services internes sont cachés

✅ **Services découplés**
- Inter-service communication via Docker DNS (product-api:5001)
- Pas d'exposition des ports internes

✅ **Monitoring centralisé**
- Logging du Gateway
- Health checks

✅ **Évolutivité**
- Ajouter load balancing
- Ajouter rate limiting
- Ajouter authentication

---

## 🛠️ Extension Future

### Rate Limiting
```csharp
builder.Services.AddRateLimiter(options => {
    options.AddSlidingWindowLimiter("default", configure => {
        configure.Window = TimeSpan.FromSeconds(10);
        configure.PermitLimit = 100;
    });
});
```

### Load Balancing
```json
{
  "LoadBalancingPolicy": "RoundRobin"
}
```

### Authentication/Authorization
```csharp
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(...);
```

---

## 📝 Fichiers modifiés

- ✅ `ProjetMarktplace_Net.sln` - Ajout APIGateway
- ✅ `docker-compose.yml` - Ajout gateway service
- ✅ `APIGateway/` - Nouveau projet YARP Gateway
- ✅ `APIGateway/appsettings.Docker.json` - Config Docker

