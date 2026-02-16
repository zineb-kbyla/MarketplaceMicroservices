# 🎯 Guide Complet d'Intégration - Tous les Microservices

**Document complet pour tester tous les microservices ensemble et voir le flux d'interaction**

---

## 📊 Tableau de Synthèse des Microservices

| Microservice | Port | Role | Base URL |
|---|---|---|---|
| **Product.API** | 5001 | Gestion des produits & catégories | `http://localhost:5001/api` |
| **Order.API** | 5002 | Gestion des commandes | `http://localhost:5002/api` |
| **Recommendation.API** | 5003 | Recommandations personnalisées | `http://localhost:5003/api` |
| **Neo4j** | 7687 (bolt) | Base de données graphe | `bolt://localhost:7687` |
| **MongoDB** | 27017 | Base de données document | `mongodb://localhost:27017` |
| **RabbitMQ** | 5672 (amqp) | Message bus | `amqp://localhost:5672` |

---

# 🔷 PRODUCT.API (http://localhost:5001/api)

## 1. GET /products - Récupérer tous les produits

### Requête
```
GET http://localhost:5001/api/products
```

### Réponse (200 OK)
```json
[
  {
    "id": "prod-001",
    "name": "Laptop Dell XPS 15",
    "description": "High-performance laptop for professionals",
    "category": "Electronics",
    "price": 1299.99,
    "stock": 50,
    "imageUrl": "https://example.com/laptop.jpg",
    "rating": 4.8,
    "reviewCount": 125,
    "status": "Active",
    "createdAt": "2024-02-01T10:00:00Z",
    "updatedAt": "2024-02-16T10:00:00Z"
  },
  {
    "id": "prod-002",
    "name": "Wireless Mouse",
    "description": "Ergonomic wireless mouse",
    "category": "Accessories",
    "price": 49.99,
    "stock": 200,
    "imageUrl": "https://example.com/mouse.jpg",
    "rating": 4.5,
    "reviewCount": 89,
    "status": "Active",
    "createdAt": "2024-02-02T10:00:00Z",
    "updatedAt": "2024-02-16T10:00:00Z"
  }
]
```

---

## 2. GET /products/{id} - Récupérer un produit par ID

### Requête
```
GET http://localhost:5001/api/products/prod-001
```

### Réponse (200 OK)
```json
{
  "id": "prod-001",
  "name": "Laptop Dell XPS 15",
  "description": "High-performance laptop for professionals",
  "category": "Electronics",
  "price": 1299.99,
  "stock": 50,
  "imageUrl": "https://example.com/laptop.jpg",
  "rating": 4.8,
  "reviewCount": 125,
  "status": "Active",
  "createdAt": "2024-02-01T10:00:00Z",
  "updatedAt": "2024-02-16T10:00:00Z"
}
```

---

## 3. GET /products/category/{category} - Récupérer par catégorie

### Requête
```
GET http://localhost:5001/api/products/category/Electronics
```

### Réponse (200 OK)
```json
[
  {
    "id": "prod-001",
    "name": "Laptop Dell XPS 15",
    "category": "Electronics",
    "price": 1299.99,
    "stock": 50,
    "imageUrl": "https://example.com/laptop.jpg",
    "rating": 4.8,
    "reviewCount": 125,
    "status": "Active",
    "createdAt": "2024-02-01T10:00:00Z",
    "updatedAt": "2024-02-16T10:00:00Z"
  }
]
```

---

## 4. GET /products/search - Rechercher des produits

### Requête
```
GET http://localhost:5001/api/products/search?q=laptop
```

### Réponse (200 OK)
```json
[
  {
    "id": "prod-001",
    "name": "Laptop Dell XPS 15",
    "category": "Electronics",
    "price": 1299.99,
    "stock": 50,
    "rating": 4.8
  },
  {
    "id": "prod-003",
    "name": "Laptop HP Pavilion 15",
    "category": "Electronics", 
    "price": 899.99,
    "stock": 30,
    "rating": 4.6
  }
]
```

---

## 5. POST /products - Créer un produit

### Requête
```
POST http://localhost:5001/api/products
Content-Type: application/json
```

### JSON Exemple 1: Électronique
```json
{
  "name": "iPhone 15 Pro",
  "description": "Latest Apple smartphone with advanced camera",
  "category": "Electronics",
  "price": 999.99,
  "stock": 100,
  "imageUrl": "https://example.com/iphone15pro.jpg"
}
```

### JSON Exemple 2: Accessoires
```json
{
  "name": "USB-C Cable",
  "description": "Fast charging USB-C cable",
  "category": "Accessories",
  "price": 19.99,
  "stock": 500,
  "imageUrl": "https://example.com/usbc-cable.jpg"
}
```

### JSON Exemple 3: Vêtements
```json
{
  "name": "Cotton T-Shirt Blue",
  "description": "Comfortable 100% cotton t-shirt",
  "category": "Clothing",
  "price": 29.99,
  "stock": 300,
  "imageUrl": "https://example.com/tshirt.jpg"
}
```

### Réponse (201 Created)
```json
{
  "id": "prod-004",
  "name": "iPhone 15 Pro",
  "description": "Latest Apple smartphone with advanced camera",
  "category": "Electronics",
  "price": 999.99,
  "stock": 100,
  "imageUrl": "https://example.com/iphone15pro.jpg",
  "rating": 0,
  "reviewCount": 0,
  "status": "Active",
  "createdAt": "2024-02-16T15:30:00Z",
  "updatedAt": "2024-02-16T15:30:00Z"
}
```

---

## 6. PUT /products/{id} - Mettre à jour un produit

### Requête
```
PUT http://localhost:5001/api/products/prod-001
Content-Type: application/json
```

### JSON Exemple
```json
{
  "name": "Laptop Dell XPS 15 2024",
  "description": "Updated high-performance laptop",
  "category": "Electronics",
  "price": 1399.99,
  "stock": 75,
  "imageUrl": "https://example.com/laptop-2024.jpg"
}
```

### Réponse (204 No Content)
```
(pas de corps)
```

---

## 7. DELETE /products/{id} - Supprimer un produit

### Requête
```
DELETE http://localhost:5001/api/products/prod-004
```

### Réponse (204 No Content)
```
(pas de corps)
```

---

## 8. POST /products/{id}/decrement-stock - Réduire le stock

### Requête
```
POST http://localhost:5001/api/products/prod-001/decrement-stock
Content-Type: application/json
```

### JSON Exemple
```json
{
  "quantity": 5
}
```

### Réponse (204 No Content)
```
(pas de corps)
```

---

## 9. GET /categories - Récupérer toutes les catégories

### Requête
```
GET http://localhost:5001/api/categories
```

### Réponse (200 OK)
```json
[
  {
    "id": "cat-001",
    "name": "Electronics",
    "description": "Electronic devices and gadgets",
    "imageUrl": "https://example.com/electronics.jpg",
    "productCount": 25,
    "createdAt": "2024-01-01T00:00:00Z"
  },
  {
    "id": "cat-002",
    "name": "Accessories",
    "description": "Phone and computer accessories",
    "imageUrl": "https://example.com/accessories.jpg",
    "productCount": 50,
    "createdAt": "2024-01-01T00:00:00Z"
  }
]
```

---

## 10. POST /categories - Créer une catégorie

### Requête
```
POST http://localhost:5001/api/categories
Content-Type: application/json
```

### JSON Exemple
```json
{
  "name": "Clothing",
  "description": "Men and women clothing",
  "imageUrl": "https://example.com/clothing.jpg"
}
```

### Réponse (201 Created)
```json
{
  "id": "cat-003",
  "name": "Clothing",
  "description": "Men and women clothing",
  "imageUrl": "https://example.com/clothing.jpg",
  "productCount": 0,
  "createdAt": "2024-02-16T15:30:00Z"
}
```

---

# 🔶 ORDER.API (http://localhost:5002/api)

## 1. GET /orders - Récupérer tous les commandes

### Requête
```
GET http://localhost:5002/api/orders
```

### Réponse (200 OK)
```json
[
  {
    "id": "order-001",
    "orderNumber": "ORD-2024-001",
    "userId": "user-123",
    "userName": "John Doe",
    "totalAmount": 1349.97,
    "status": "Pending",
    "orderItems": [
      {
        "productId": "prod-001",
        "productName": "Laptop Dell XPS 15",
        "quantity": 1,
        "unitPrice": 1299.99,
        "totalPrice": 1299.99
      },
      {
        "productId": "prod-002",
        "productName": "Wireless Mouse",
        "quantity": 1,
        "unitPrice": 49.99,
        "totalPrice": 49.99
      }
    ],
    "shippingAddress": {
      "street": "123 Main St",
      "city": "Paris",
      "state": "Île-de-France",
      "country": "France",
      "zipCode": "75001",
      "phoneNumber": "+33123456789"
    },
    "paymentInfo": {
      "cardName": "John Doe",
      "cardNumber": "****-****-****-1234",
      "expiration": "12/25",
      "paymentMethod": "CreditCard"
    },
    "createdAt": "2024-02-16T15:00:00Z",
    "updatedAt": "2024-02-16T15:00:00Z"
  }
]
```

---

## 2. GET /orders/{id} - Récupérer une commande par ID

### Requête
```
GET http://localhost:5002/api/orders/order-001
```

### Réponse (200 OK)
```json
{
  "id": "order-001",
  "orderNumber": "ORD-2024-001",
  "userId": "user-123",
  "userName": "John Doe",
  "totalAmount": 1349.97,
  "status": "Pending",
  "orderItems": [
    {
      "productId": "prod-001",
      "productName": "Laptop Dell XPS 15",
      "quantity": 1,
      "unitPrice": 1299.99,
      "totalPrice": 1299.99
    },
    {
      "productId": "prod-002",
      "productName": "Wireless Mouse",
      "quantity": 1,
      "unitPrice": 49.99,
      "totalPrice": 49.99
    }
  ],
  "shippingAddress": {
    "street": "123 Main St",
    "city": "Paris",
    "state": "Île-de-France",
    "country": "France",
    "zipCode": "75001",
    "phoneNumber": "+33123456789"
  },
  "paymentInfo": {
    "cardName": "John Doe",
    "cardNumber": "****-****-****-1234",
    "expiration": "12/25",
    "paymentMethod": "CreditCard"
  },
  "createdAt": "2024-02-16T15:00:00Z",
  "updatedAt": "2024-02-16T15:00:00Z"
}
```

---

## 3. GET /orders/user/{userId} - Récupérer les commandes d'un utilisateur

### Requête
```
GET http://localhost:5002/api/orders/user/user-123
```

### Réponse (200 OK)
```json
[
  {
    "id": "order-001",
    "orderNumber": "ORD-2024-001",
    "userId": "user-123",
    "userName": "John Doe",
    "totalAmount": 1349.97,
    "status": "Pending",
    "orderItems": [...]
  }
]
```

---

## 4. POST /orders - Créer une commande

### Requête
```
POST http://localhost:5002/api/orders
Content-Type: application/json
```

### JSON Exemple 1: Commande Simple
```json
{
  "userId": "user-123",
  "userName": "John Doe",
  "items": [
    {
      "productId": "prod-001",
      "productName": "Laptop Dell XPS 15",
      "quantity": 1,
      "unitPrice": 1299.99,
      "totalPrice": 1299.99
    }
  ],
  "shippingAddress": {
    "street": "123 Main St",
    "city": "Paris",
    "state": "Île-de-France",
    "country": "France",
    "zipCode": "75001",
    "phoneNumber": "+33123456789"
  },
  "paymentInfo": {
    "cardName": "John Doe",
    "cardNumber": "1234-5678-9101-1121",
    "expiration": "12/25",
    "cvv": "123",
    "paymentMethod": "CreditCard"
  }
}
```

### JSON Exemple 2: Commande Groupée
```json
{
  "userId": "user-456",
  "userName": "Jane Smith",
  "items": [
    {
      "productId": "prod-001",
      "productName": "Laptop Dell XPS 15",
      "quantity": 1,
      "unitPrice": 1299.99,
      "totalPrice": 1299.99
    },
    {
      "productId": "prod-002",
      "productName": "Wireless Mouse",
      "quantity": 2,
      "unitPrice": 49.99,
      "totalPrice": 99.98
    },
    {
      "productId": "prod-003",
      "productName": "USB-C Cable",
      "quantity": 3,
      "unitPrice": 19.99,
      "totalPrice": 59.97
    }
  ],
  "shippingAddress": {
    "street": "456 Oak Avenue",
    "city": "Lyon",
    "state": "Auvergne-Rhône-Alpes",
    "country": "France",
    "zipCode": "69000",
    "phoneNumber": "+33987654321"
  },
  "paymentInfo": {
    "cardName": "Jane Smith",
    "cardNumber": "2345-6789-0121-3141",
    "expiration": "06/26",
    "cvv": "456",
    "paymentMethod": "CreditCard"
  }
}
```

### JSON Exemple 3: Commande en Gros
```json
{
  "userId": "user-789",
  "userName": "Bulk Buyer Inc",
  "items": [
    {
      "productId": "prod-002",
      "productName": "Wireless Mouse",
      "quantity": 100,
      "unitPrice": 49.99,
      "totalPrice": 4999.00
    },
    {
      "productId": "prod-003",
      "productName": "USB-C Cable",
      "quantity": 500,
      "unitPrice": 19.99,
      "totalPrice": 9995.00
    }
  ],
  "shippingAddress": {
    "street": "999 Commerce Blvd",
    "city": "Marseille",
    "state": "Provence-Alpes-Côte d'Azur",
    "country": "France",
    "zipCode": "13000",
    "phoneNumber": "+33555666777"
  },
  "paymentInfo": {
    "cardName": "Bulk Buyer Inc",
    "cardNumber": "3456-7890-1213-1415",
    "expiration": "03/27",
    "cvv": "789",
    "paymentMethod": "CreditCard"
  }
}
```

### Réponse (201 Created)
```json
{
  "id": "order-002",
  "orderNumber": "ORD-2024-002",
  "userId": "user-456",
  "userName": "Jane Smith",
  "totalAmount": 1459.94,
  "status": "Pending",
  "orderItems": [
    {
      "productId": "prod-001",
      "productName": "Laptop Dell XPS 15",
      "quantity": 1,
      "unitPrice": 1299.99,
      "totalPrice": 1299.99
    },
    {
      "productId": "prod-002",
      "productName": "Wireless Mouse",
      "quantity": 2,
      "unitPrice": 49.99,
      "totalPrice": 99.98
    },
    {
      "productId": "prod-003",
      "productName": "USB-C Cable",
      "quantity": 3,
      "unitPrice": 19.99,
      "totalPrice": 59.97
    }
  ],
  "shippingAddress": {
    "street": "456 Oak Avenue",
    "city": "Lyon",
    "state": "Auvergne-Rhône-Alpes",
    "country": "France",
    "zipCode": "69000",
    "phoneNumber": "+33987654321"
  },
  "paymentInfo": {
    "cardName": "Jane Smith",
    "cardNumber": "****-****-****-3141",
    "expiration": "06/26",
    "paymentMethod": "CreditCard"
  },
  "createdAt": "2024-02-16T16:00:00Z",
  "updatedAt": "2024-02-16T16:00:00Z"
}
```

---

## 5. PUT /orders/{id}/status - Mettre à jour le statut de la commande

### Requête
```
PUT http://localhost:5002/api/orders/order-001/status
Content-Type: application/json
```

### JSON Exemple
```json
{
  "status": "Processing"
}
```

**Statuts possibles:**
- `Pending` - En attente
- `Processing` - En cours de traitement
- `Shipped` - Expédié
- `Delivered` - Livré
- `Cancelled` - Annulé

### Réponse (200 OK)
```json
{
  "message": "Order status updated successfully"
}
```

---

## 6. DELETE /orders/{id} - Annuler une commande

### Requête
```
DELETE http://localhost:5002/api/orders/order-001
```

### Réponse (204 No Content)
```
(pas de corps)
```

---

## 7. GET /orders/{id}/tracking - Obtenir le suivi de la commande

### Requête
```
GET http://localhost:5002/api/orders/order-001/tracking
```

### Réponse (200 OK)
```json
{
  "orderId": "order-001",
  "orderNumber": "ORD-2024-001",
  "status": "Processing",
  "estimatedDelivery": "2024-02-20T00:00:00Z",
  "trackingEvents": [
    {
      "timestamp": "2024-02-16T15:00:00Z",
      "status": "Pending",
      "description": "Order received"
    },
    {
      "timestamp": "2024-02-16T16:30:00Z",
      "status": "Processing",
      "description": "Order is being prepared"
    }
  ]
}
```

---

# 🟣 RECOMMENDATION.API (http://localhost:5003/api)

## 1. GET /recommendations/{userId} - Recommandations Personnalisées

### Requête
```
GET http://localhost:5003/api/recommendations/user-123?limit=10
```

### Réponse (200 OK)
```json
[
  {
    "productId": "prod-004",
    "name": "iPhone 15 Pro",
    "category": "Electronics",
    "price": 999.99,
    "imageUrl": "https://example.com/iphone15pro.jpg",
    "rating": 4.9,
    "score": 0.95,
    "reason": "12 utilisateurs similaires ont acheté ce produit",
    "confidence": 0.92
  },
  {
    "productId": "prod-003",
    "name": "USB-C Cable",
    "category": "Accessories",
    "price": 19.99,
    "imageUrl": "https://example.com/usbc.jpg",
    "rating": 4.7,
    "score": 0.88,
    "reason": "Souvent acheté avec vos produits",
    "confidence": 0.85
  }
]
```

---

## 2. GET /recommendations/similar/{productId} - Produits Similaires

### Requête
```
GET http://localhost:5003/api/recommendations/similar/prod-001?limit=5
```

### Réponse (200 OK)
```json
[
  {
    "productId": "prod-003",
    "name": "Laptop HP Pavilion 15",
    "category": "Electronics",
    "price": 899.99,
    "similarityScore": 0.91,
    "reason": "Même catégorie - Laptop haute performance"
  },
  {
    "productId": "prod-005",
    "name": "Laptop ASUS ROG",
    "category": "Electronics",
    "price": 1499.99,
    "similarityScore": 0.88,
    "reason": "Même catégorie - Laptop Gaming"
  }
]
```

---

## 3. GET /recommendations/trending - Produits Tendance

### Requête
```
GET http://localhost:5003/api/recommendations/trending?days=7&limit=10
```

### Réponse (200 OK)
```json
[
  {
    "productId": "prod-004",
    "name": "iPhone 15 Pro",
    "category": "Electronics",
    "price": 999.99,
    "recentPurchases": 245,
    "trendScore": 1.0
  },
  {
    "productId": "prod-002",
    "name": "AirPods Pro",
    "category": "Accessories",
    "price": 249.99,
    "recentPurchases": 198,
    "trendScore": 0.98
  }
]
```

---

## 4. GET /recommendations/history/{userId} - Historique Utilisateur

### Requête
```
GET http://localhost:5003/api/recommendations/history/user-123?limit=20
```

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

## 5. POST /recommendations/view - Enregistrer une Vue de Produit

### Requête
```
POST http://localhost:5003/api/recommendations/view
Content-Type: application/json
```

### JSON Exemple 1: Vue Web
```json
{
  "userId": "user-123",
  "productId": "prod-001",
  "duration": 120,
  "source": "web"
}
```

### JSON Exemple 2: Vue Mobile
```json
{
  "userId": "user-456",
  "productId": "prod-002",
  "duration": 45,
  "source": "mobile"
}
```

### JSON Exemple 3: Vue Email
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
(pas de corps)
```

---

## 6. POST /recommendations/purchase - Enregistrer un Achat

### Requête
```
POST http://localhost:5003/api/recommendations/purchase
Content-Type: application/json
```

### JSON Exemple 1: Achat Simple
```json
{
  "userId": "user-123",
  "orderId": "order-001",
  "items": [
    {
      "productId": "prod-001",
      "quantity": 1,
      "price": 1299.99
    }
  ]
}
```

### JSON Exemple 2: Achat Multiple
```json
{
  "userId": "user-456",
  "orderId": "order-002",
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
    },
    {
      "productId": "prod-003",
      "quantity": 3,
      "price": 19.99
    }
  ]
}
```

### JSON Exemple 3: Achat en Gros
```json
{
  "userId": "user-789",
  "orderId": "order-003",
  "items": [
    {
      "productId": "prod-002",
      "quantity": 100,
      "price": 49.99
    },
    {
      "productId": "prod-003",
      "quantity": 500,
      "price": 19.99
    }
  ]
}
```

### Réponse (204 No Content)
```
(pas de corps)
```

---

# 🚀 SCÉNARIO DE TEST COMPLET - FLUX D'INTÉGRATION

## Étape 1: Créer des Produits

```powershell
# Créer Product 1: Laptop
$product1 = @{
    name = "Laptop Dell XPS 15"
    description = "High-performance laptop"
    category = "Electronics"
    price = 1299.99
    stock = 50
    imageUrl = "https://example.com/laptop.jpg"
} | ConvertTo-Json

$response1 = Invoke-WebRequest -Uri "http://localhost:5001/api/products" `
  -Method POST `
  -Header @{"Content-Type"="application/json"} `
  -Body $product1 -UseBasicParsing
$prodId1 = ($response1.Content | ConvertFrom-Json).id

# Créer Product 2: Mouse
$product2 = @{
    name = "Wireless Mouse"
    description = "Ergonomic wireless mouse"
    category = "Accessories"
    price = 49.99
    stock = 200
    imageUrl = "https://example.com/mouse.jpg"
} | ConvertTo-Json

$response2 = Invoke-WebRequest -Uri "http://localhost:5001/api/products" `
  -Method POST `
  -Header @{"Content-Type"="application/json"} `
  -Body $product2 -UseBasicParsing
$prodId2 = ($response2.Content | ConvertFrom-Json).id

Write-Host "✅ Produits créés:"
Write-Host "  - Product 1 ID: $prodId1"
Write-Host "  - Product 2 ID: $prodId2"
```

---

## Étape 2: Enregistrer des Vues de Produits

```powershell
# User 1 regarde Laptop
$view1 = @{
    userId = "user-123"
    productId = $prodId1
    duration = 180
    source = "web"
} | ConvertTo-Json

Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/view" `
  -Method POST `
  -Header @{"Content-Type"="application/json"} `
  -Body $view1 -UseBasicParsing | Out-Null

# User 1 regarde Mouse
$view2 = @{
    userId = "user-123"
    productId = $prodId2
    duration = 60
    source = "web"
} | ConvertTo-Json

Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/view" `
  -Method POST `
  -Header @{"Content-Type"="application/json"} `
  -Body $view2 -UseBasicParsing | Out-Null

Write-Host "✅ Vues enregistrées pour user-123"
```

---

## Étape 3: Créer une Commande

```powershell
# User 1 commande les produits
$order = @{
    userId = "user-123"
    userName = "John Doe"
    items = @(
        @{
            productId = $prodId1
            productName = "Laptop Dell XPS 15"
            quantity = 1
            unitPrice = 1299.99
            totalPrice = 1299.99
        },
        @{
            productId = $prodId2
            productName = "Wireless Mouse"
            quantity = 2
            unitPrice = 49.99
            totalPrice = 99.98
        }
    )
    shippingAddress = @{
        street = "123 Main St"
        city = "Paris"
        state = "Île-de-France"
        country = "France"
        zipCode = "75001"
        phoneNumber = "+33123456789"
    }
    paymentInfo = @{
        cardName = "John Doe"
        cardNumber = "1234-5678-9101-1121"
        expiration = "12/25"
        cvv = "123"
        paymentMethod = "CreditCard"
    }
} | ConvertTo-Json -Depth 5

$orderResponse = Invoke-WebRequest -Uri "http://localhost:5002/api/orders" `
  -Method POST `
  -Header @{"Content-Type"="application/json"} `
  -Body $order -UseBasicParsing
$orderId = ($orderResponse.Content | ConvertFrom-Json).id

Write-Host "✅ Commande créée: $orderId"
```

---

## Étape 4: Enregistrer l'Achat

```powershell
# Enregistrer l'achat dans Recommendation.API
$purchase = @{
    userId = "user-123"
    orderId = $orderId
    items = @(
        @{
            productId = $prodId1
            quantity = 1
            price = 1299.99
        },
        @{
            productId = $prodId2
            quantity = 2
            price = 49.99
        }
    )
} | ConvertTo-Json -Depth 3

Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/purchase" `
  -Method POST `
  -Header @{"Content-Type"="application/json"} `
  -Body $purchase -UseBasicParsing | Out-Null

Write-Host "✅ Achat enregistré dans Recommendation.API"
```

---

## Étape 5: Mettre à Jour le Statut de la Commande

```powershell
# Passer la commande de "Pending" à "Processing"
$statusUpdate = @{
    status = "Processing"
} | ConvertTo-Json

Invoke-WebRequest -Uri "http://localhost:5002/api/orders/$orderId/status" `
  -Method PUT `
  -Header @{"Content-Type"="application/json"} `
  -Body $statusUpdate -UseBasicParsing | Out-Null

Write-Host "✅ Statut de la commande mis à jour: Processing"
```

---

## Étape 6: Récupérer les Recommandations

```powershell
# Récupérer les recommandations personnalisées
$recoResponse = Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/user-123?limit=10" `
  -Method GET `
  -UseBasicParsing

Write-Host "✅ Recommandations personnalisées:"
$recoResponse.Content | ConvertFrom-Json | ConvertTo-Json -Depth 3
```

---

## Étape 7: Récupérer l'Historique

```powershell
# Récupérer l'historique de l'utilisateur
$historyResponse = Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/history/user-123?limit=20" `
  -Method GET `
  -UseBasicParsing

Write-Host "✅ Historique utilisateur:"
$historyResponse.Content | ConvertFrom-Json | ConvertTo-Json -Depth 3
```

---

# 🧪 TESTS COMPLETS (PowerShell One-Liners)

## Test Rapide - Tous les Endpoints

```powershell
# 1. GET tous les produits
Invoke-WebRequest -Uri "http://localhost:5001/api/products" -Method GET -UseBasicParsing | Select-Object -ExpandProperty Content | ConvertFrom-Json

# 2. GET toutes les commandes
Invoke-WebRequest -Uri "http://localhost:5002/api/orders" -Method GET -UseBasicParsing | Select-Object -ExpandProperty Content | ConvertFrom-Json

# 3. GET produits tendance
Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/trending?days=7&limit=10" -Method GET -UseBasicParsing | Select-Object -ExpandProperty Content | ConvertFrom-Json

# 4. GET toutes les catégories
Invoke-WebRequest -Uri "http://localhost:5001/api/categories" -Method GET -UseBasicParsing | Select-Object -ExpandProperty Content | ConvertFrom-Json

# 5. Créer un produit simple
$prodBody = @{name="Test Item"; description="Test"; category="Test"; price=99.99; stock=10; imageUrl="http://test.jpg"} | ConvertTo-Json
Invoke-WebRequest -Uri "http://localhost:5001/api/products" -Method POST -Header @{"Content-Type"="application/json"} -Body $prodBody -UseBasicParsing | Select-Object StatusCode

# 6. Enregistrer une vue
$viewBody = @{userId="test-user"; productId="test-prod"; duration=60; source="web"} | ConvertTo-Json
Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/view" -Method POST -Header @{"Content-Type"="application/json"} -Body $viewBody -UseBasicParsing | Select-Object StatusCode
```

---

## Vérifier l'État des Services

```powershell
# Vérifier que tous les services répondent
Write-Host "🔍 Vérification des services..."

try { 
    $p1 = Invoke-WebRequest -Uri "http://localhost:5001/api/products" -Method GET -UseBasicParsing -TimeoutSec 3
    Write-Host "✅ Product.API: OK (Port 5001)"
}
catch { Write-Host "❌ Product.API: ERREUR (Port 5001)" }

try { 
    $p2 = Invoke-WebRequest -Uri "http://localhost:5002/api/orders" -Method GET -UseBasicParsing -TimeoutSec 3
    Write-Host "✅ Order.API: OK (Port 5002)"
}
catch { Write-Host "❌ Order.API: ERREUR (Port 5002)" }

try { 
    $p3 = Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/trending" -Method GET -UseBasicParsing -TimeoutSec 3
    Write-Host "✅ Recommendation.API: OK (Port 5003)"
}
catch { Write-Host "❌ Recommendation.API: ERREUR (Port 5003)" }

# Vérifier les conteneurs Docker
Write-Host "`n🐳 État des conteneurs Docker:"
docker ps --filter "label=com.docker.compose.project=projetmarktplace_net" --format "{{.Names}}\t{{.Status}}"
```

---

# 📝 Fichiers cURL Complètes

## Créer un Produit (cURL)
```bash
curl -X POST "http://localhost:5001/api/products" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "iPhone 15 Pro",
    "description": "Latest Apple smartphone",
    "category": "Electronics",
    "price": 999.99,
    "stock": 100,
    "imageUrl": "https://example.com/iphone.jpg"
  }'
```

## Créer une Commande (cURL)
```bash
curl -X POST "http://localhost:5002/api/orders" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user-123",
    "userName": "John Doe",
    "items": [
      {
        "productId": "prod-001",
        "productName": "Laptop Dell XPS 15",
        "quantity": 1,
        "unitPrice": 1299.99,
        "totalPrice": 1299.99
      }
    ],
    "shippingAddress": {
      "street": "123 Main St",
      "city": "Paris",
      "state": "Île-de-France",
      "country": "France",
      "zipCode": "75001",
      "phoneNumber": "+33123456789"
    },
    "paymentInfo": {
      "cardName": "John Doe",
      "cardNumber": "1234-5678-9101-1121",
      "expiration": "12/25",
      "cvv": "123",
      "paymentMethod": "CreditCard"
    }
  }'
```

## Enregistrer une Vue (cURL)
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

---

# ✅ Checklist de Validation

Avant de tester, assurez-vous que:

- ✅ Tous les conteneurs Docker sont en cours d'exécution
  ```powershell
  docker ps --filter "label=com.docker.compose.project=projetmarktplace_net"
  ```

- ✅ Product.API écoute sur http://localhost:5001
- ✅ Order.API écoute sur http://localhost:5002
- ✅ Recommendation.API écoute sur http://localhost:5003
- ✅ Neo4j est accessible sur bolt://localhost:7687
- ✅ MongoDB est accessible sur mongodb://localhost:27017
- ✅ RabbitMQ est accessible sur amqp://localhost:5672

---

# 🔗 Ressources Additionnelles

## Dashboards et Outils
- **Neo4j Browser**: http://localhost:7474 (user: neo4j, password: changeme)
- **RabbitMQ Management**: http://localhost:15672 (user: guest, password: guest)

## Fichiers de Documentation
- `ENDPOINTS_JSON.md` - Détails des endpoints Recommendation.API
- `POSTMAN.md` - Guide d'utilisation Postman
- `TEST_GUIDE.md` - Guide de test complet
- `README.md` - Documentation générale

---

## 📧 Support & Dépannage

### Erreur: "Connection refused"
- Vérifier que Docker est démarré
- Relancer les conteneurs: `docker-compose up -d`

### Erreur 500 dans les recommandations
- Vérifier la connexion Neo4j
- Vérifier les logs: `docker logs recommendation_api`

### Erreur 400 dans les commandes
- Vérifier le format JSON (tous les champs requis)
- Vérifier que les IDs de produits existent

---

**Créé**: 16 février 2024
**Version**: 1.0
**Dernière mise à jour**: 16 février 2024
