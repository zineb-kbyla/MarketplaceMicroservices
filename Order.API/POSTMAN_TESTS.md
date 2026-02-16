# Postman Tests - Order Service 🛒

Guide complet pour tester tous les endpoints de l'API Order Service avec Postman.

---

## 📋 Base URL (local)

- **HTTP**: http://localhost:5002
- **HTTPS**: https://localhost:7002
- **Swagger/Scalar UI**: http://localhost:5002/scalar/v1

---

## 🔧 Configuration Postman

### Headers communs

Tous les endpoints nécessitent:
```
Content-Type: application/json
```

### Variables d'environnement (optionnel)

Créer un environnement Postman avec:

```json
{
  "baseUrl": "http://localhost:5002",
  "orderId": "",
  "userId": "user123"
}
```

---

## 📌 Liste complète des endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/orders` | Get all orders (avec filtre userId optionnel) |
| GET | `/api/orders/{id}` | Get order by ID |
| GET | `/api/orders/user/{userId}` | Get orders for specific user |
| POST | `/api/orders` | Create new order |
| PUT | `/api/orders/{id}/status` | Update order status |
| DELETE | `/api/orders/{id}` | Cancel order |
| GET | `/api/orders/{id}/tracking` | Get order tracking info |

---

---

## 🧪 Tests détaillés par endpoint

### 1️⃣ Get all orders

**GET** `/api/orders`

**URL Postman**:
```
http://localhost:5002/api/orders
```

**cURL**:
```bash
curl -X GET "http://localhost:5002/api/orders" -H "Content-Type: application/json"
```

**Response (200 OK)**:
```json
[
  {
    "id": "67890abcdef123456789",
    "orderNumber": "ORD-20260216-SAMPLE01",
    "userId": "user123",
    "userName": "John Doe",
    "totalAmount": 999.99,
    "status": "Delivered",
    "orderItems": [...],
    "shippingAddress": {...},
    "paymentInfo": {...},
    "createdAt": "2026-02-01T10:00:00Z",
    "updatedAt": "2026-02-05T15:30:00Z"
  }
]
```

---

### 1️⃣-B Get orders filtered by userId

**GET** `/api/orders?userId={userId}`

**URL Postman**:
```
http://localhost:5002/api/orders?userId=user123
```

**Query Parameters**:
- `userId` (optional): Filter by user ID

**cURL**:
```bash
curl -X GET "http://localhost:5002/api/orders?userId=user123" -H "Content-Type: application/json"
```

---

### 2️⃣ Get order by ID

**GET** `/api/orders/{id}`

**URL Postman** (remplacer `{id}` par un vrai ID MongoDB):
```
http://localhost:5002/api/orders/67890abcdef123456789
```

**cURL**:
```bash
curl -X GET "http://localhost:5002/api/orders/67890abcdef123456789" -H "Content-Type: application/json"
```

**Response (200 OK)**:
```json
{
  "id": "67890abcdef123456789",
  "orderNumber": "ORD-20260216-A1B2C3D4",
  "userId": "user123",
  "userName": "John Doe",
  "totalAmount": 1299.98,
  "status": "Processing",
  "orderItems": [
    {
      "productId": "prod001",
      "productName": "iPhone 15 Pro",
      "quantity": 1,
      "unitPrice": 999.99,
      "totalPrice": 999.99
    },
    {
      "productId": "prod002",
      "productName": "AirPods Pro",
      "quantity": 1,
      "unitPrice": 299.99,
      "totalPrice": 299.99
    }
  ],
  "shippingAddress": {
    "street": "123 Main St",
    "city": "New York",
    "state": "NY",
    "country": "USA",
    "zipCode": "10001",
    "phoneNumber": "+1234567890"
  },
  "paymentInfo": {
    "paymentMethod": "CreditCard",
    "cardName": "John Doe",
    "cardNumber": "****1234",
    "expiration": "12/25",
    "cvv": "***"
  },
  "createdAt": "2026-02-16T09:00:00Z",
  "updatedAt": "2026-02-16T10:30:00Z"
}
```

**Response (404 Not Found)**:
```json
{
  "message": "Order with ID 67890abcdef123456789 not found"
}
```

---

### 3️⃣ Get orders for specific user

**GET** `/api/orders/user/{userId}`

**URL Postman**:
```
http://localhost:5002/api/orders/user/user123
```

**cURL**:
```bash
curl -X GET "http://localhost:5002/api/orders/user/user123" -H "Content-Type: application/json"
```

**Response**: Same format as "Get all orders"

---

### 4️⃣ Create new order

**POST** `/api/orders`

**URL Postman**:
```
http://localhost:5002/api/orders
```

**Request Body - Commande simple**:
```json
{
  "userId": "user123",
  "userName": "John Doe",
  "items": [
    {
      "productId": "prod001",
      "productName": "iPhone 15 Pro",
      "quantity": 1,
      "unitPrice": 999.99
    }
  ],
  "shippingAddress": {
    "street": "123 Main St",
    "city": "New York",
    "state": "NY",
    "country": "USA",
    "zipCode": "10001",
    "phoneNumber": "+1234567890"
  },
  "paymentInfo": {
    "paymentMethod": "CreditCard",
    "cardName": "John Doe",
    "cardNumber": "4532123456789012",
    "expiration": "12/25",
    "cvv": "123"
  }
}
```

**Request Body - Commande multi-produits**:
```json
{
  "userId": "user456",
  "userName": "Jane Smith",
  "items": [
    {
      "productId": "prod001",
      "productName": "iPhone 15 Pro",
      "quantity": 1,
      "unitPrice": 999.99
    },
    {
      "productId": "prod002",
      "productName": "AirPods Pro",
      "quantity": 2,
      "unitPrice": 299.99
    },
    {
      "productId": "prod003",
      "productName": "Apple Watch Ultra",
      "quantity": 1,
      "unitPrice": 799.99
    }
  ],
  "shippingAddress": {
    "street": "456 Oak Ave",
    "city": "Los Angeles",
    "state": "CA",
    "country": "USA",
    "zipCode": "90001",
    "phoneNumber": "+1987654321"
  },
  "paymentInfo": {
    "paymentMethod": "CreditCard",
    "cardName": "Jane Smith",
    "cardNumber": "5412345678901234",
    "expiration": "06/27",
    "cvv": "456"
  }
}
```

**cURL**:
```bash
curl -X POST "http://localhost:5002/api/orders" \
-H "Content-Type: application/json" \
-d '{
  "userId": "user123",
  "userName": "John Doe",
  "items": [
    {
      "productId": "prod001",
      "productName": "iPhone 15 Pro",
      "quantity": 1,
      "unitPrice": 999.99
    }
  ],
  "shippingAddress": {
    "street": "123 Main St",
    "city": "New York",
    "state": "NY",
    "country": "USA",
    "zipCode": "10001",
    "phoneNumber": "+1234567890"
  },
  "paymentInfo": {
    "paymentMethod": "CreditCard",
    "cardName": "John Doe",
    "cardNumber": "4532123456789012",
    "expiration": "12/25",
    "cvv": "123"
  }
}'
```

**Response (201 Created)**:
```json
{
  "id": "67890abcdef123456789",
  "orderNumber": "ORD-20260216-A1B2C3D4",
  "userId": "user123",
  "userName": "John Doe",
  "totalAmount": 999.99,
  "status": "Pending",
  "orderItems": [
    {
      "productId": "prod001",
      "productName": "iPhone 15 Pro",
      "quantity": 1,
      "unitPrice": 999.99,
      "totalPrice": 999.99
    }
  ],
  "shippingAddress": {
    "street": "123 Main St",
    "city": "New York",
    "state": "NY",
    "country": "USA",
    "zipCode": "10001",
    "phoneNumber": "+1234567890"
  },
  "paymentInfo": {
    "paymentMethod": "CreditCard",
    "cardName": "John Doe",
    "cardNumber": "****9012",
    "expiration": "12/25",
    "cvv": "***"
  },
  "createdAt": "2026-02-16T12:00:00Z",
  "updatedAt": "2026-02-16T12:00:00Z"
}
```

**Response (400 Bad Request)**:
```json
{
  "message": "Order must contain at least one item"
}
```

---

### 5️⃣ Update order status

**PUT** `/api/orders/{id}/status`

**URL Postman** (remplacer `{id}`):
```
http://localhost:5002/api/orders/67890abcdef123456789/status
```

**Statuts possibles**:
- `Pending` - En attente
- `Confirmed` - Confirmée
- `Processing` - En traitement
- `Shipped` - Expédiée
- `Delivered` - Livrée
- `Cancelled` - Annulée
- `Refunded` - Remboursée

**Exemples de Request Body**:

**a) Confirmer une commande**:
```json
{
  "status": "Confirmed"
}
```

**b) Passer en traitement**:
```json
{
  "status": "Processing"
}
```

**c) Marquer comme expédiée**:
```json
{
  "status": "Shipped"
}
```

**d) Marquer comme livrée**:
```json
{
  "status": "Delivered"
}
```

**cURL**:
```bash
curl -X PUT "http://localhost:5002/api/orders/67890abcdef123456789/status" \
-H "Content-Type: application/json" \
-d '{ "status": "Confirmed" }'
```

**Response (200 OK)**:
```json
{
  "message": "Order status updated successfully"
}
```

**Response (404 Not Found)**:
```json
{
  "message": "Order with ID 67890abcdef123456789 not found"
}
```

---

### 6️⃣ Cancel order

**DELETE** `/api/orders/{id}`

**URL Postman** (remplacer `{id}`):
```
http://localhost:5002/api/orders/67890abcdef123456789
```

**⚠️ Important**: Une commande ne peut être annulée que si son statut est:
- `Pending`
- `Confirmed`
- `Processing`

**cURL**:
```bash
curl -X DELETE "http://localhost:5002/api/orders/67890abcdef123456789" \
-H "Content-Type: application/json"
```

**Response (200 OK)**:
```json
{
  "message": "Order cancelled successfully"
}
```

**Response (400 Bad Request)** - Si statut ne permet pas l'annulation:
```json
{
  "message": "Order ORD-20260216-A1B2C3D4 cannot be cancelled in status Delivered"
}
```

**Response (404 Not Found)**:
```json
{
  "message": "Order with ID 67890abcdef123456789 not found"
}
```

---

### 7️⃣ Get order tracking

**GET** `/api/orders/{id}/tracking`

**URL Postman** (remplacer `{id}`):
```
http://localhost:5002/api/orders/67890abcdef123456789/tracking
```

**cURL**:
```bash
curl -X GET "http://localhost:5002/api/orders/67890abcdef123456789/tracking" \
-H "Content-Type: application/json"
```

**Response (200 OK)**:
```json
{
  "orderId": "67890abcdef123456789",
  "orderNumber": "ORD-20260216-A1B2C3D4",
  "status": "Shipped",
  "createdAt": "2026-02-16T09:00:00Z",
  "updatedAt": "2026-02-16T14:30:00Z",
  "estimatedDelivery": "2026-02-19T09:00:00Z"
}
```

**EstimatedDelivery Logic**:
- `Pending`: +7 jours
- `Confirmed`: +6 jours
- `Processing`: +5 jours
- `Shipped`: +3 jours
- `Delivered`: null (déjà livrée)
- `Cancelled`: null (annulée)
- `Refunded`: null (remboursée)

**Response (404 Not Found)**:
```json
{
  "message": "Order with ID 67890abcdef123456789 not found"
}
```

---

---

## 🎯 Scénarios de test complets

### Scénario 1: Workflow complet d'une commande (du début à la livraison)

1. **Créer une commande**
   ```
   POST http://localhost:5002/api/orders
   ```
   ➡️ Copier l'`id` retourné

2. **Vérifier la commande créée**
   ```
   GET http://localhost:5002/api/orders/{id}
   ```
   ✅ status = "Pending"

3. **Suivre la commande**
   ```
   GET http://localhost:5002/api/orders/{id}/tracking
   ```
   ✅ estimatedDelivery = +7 jours

4. **Confirmer la commande**
   ```
   PUT http://localhost:5002/api/orders/{id}/status
   Body: { "status": "Confirmed" }
   ```

5. **Passer en traitement**
   ```
   PUT http://localhost:5002/api/orders/{id}/status
   Body: { "status": "Processing" }
   ```

6. **Expédier la commande**
   ```
   PUT http://localhost:5002/api/orders/{id}/status
   Body: { "status": "Shipped" }
   ```

7. **Vérifier le tracking**
   ```
   GET http://localhost:5002/api/orders/{id}/tracking
   ```
   ✅ estimatedDelivery = +3 jours

8. **Livrer la commande**
   ```
   PUT http://localhost:5002/api/orders/{id}/status
   Body: { "status": "Delivered" }
   ```

9. **Vérifier livraison**
   ```
   GET http://localhost:5002/api/orders/{id}
   ```
   ✅ status = "Delivered"

---

### Scénario 2: Annulation d'une commande

1. **Créer une commande**
   ```
   POST http://localhost:5002/api/orders
   ```

2. **Annuler immédiatement**
   ```
   DELETE http://localhost:5002/api/orders/{id}
   ```
   ✅ Status 200 - "Order cancelled successfully"

3. **Vérifier l'annulation**
   ```
   GET http://localhost:5002/api/orders/{id}
   ```
   ✅ status = "Cancelled"

---

### Scénario 3: Test d'annulation impossible

1. **Créer une commande**
   ```
   POST http://localhost:5002/api/orders
   ```

2. **Livrer la commande**
   ```
   PUT http://localhost:5002/api/orders/{id}/status
   Body: { "status": "Delivered" }
   ```

3. **Tenter d'annuler** (doit échouer)
   ```
   DELETE http://localhost:5002/api/orders/{id}
   ```
   ❌ Status 400 - "Order cannot be cancelled in status Delivered"

---

### Scénario 4: Filtrage par utilisateur

1. **Récupérer toutes les commandes**
   ```
   GET http://localhost:5002/api/orders
   ```

2. **Filtrer par userId via query param**
   ```
   GET http://localhost:5002/api/orders?userId=user123
   ```
   ✅ Toutes les commandes ont userId = "user123"

3. **Filtrer par userId via route dédiée**
   ```
   GET http://localhost:5002/api/orders/user/user123
   ```
   ✅ Même résultat que l'étape 2

---

### Scénario 5: Validation des erreurs

1. **Créer commande sans items** (doit échouer)
   ```
   POST http://localhost:5002/api/orders
   Body: { "userId": "user123", "items": [] }
   ```
   ❌ Status 400 - "Order must contain at least one item"

2. **Récupérer commande inexistante** (doit échouer)
   ```
   GET http://localhost:5002/api/orders/invalidId123
   ```
   ❌ Status 404 - "Order with ID invalidId123 not found"

3. **Mettre à jour statut commande inexistante** (doit échouer)
   ```
   PUT http://localhost:5002/api/orders/invalidId123/status
   Body: { "status": "Confirmed" }
   ```
   ❌ Status 404 - "Order with ID invalidId123 not found"

---

## Codes de Statut Attendus

| Endpoint | Method | Success Code | Error Codes |
|----------|--------|--------------|-------------|
| /api/orders | GET | 200 | 500 |
| /api/orders/{id} | GET | 200 | 404, 500 |
| /api/orders | POST | 201 | 400, 500 |
| /api/orders/{id}/status | PUT | 200 | 404, 500 |
| /api/orders/{id} | DELETE | 200 | 400, 404, 500 |
| /api/orders/{id}/tracking | GET | 200 | 404, 500 |

## Notes de Sécurité

⚠️ **Attention**: Les numéros de carte bancaire sont automatiquement masqués par le service:
- Les numéros complets sont remplacés par `****` + les 4 derniers chiffres
- Le CVV est remplacé par `***`

Les données sensibles ne sont jamais retournées dans les réponses de l'API.

## Utilisation

1. Importer la collection dans Postman
2. Configurer l'environnement avec les variables
3. Exécuter les requêtes dans l'ordre suggéré
4. Vérifier les résultats et les statuts de réponse
