# 🧪 Guide de Test avec 2-3 Microservices

Ce guide montre comment tester un sous-ensemble de microservices ensemble sur Postman.

---

## 📋 Table des matières
1. [Configuration Sélective](#configuration-sélective)
2. [Scénarios de Test](#scénarios-de-test)
3. [Tests Postman](#tests-postman)
4. [Flux d'Intégration](#flux-dintégration)

---

## 🎯 Configuration Sélective

### Option 1: Démarrer Product.API + Order.API (sans Recommendation)

```bash
# Windows
docker-compose up -d api-gateway mongodb rabbitmq product-api order-api

# Vérifier le statut
docker-compose ps
```

**Services actifs:**
- APIGateway (port 5000) ✅
- Product.API (port 5001) ✅
- Order.API (port 5002) ✅
- MongoDB (port 27017) ✅
- RabbitMQ (port 5672) ✅
- Neo4j (port 7687) ❌ Inactif (Recommendation non utilisé)

---

### Option 2: Démarrer Product.API + Recommendation.API (sans Order)

```bash
docker-compose up -d api-gateway mongodb neo4j product-api recommendation-api
```

**Services actifs:**
- APIGateway (port 5000) ✅
- Product.API (port 5001) ✅
- Recommendation.API (port 5003) ✅
- MongoDB (port 27017) ✅
- Neo4j (port 7687) ✅
- RabbitMQ (port 5672) ❌ Inactif (Order non utilisé)

---

### Option 3: Tous les 3 microservices

```bash
docker-compose up -d
```

---

## 🔄 Flux d'Intégration

### Configuration 1: Product + Order

```
CLIENT → APIGateway (5000)
         ├─ GET /api/products → Product.API (5001)
         ├─ POST /api/orders → Order.API (5002)
         │   └─ Vérifie stock via Product.API
         │   └─ Publie événement OrderCreated → RabbitMQ
         └─ GET /api/orders/{id} → Order.API (5002)

Database:
  - MongoDB (Products + Orders)
  - RabbitMQ (Événements)
```

### Configuration 2: Product + Recommendation

```
CLIENT → APIGateway (5000)
         ├─ GET /api/products → Product.API (5001)
         ├─ POST /api/products/{id}/view → Product.API
         │   └─ Publie ProductViewedEvent → RabbitMQ
         ├─ GET /api/recommendations/{userId} → Recommendation.API (5003)
         └─ GET /api/recommendations/trending → Recommendation.API (5003)

Database:
  - MongoDB (Products)
  - Neo4j (Graphe de recommandations)
  - RabbitMQ (Événements)
```

---

## 📊 Tests Postman

### **Scénario 1: Product + Order**

#### 1️⃣ Récupérer tous les produits

```http
GET http://localhost:5000/api/products
```

**Réponse attendue (200):**
```json
{
  "success": true,
  "data": [
    {
      "id": "product1",
      "name": "Laptop",
      "price": 999.99,
      "stock": 10
    }
  ]
}
```

---

#### 2️⃣ Créer une commande

```http
POST http://localhost:5000/api/orders
Content-Type: application/json

{
  "userId": "user123",
  "userName": "John Doe",
  "items": [
    {
      "productId": "product1",
      "productName": "Laptop",
      "quantity": 1,
      "price": 999.99
    }
  ],
  "shippingAddress": {
    "street": "123 Main St",
    "city": "New York",
    "state": "NY",
    "country": "USA",
    "zipCode": "10001",
    "phoneNumber": "555-1234"
  },
  "paymentInfo": {
    "cardName": "John Doe",
    "cardNumber": "4111111111111111",
    "expiration": "12/25",
    "cvv": "123"
  }
}
```

**Réponse attendue (201):**
```json
{
  "success": true,
  "data": {
    "id": "order_abc123",
    "orderId": "ORD-001",
    "userId": "user123",
    "userName": "John Doe",
    "totalPrice": 999.99,
    "status": "Pending",
    "createdAt": "2026-02-17T10:30:00Z"
  }
}
```

---

#### 3️⃣ Récupérer une commande

```http
GET http://localhost:5000/api/orders/order_abc123
```

**Réponse attendue (200):**
```json
{
  "success": true,
  "data": {
    "id": "order_abc123",
    "orderId": "ORD-001",
    "userId": "user123",
    "status": "Pending",
    "items": [
      {
        "productId": "product1",
        "productName": "Laptop",
        "quantity": 1,
        "price": 999.99
      }
    ]
  }
}
```

---

#### 4️⃣ Mettre à jour le statut de la commande

```http
PUT http://localhost:5000/api/orders/order_abc123/status
Content-Type: application/json

{
  "status": "Shipped",
  "notes": "Order has been shipped"
}
```

**Réponse attendue (200):**
```json
{
  "success": true,
  "data": {
    "id": "order_abc123",
    "status": "Shipped",
    "updatedAt": "2026-02-17T10:35:00Z"
  }
}
```

---

#### 5️⃣ Vérifier les commandes de l'utilisateur

```http
GET http://localhost:5000/api/orders/user/user123
```

**Réponse attendue (200):**
```json
{
  "success": true,
  "data": [
    {
      "id": "order_abc123",
      "orderId": "ORD-001",
      "status": "Shipped",
      "totalPrice": 999.99
    }
  ]
}
```

---

### **Scénario 2: Product + Recommendation**

#### 1️⃣ Récupérer les produits

```http
GET http://localhost:5000/api/products
```

---

#### 2️⃣ Enregistrer une vue de produit

```http
POST http://localhost:5000/api/products/product1/view
Content-Type: application/json

{
  "userId": "user456"
}
```

**Réponse attendue (200):**
```json
{
  "success": true,
  "message": "Product view recorded"
}
```

---

#### 3️⃣ Obtenir les recommandations pour un utilisateur

```http
GET http://localhost:5000/api/recommendations/user456
```

**Réponse attendue (200):**
```json
{
  "success": true,
  "data": {
    "userId": "user456",
    "recommendations": [
      {
        "productId": "product2",
        "productName": "Monitor",
        "reason": "Similar to viewed products",
        "score": 0.85
      }
    ]
  }
}
```

---

#### 4️⃣ Obtenir les tendances du moment

```http
GET http://localhost:5000/api/recommendations/trending
```

**Réponse attendue (200):**
```json
{
  "success": true,
  "data": {
    "trendingProducts": [
      {
        "productId": "product1",
        "productName": "Laptop",
        "viewCount": 45,
        "score": 0.95
      }
    ]
  }
}
```

---

## 🧩 Tests d'Intégration avec Collection Postman

### Créer une Collection Postman

**1. Créer une nouvelle collection:** `Marketplace - 2-3 Services`

**2. Ajouter les variables d'environnement:**

| Variable | Valeur | Description |
|----------|--------|-------------|
| `base_url` | `http://localhost:5000` | URL APIGateway |
| `product_id` | `product1` | ID produit pour tests |
| `order_id` | (sera généré) | ID commande créée |
| `user_id` | `test_user_001` | ID utilisateur |

**3. Ajouter les requêtes:**

```postman
├── 📁 Product Service
│   ├── GET All Products
│   ├── GET Product by ID
│   └── POST Product View (pour Recommendation)
│
├── 📁 Order Service
│   ├── POST Create Order
│   ├── GET Order by ID
│   ├── GET User Orders
│   └── PUT Update Order Status
│
└── 📁 Recommendation Service
    ├── GET User Recommendations
    └── GET Trending Products
```

---

## ✅ Checklist de Vérification

### Configuration: Product + Order

- [ ] APIGateway répond sur port 5000
- [ ] MongoDB est actif et contient des produits
- [ ] RabbitMQ est actif (consommable sur port 15672)
- [ ] Créer une commande fonctionne
- [ ] La commande est stockée dans MongoDB
- [ ] L'événement OrderCreated est publié sur RabbitMQ
- [ ] Récupérer les commandes de l'utilisateur fonctionne

### Configuration: Product + Recommendation

- [ ] APIGateway répond sur port 5000
- [ ] MongoDB est actif et contient des produits
- [ ] Neo4j est actif (consommable sur port 7474)
- [ ] RabbitMQ est actif pour les événements
- [ ] Enregistrer une vue de produit fonctionne
- [ ] Les recommandations s'affichent pour l'utilisateur
- [ ] Les tendances s'affichent correctement

---

## 🐛 Dépannage

### Service ne répond pas

```bash
# Vérifier le statut
docker-compose ps

# Voir les logs
docker-compose logs api-gateway
docker-compose logs product-api
docker-compose logs order-api
docker-compose logs recommendation-api
```

### Port déjà utilisé

```bash
# Trouver le processus utilisant le port
netstat -ano | findstr :5000

# Tuer le processus (Windows)
taskkill /PID <PID> /F

# Tuer le processus (Linux/Mac)
lsof -ti:5000 | xargs kill -9
```

### MongoDB vide

```bash
# Se connecter à MongoDB
mongodb shell
use marketplace
db.products.insertOne({name: "Test Product", price: 99.99, stock: 10})
```

---

## 📝 Notes Importantes

1. **APIGateway est toujours activé** - Tous les tests passent par le gateway sur port 5000
2. **Les services internes communiquent directement** - Pour la verification de stock, etc.
3. **Les événements nécessitent RabbitMQ** - Important pour Product+Order et Product+Recommendation
4. **Neo4j nécessaire pour Recommendation** - Pour le graphe et calculs de recommandations
5. **MongoDB global** - Utilisé par Product et Order

---

## 🚀 Commandes Rapides

```bash
# Option 1: Product + Order
docker-compose up -d api-gateway mongodb rabbitmq product-api order-api

# Option 2: Product + Recommendation
docker-compose up -d api-gateway mongodb neo4j rabbitmq product-api recommendation-api

# Option 3: Tous les services
docker-compose up -d

# Arrêter tout
docker-compose down

# Logs en temps réel
docker-compose logs -f

# Redémarrer un service
docker-compose restart order-api
```

---

**Bon test! 🎉**
