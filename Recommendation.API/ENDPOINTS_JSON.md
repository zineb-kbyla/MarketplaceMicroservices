# 📋 Endpoints & JSON pour Recommendation API

## 🔗 Base URL
```
http://localhost:5003/api/recommendations
```

---

## 1️⃣ GET - Obtenir les recommandations personnalisées

### Endpoint
```
GET http://localhost:5003/api/recommendations/user-123?limit=10
```

### Paramètres
- `userId` (path): ID utilisateur (requis)
- `limit` (query): Nombre max de recommandations (optionnel, défaut: 10)

### Réponse (200 OK)
```json
[
  {
    "productId": "prod-001",
    "name": "Laptop Dell XPS 15",
    "category": "Electronics",
    "price": 1299.99,
    "imageUrl": "https://example.com/laptop.jpg",
    "rating": 4.8,
    "score": 0.92,
    "reason": "15 utilisateurs similaires ont acheté ce produit",
    "confidence": 0.85
  }
]
```

---

## 2️⃣ GET - Produits similaires

### Endpoint
```
GET http://localhost:5003/api/recommendations/similar/prod-001?limit=5
```

### Paramètres
- `productId` (path): ID produit (requis)
- `limit` (query): Nombre max de produits similaires (optionnel, défaut: 5)

### Réponse (200 OK)
```json
[
  {
    "productId": "prod-003",
    "name": "Laptop HP Pavilion 15",
    "category": "Electronics",
    "price": 899.99,
    "similarityScore": 0.89,
    "reason": "Same category - High-performance laptop"
  },
  {
    "productId": "prod-004",
    "name": "Laptop ASUS ROG",
    "category": "Electronics",
    "price": 1499.99,
    "similarityScore": 0.85,
    "reason": "Same category - Gaming laptop"
  }
]
```

---

## 3️⃣ GET - Produits tendance

### Endpoint
```
GET http://localhost:5003/api/recommendations/trending?days=7&limit=10
```

### Paramètres
- `days` (query): Nombre de jours à analyser (optionnel, défaut: 7)
- `limit` (query): Nombre max de produits (optionnel, défaut: 10)

### Réponse (200 OK)
```json
[
  {
    "productId": "prod-005",
    "name": "iPhone 15 Pro",
    "category": "Electronics",
    "price": 999.99,
    "recentPurchases": 145,
    "trendScore": 1.0
  },
  {
    "productId": "prod-006",
    "name": "AirPods Pro",
    "category": "Accessories",
    "price": 249.99,
    "recentPurchases": 98,
    "trendScore": 0.98
  }
]
```

---

## 4️⃣ GET - Historique utilisateur

### Endpoint
```
GET http://localhost:5003/api/recommendations/history/user-123?limit=20
```

### Paramètres
- `userId` (path): ID utilisateur (requis)
- `limit` (query): Nombre max d'éléments (optionnel, défaut: 20)

### Réponse (200 OK)
```json
[
  {
    "productId": "prod-001",
    "name": "Laptop Dell XPS 15",
    "category": "Electronics",
    "purchaseDate": "2024-02-10T15:30:00Z",
    "quantity": 1,
    "price": 1299.99
  },
  {
    "productId": "prod-002",
    "name": "Wireless Mouse",
    "category": "Accessories",
    "purchaseDate": "2024-02-15T10:20:00Z",
    "quantity": 2,
    "price": 49.99
  }
]
```

---

## 5️⃣ POST - Enregistrer une vue de produit

### Endpoint
```
POST http://localhost:5003/api/recommendations/view
Content-Type: application/json
```

### JSON Corps de la requête

#### Exemple 1: Vue Web simple
```json
{
  "userId": "user-123",
  "productId": "prod-001",
  "duration": 120,
  "source": "web"
}
```

#### Exemple 2: Vue Mobile
```json
{
  "userId": "user-456",
  "productId": "prod-002",
  "duration": 45,
  "source": "mobile"
}
```

#### Exemple 3: Vue Email
```json
{
  "userId": "user-789",
  "productId": "prod-003",
  "duration": 30,
  "source": "email"
}
```

### Réponse (204 No Content)
```
(Aucun corps)
```

---

## 6️⃣ POST - Enregistrer un achat

### Endpoint
```
POST http://localhost:5003/api/recommendations/purchase
Content-Type: application/json
```

### JSON Corps de la requête

#### Exemple 1: Achat simple (1 article)
```json
{
  "userId": "user-123",
  "orderId": "order-2024-001",
  "items": [
    {
      "productId": "prod-001",
      "quantity": 1,
      "price": 1299.99
    }
  ]
}
```

#### Exemple 2: Achat multiple (plusieurs articles)
```json
{
  "userId": "user-456",
  "orderId": "order-2024-002",
  "items": [
    {
      "productId": "prod-002",
      "quantity": 2,
      "price": 49.99
    },
    {
      "productId": "prod-003",
      "quantity": 1,
      "price": 29.99
    },
    {
      "productId": "prod-004",
      "quantity": 3,
      "price": 15.99
    }
  ]
}
```

#### Exemple 3: Achat en gros (quantité importante)
```json
{
  "userId": "user-789",
  "orderId": "order-2024-003",
  "items": [
    {
      "productId": "prod-001",
      "quantity": 5,
      "price": 1299.99
    },
    {
      "productId": "prod-005",
      "quantity": 10,
      "price": 999.99
    }
  ]
}
```

### Réponse (204 No Content)
```
(Aucun corps)
```

---

## 🧪 Commandes de Test (PowerShell)

### Test 1: Obtenir recommandations
```powershell
$response = Invoke-WebRequest `
  -Uri "http://localhost:5003/api/recommendations/user-123?limit=10" `
  -Method GET `
  -UseBasicParsing

$response.StatusCode
$response.Content | ConvertFrom-Json | ConvertTo-Json -Depth 3
```

### Test 2: Obtenir produits similaires
```powershell
$response = Invoke-WebRequest `
  -Uri "http://localhost:5003/api/recommendations/similar/prod-001?limit=5" `
  -Method GET `
  -UseBasicParsing

$response.StatusCode
$response.Content | ConvertFrom-Json | ConvertTo-Json -Depth 3
```

### Test 3: Obtenir produits tendance
```powershell
$response = Invoke-WebRequest `
  -Uri "http://localhost:5003/api/recommendations/trending?days=7&limit=10" `
  -Method GET `
  -UseBasicParsing

$response.StatusCode
$response.Content | ConvertFrom-Json | ConvertTo-Json -Depth 3
```

### Test 4: Obtenir historique utilisateur
```powershell
$response = Invoke-WebRequest `
  -Uri "http://localhost:5003/api/recommendations/history/user-123?limit=20" `
  -Method GET `
  -UseBasicParsing

$response.StatusCode
$response.Content | ConvertFrom-Json | ConvertTo-Json -Depth 3
```

### Test 5: Enregistrer une vue
```powershell
$body = @{
    userId = "user-123"
    productId = "prod-001"
    duration = 120
    source = "web"
} | ConvertTo-Json

$response = Invoke-WebRequest `
  -Uri "http://localhost:5003/api/recommendations/view" `
  -Method POST `
  -Header @{"Content-Type" = "application/json"} `
  -Body $body `
  -UseBasicParsing

$response.StatusCode
```

### Test 6: Enregistrer un achat
```powershell
$body = @{
    userId = "user-123"
    orderId = "order-2024-001"
    items = @(
        @{
            productId = "prod-001"
            quantity = 1
            price = 1299.99
        },
        @{
            productId = "prod-002"
            quantity = 2
            price = 49.99
        }
    )
} | ConvertTo-Json -Depth 3

$response = Invoke-WebRequest `
  -Uri "http://localhost:5003/api/recommendations/purchase" `
  -Method POST `
  -Header @{"Content-Type" = "application/json"} `
  -Body $body `
  -UseBasicParsing

$response.StatusCode
```

---

## 🧪 Commandes de Test (cURL)

### Test 1: Obtenir recommandations
```bash
curl -X GET "http://localhost:5003/api/recommendations/user-123?limit=10" \
  -H "Content-Type: application/json"
```

### Test 2: Obtenir produits similaires
```bash
curl -X GET "http://localhost:5003/api/recommendations/similar/prod-001?limit=5" \
  -H "Content-Type: application/json"
```

### Test 3: Obtenir produits tendance
```bash
curl -X GET "http://localhost:5003/api/recommendations/trending?days=7&limit=10" \
  -H "Content-Type: application/json"
```

### Test 4: Obtenir historique utilisateur
```bash
curl -X GET "http://localhost:5003/api/recommendations/history/user-123?limit=20" \
  -H "Content-Type: application/json"
```

### Test 5: Enregistrer une vue
```bash
curl -X POST "http://localhost:5003/api/recommendations/view" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user-123",
    "productId": "prod-001",
    "duration": 120,
    "source": "web"
  }'
```

### Test 6: Enregistrer un achat
```bash
curl -X POST "http://localhost:5003/api/recommendations/purchase" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user-123",
    "orderId": "order-2024-001",
    "items": [
      {
        "productId": "prod-001",
        "quantity": 1,
        "price": 1299.99
      },
      {
        "productId": "prod-002",
        "quantity": 2,
        "price": 49.99
      }
    ]
  }'
```

---

## ❌ Codes d'erreur

### 400 - Bad Request
```json
{
  "message": "UserId cannot be empty"
}
```

### 500 - Erreur interne
```json
{
  "message": "Error retrieving recommendations"
}
```

---

## 📝 Fichiers Utiles

- **POSTMAN.md** - Guide Postman complet avec tests
- **TEST_GUIDE.md** - Guide de test complet
- **README.md** - Documentation générale

---

## 🚀 Exécution Rapide

Copier-coller ces commandes dans PowerShell:

```powershell
# Test 1: Enregistrer une vue
$body1 = @{userId="user-1"; productId="prod-1"; duration=120; source="web"} | ConvertTo-Json
Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/view" -Method POST -Header @{"Content-Type"="application/json"} -Body $body1 -UseBasicParsing | Select-Object StatusCode

# Test 2: Enregistrer un achat
$body2 = @{userId="user-1"; orderId="order-1"; items=@(@{productId="prod-1"; quantity=1; price=99.99})} | ConvertTo-Json -Depth 3
Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/purchase" -Method POST -Header @{"Content-Type"="application/json"} -Body $body2 -UseBasicParsing | Select-Object StatusCode

# Test 3: Obtenir les recommandations
Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/user-1?limit=5" -Method GET -UseBasicParsing | Select-Object -ExpandProperty Content | ConvertFrom-Json

# Test 4: Obtenir les tendances
Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/trending?days=7&limit=10" -Method GET -UseBasicParsing | Select-Object -ExpandProperty Content | ConvertFrom-Json
```

---

## ✅ Validations

**Avant de tester, assurez-vous que:**

- ✅ Tous les conteneurs Docker sont en cours d'exécution
- ✅ Recommendation API écoute sur `http://localhost:5003`
- ✅ Neo4j est accessible sur `bolt://localhost:7687`
- ✅ MongoDB est accessible sur `mongodb://localhost:27017`
- ✅ RabbitMQ est accessible sur `amqp://localhost:5672`

Vérifier:
```powershell
docker ps --filter "label=com.docker.compose.project=projetmarktplace_net"
```
