# 🧪 Tests API Gateway - Vérification Point d'Entrée Unique

## 📋 Pré-requis

```bash
# Démarrer tous les services via Docker Compose
docker-compose up --build

# Ou en arrière-plan
docker-compose up -d --build

# Vérifier que tous les services sont actifs
docker-compose ps
```

---

## ✅ Teste 1: Health Check Gateway

```bash
# Vérifier que le gateway répond
curl -i http://localhost:5000/health

# Résultat attendu:
# HTTP/1.1 200 OK
# Content-Type: application/json
# {"status":"Healthy"}
```

---

## ✅ Test 2: Product Service via Gateway

### Lister tous les produits
```bash
curl -i http://localhost:5000/api/products

# Ou avec jq pour formatter
curl http://localhost:5000/api/products | jq '.'
```

### Créer un produit
```bash
curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Product",
    "description": "Test via Gateway",
    "category": "Electronics",
    "price": 99.99,
    "stock": 50,
    "imageUrl": "https://example.com/image.jpg"
  }'
```

### Récupérer un produit spécifique
```bash
# Remplacer {id} par un ID réel
curl http://localhost:5000/api/products/{id} | jq '.'
```

### Chercher des produits
```bash
curl "http://localhost:5000/api/products/search?q=laptop" | jq '.'
```

### Décrémenter le stock
```bash
curl -X POST http://localhost:5000/api/products/{id}/decrement-stock \
  -H "Content-Type: application/json" \
  -d '{"quantity": 5}'
```

---

## ✅ Test 3: Order Service via Gateway

### Lister les commandes
```bash
curl http://localhost:5000/api/orders | jq '.'
```

### Créer une commande
```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user123",
    "items": [
      {
        "productId": "PRODUCT_ID_HERE",
        "quantity": 2,
        "price": 99.99
      }
    ],
    "shippingAddress": "123 Main St",
    "paymentInfo": {
      "cardNumber": "4111111111111111",
      "cardHolder": "John Doe",
      "cvv": "123",
      "expiryDate": "12/25"
    }
  }'
```

### Récupérer une commande
```bash
curl http://localhost:5000/api/orders/{orderId} | jq '.'
```

### Mettre à jour le statut de commande
```bash
curl -X PUT http://localhost:5000/api/orders/{orderId}/status \
  -H "Content-Type: application/json" \
  -d '{"newStatus": "Shipped"}'
```

---

## ✅ Test 4: Recommendation Service via Gateway

### Obtenir les recommandations
```bash
curl http://localhost:5000/api/recommendations/user123 | jq '.'
```

### Utilisateurs similaires
```bash
curl http://localhost:5000/api/recommendations/user123/similar-users | jq '.'
```

### Rafraîchir l'algorithme
```bash
curl -X POST http://localhost:5000/api/recommendations/refresh
```

---

## 🔍 Test 5: Vérifier la Topologie du Gateway

### Via Gateway
```bash
# Affiche les logs du gateway
docker logs api_gateway -f

# Affiche les requêtes qui arrivent au gateway
```

### Appels directs AUX services (comparaison)
```bash
# ❌ NE PAS faire en production
# Juste pour vérifier que les services répondent en interne

# Product API direct
curl http://localhost:5001/api/products

# Order API direct
curl http://localhost:5002/api/orders

# Recommendation API direct
curl http://localhost:5003/api/recommendations/user123
```

---

## 📊 Test 6: Flux Complet - Scénario Utilisateur

```bash
#!/bin/bash

API="http://localhost:5000"

echo "=== 1. Lister les produits ==="
curl -s $API/api/products | jq '.[] | {id: .id, name: .name, price: .price}'

echo -e "\n=== 2. Créer un produit ==="
PRODUCT=$(curl -s -X POST $API/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Gaming Laptop",
    "description": "High-performance laptop",
    "category": "Electronics",
    "price": 1500,
    "stock": 20,
    "imageUrl": "https://example.com/laptop.jpg"
  }')
PRODUCT_ID=$(echo $PRODUCT | jq -r '.id')
echo "Product créé: $PRODUCT_ID"

echo -e "\n=== 3. Créer une commande avec ce produit ==="
ORDER=$(curl -s -X POST $API/api/orders \
  -H "Content-Type: application/json" \
  -d "{
    \"userId\": \"user123\",
    \"items\": [
      {
        \"productId\": \"$PRODUCT_ID\",
        \"quantity\": 1,
        \"price\": 1500
      }
    ],
    \"shippingAddress\": \"123 Main St\",
    \"paymentInfo\": {
      \"cardNumber\": \"4111111111111111\",
      \"cardHolder\": \"John Doe\",
      \"cvv\": \"123\",
      \"expiryDate\": \"12/25\"
    }
  }")
ORDER_ID=$(echo $ORDER | jq -r '.id')
echo "Commande créée: $ORDER_ID"

echo -e "\n=== 4. Vérifier le stock du produit ==="
curl -s $API/api/products/$PRODUCT_ID | jq '{id: .id, name: .name, stock: .stock}'

echo -e "\n=== 5. Obtenir recommandations pour l'utilisateur ==="
curl -s $API/api/recommendations/user123 | jq '.'

echo -e "\n=== Scénario complet réussi! ==="
```

---

## 🚨 Test 7: Vérifier les Erreurs Communes

### ❌ Service non accessible
```bash
# Si vous obtenez 504 Bad Gateway
curl -v http://localhost:5000/api/products

# Vérificez que les containers sont en cours d'exécution
docker-compose ps

# Vérifiez les logs
docker logs api_gateway
docker logs product_api
```

### ❌ Port déjà utilisé
```bash
# Si port 5000 utilisé
lsof -i :5000

# Arrêtez les services
docker-compose down

# Relancez
docker-compose up --build
```

### ❌ Services non en réseau
```bash
# Vérifiez la connectivité interne
docker exec api_gateway ping product-api

# Vérifiez la configuration du gateway
docker exec api_gateway cat /app/appsettings.json
```

---

## 📈 Test 8: Performance - Load Testing

```bash
# Installation Apache Bench
apt-get install apache2-utils  # Linux
brew install httpd             # macOS

# Test simple
ab -n 100 -c 10 http://localhost:5000/api/products

# Test avec POST
ab -n 50 -c 5 -p payload.json http://localhost:5000/api/orders
```

---

## 🔧 Test 9: Vérifier la Communication Inter-Services

### Order → Product (via réseau interne)
```bash
# Entrer dans le container Order
docker exec -it order_api /bin/sh

# Tester la connexion à Product
curl http://product-api:5001/api/products

# Sortie: devrait retourner les produits
```

### Recommendation → Product (via réseau interne)
```bash
# Entrer dans le container Recommendation
docker exec -it recommendation_api /bin/sh

# Tester la connexion
curl http://product-api:5001/api/products
```

---

## ✨ Test 10: Monitoring Centralisé

```bash
# Logs du gateway en temps réel
docker logs -f api_gateway

# Dans un autre terminal - faire une requête
curl http://localhost:5000/api/products

# Vous devriez voir les logs du gateway affichant:
# Gateway: GET /api/products
# Gateway Response: 200
```

---

## ✅ Checklist de Validation

```
[ ] Gateway répond sur port 5000
[ ] Health check: /health retourne 200
[ ] Product Service accessible via /api/products
[ ] Order Service accessible via /api/orders
[ ] Recommendation Service accessible via /api/recommendations
[ ] Créer un produit → OK
[ ] Créer une commande → OK (vérifie les stocks)
[ ] Obtenir recommandations → OK
[ ] Les logs du gateway affichent les requêtes
[ ] Services internes communiquent via nom DNS (product-api, order-api, etc)
[ ] Les clients externes utilisent UNIQUEMENT le Gateway
```

---

## 📝 Résumé

✅ **Clients externes:**
- Accèdent UNIQUEMENT via le Gateway: `http://localhost:5000`
- Ne connaissent pas les ports des services internes

✅ **Services internes (Docker network):**
- Communiquent via noms DNS: `product-api:5001`, `order-api:5002`
- Pas d'exposition directe des ports

✅ **Gateway (YARP):**
- Routage unique point d'entrée
- Logging centralisé
- Health checks actifs

