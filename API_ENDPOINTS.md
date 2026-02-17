# 📡 API Endpoints - Via API Gateway

## 🚪 Point d'Entrée Unique

**Base URL:** `http://localhost:5000` (ou `http://api-gateway:5000` en Docker)

---

## 📦 PRODUCT SERVICE

### Produits

#### Lister tous les produits
```http
GET /api/products
```

#### Obtenir un produit par ID
```http
GET /api/products/{id}
```

#### Créer un produit
```http
POST /api/products
Content-Type: application/json

{
  "name": "Product Name",
  "description": "Product Description",
  "category": "Electronics",
  "price": 99.99,
  "stock": 50,
  "imageUrl": "https://example.com/image.jpg"
}
```

#### Mettre à jour un produit
```http
PUT /api/products/{id}
Content-Type: application/json

{
  "name": "Updated Name",
  "description": "Updated Description",
  "category": "Electronics",
  "price": 129.99,
  "stock": 40,
  "imageUrl": "https://example.com/new-image.jpg"
}
```

#### Supprimer un produit
```http
DELETE /api/products/{id}
```

#### Produits par catégorie
```http
GET /api/products/category/{categoryName}
```

#### Rechercher des produits
```http
GET /api/products/search?q=laptop
```

#### Décrémenter le stock
```http
POST /api/products/{id}/decrement-stock
Content-Type: application/json

{
  "quantity": 5
}
```

---

## 🛒 ORDER SERVICE

### Commandes

#### Lister toutes les commandes
```http
GET /api/orders
```

#### Obtenir les commandes d'un utilisateur
```http
GET /api/orders?userId=user123
```

#### Obtenir une commande par ID
```http
GET /api/orders/{id}
```

#### Créer une commande
```http
POST /api/orders
Content-Type: application/json

{
  "userId": "user123",
  "items": [
    {
      "productId": "prod456",
      "quantity": 2,
      "price": 99.99
    }
  ],
  "shippingAddress": "123 Main St, City, State 12345",
  "paymentInfo": {
    "cardNumber": "4111111111111111",
    "cardHolder": "John Doe",
    "expiryDate": "12/25",
    "cvv": "123"
  }
}
```

#### Mettre à jour une commande
```http
PUT /api/orders/{id}
Content-Type: application/json

{
  "shippingAddress": "456 Oak Ave",
  "paymentInfo": { ... }
}
```

#### Mettre à jour le statut d'une commande
```http
PUT /api/orders/{id}/status
Content-Type: application/json

{
  "newStatus": "Shipped"
}
```

**Statuts valides:**
- Pending
- Processing
- Shipped
- Delivered
- Cancelled

#### Annuler une commande
```http
DELETE /api/orders/{id}
```

---

## 🎯 RECOMMENDATION SERVICE

### Recommandations

#### Obtenir les recommandations pour un utilisateur
```http
GET /api/recommendations/{userId}
```

#### Obtenir les utilisateurs similaires
```http
GET /api/recommendations/{userId}/similar-users
```

#### Rafraîchir les recommandations
```http
POST /api/recommendations/refresh
```

---

## 📡 Exemples cURL

### Product Service

```bash
# Lister les produits
curl http://localhost:5000/api/products

# Créer un produit
curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Gaming Laptop",
    "description": "High-performance",
    "category": "Electronics",
    "price": 1500,
    "stock": 20,
    "imageUrl": "https://example.com/laptop.jpg"
  }'

# Rechercher
curl "http://localhost:5000/api/products/search?q=laptop"
```

### Order Service

```bash
# Lister les commandes
curl http://localhost:5000/api/orders

# Créer une commande
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user123",
    "items": [{"productId": "id1", "quantity": 2, "price": 100}],
    "shippingAddress": "Main St",
    "paymentInfo": {"cardNumber": "4111111111111111", "cardHolder": "John", "expiryDate": "12/25", "cvv": "123"}
  }'

# Changer le statut
curl -X PUT http://localhost:5000/api/orders/{orderId}/status \
  -H "Content-Type: application/json" \
  -d '{"newStatus": "Shipped"}'
```

### Recommendation Service

```bash
# Obtenir les recommandations
curl http://localhost:5000/api/recommendations/user123

# Utilisateurs similaires
curl http://localhost:5000/api/recommendations/user123/similar-users

# Rafraîchir
curl -X POST http://localhost:5000/api/recommendations/refresh
```

---

## 📊 Réponses Standard

### Success (2xx)

```json
{
  "id": "123",
  "name": "Product Name",
  "price": 99.99,
  "status": "success"
}
```

### Error (4xx/5xx)

```json
{
  "error": "Not Found",
  "message": "Product with ID 123 not found",
  "statusCode": 404
}
```

---

## ⏱️ Timeouts

- Tous les appels ont un timeout de **30 secondes**
- Si un service ne répond pas → `504 Bad Gateway`

---

## 🔄 Code HTTP

| Code | Signification |
|------|---------------|
| 200 | OK |
| 201 | Created |
| 204 | No Content |
| 400 | Bad Request |
| 404 | Not Found |
| 500 | Server Error |
| 504 | Gateway Timeout |

---

## 🧪 Testing avec Postman

1. **Importer les collections:**
   - `Product-Service.postman_collection.json`
   - `Order-Service.postman_collection.json`

2. **Modifier les endpoints** de `localhost:5001` vers `localhost:5000/api`

3. **Tester via le Gateway**

---

## 🔐 Sécurité (À Implémenter)

- [ ] Authentication JWT
- [ ] Rate limiting
- [ ] HTTPS enforcement
- [ ] CORS configuration (restrict origins)

---

## 📞 Support

- Vérifier que le Gateway est actif: `curl http://localhost:5000/health`
- Vérifier les logs: `docker logs api_gateway`
- Consulter: `GATEWAY_TESTING.md`

