# 🧪 Test Guide - Recommendation API Microservice

## ✅ Services Status

Tous les services sont maintenant en cours d'exécution dans Docker:

| Service | Port | Status | URL |
|---------|------|--------|-----|
| **Recommendation API** | 5003 | ✅ Running | http://localhost:5003 |
| **Product API** | 5001 | ✅ Running | http://localhost:5001 |
| **Order API** | 5002 | ✅ Running | http://localhost:5002 |
| **MongoDB** | 27017 | ✅ Running | mongodb://localhost:27017 |
| **RabbitMQ** | 5672 | ✅ Running | amqp://localhost:5672 |
| **RabbitMQ Manager** | 15672 | ✅ Running | http://localhost:15672 |
| **Neo4j** | 7687 | ✅ Running | bolt://localhost:7687 |
| **Neo4j Browser** | 7474 | ✅ Running | http://localhost:7474 |

---

## 🚀 Testing Endpoints

### Base URL: `http://localhost:5003/api/recommendations`

---

## 📌 Test 1: Get Personalized Recommendations

**Endpoint**: `GET /api/recommendations/{userId}`

**URL**: 
```
http://localhost:5003/api/recommendations/user-123?limit=10
```

**PowerShell**:
```powershell
$response = Invoke-WebRequest `
  -Uri "http://localhost:5003/api/recommendations/user-123?limit=10" `
  -Method GET `
  -UseBasicParsing

Write-Host "Status Code: $($response.StatusCode)"
Write-Host "Response: $($response.Content)"
```

**cURL**:
```bash
curl -X GET "http://localhost:5003/api/recommendations/user-123?limit=10"
```

**Expected Response** (200 OK):
```json
[]
```
*(Empty array because no purchase history yet)*

---

## 📌 Test 2: Record Product View (Event)

**Endpoint**: `POST /api/recommendations/view`

**PowerShell**:
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

Write-Host "Status Code: $($response.StatusCode)"
```

**cURL**:
```bash
curl -X POST "http://localhost:5003/api/recommendations/view" \
  -H "Content-Type: application/json" \
  -d '{"userId":"user-123","productId":"prod-001","duration":120,"source":"web"}'
```

**Expected**: 204 No Content

---

## 📌 Test 3: Record Purchase (Event)

**Endpoint**: `POST /api/recommendations/purchase`

**PowerShell**:
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

Write-Host "Status Code: $($response.StatusCode)"
```

**cURL**:
```bash
curl -X POST "http://localhost:5003/api/recommendations/purchase" \
  -H "Content-Type: application/json" \
  -d '{
    "userId":"user-123",
    "orderId":"order-2024-001",
    "items":[
      {"productId":"prod-001","quantity":1,"price":1299.99},
      {"productId":"prod-002","quantity":2,"price":49.99}
    ]
  }'
```

**Expected**: 204 No Content

---

## 📌 Test 4: Get Similar Products

**Endpoint**: `GET /api/recommendations/similar/{productId}`

**PowerShell**:
```powershell
$response = Invoke-WebRequest `
  -Uri "http://localhost:5003/api/recommendations/similar/prod-001?limit=5" `
  -Method GET `
  -UseBasicParsing

Write-Host "Status Code: $($response.StatusCode)"
Write-Host "Response: $($response.Content | ConvertFrom-Json | ConvertTo-Json)"
```

**cURL**:
```bash
curl -X GET "http://localhost:5003/api/recommendations/similar/prod-001?limit=5"
```

**Expected**: 200 OK with array

---

## 📌 Test 5: Get Trending Products

**Endpoint**: `GET /api/recommendations/trending`

**PowerShell**:
```powershell
$response = Invoke-WebRequest `
  -Uri "http://localhost:5003/api/recommendations/trending?days=7&limit=10" `
  -Method GET `
  -UseBasicParsing

Write-Host "Status Code: $($response.StatusCode)"
Write-Host "Response: $($response.Content | ConvertFrom-Json | ConvertTo-Json)"
```

**cURL**:
```bash
curl -X GET "http://localhost:5003/api/recommendations/trending?days=7&limit=10"
```

**Expected**: 200 OK with trending products array

---

## 📌 Test 6: Get User History

**Endpoint**: `GET /api/recommendations/history/{userId}`

**PowerShell**:
```powershell
$response = Invoke-WebRequest `
  -Uri "http://localhost:5003/api/recommendations/history/user-123?limit=20" `
  -Method GET `
  -UseBasicParsing

Write-Host "Status Code: $($response.StatusCode)"
Write-Host "Response: $($response.Content | ConvertFrom-Json | ConvertTo-Json)"
```

**cURL**:
```bash
curl -X GET "http://localhost:5003/api/recommendations/history/user-123?limit=20"
```

**Expected**: 200 OK with user history

---

## 🔄 Complete Test Scenario (Recommended)

**Sequence pour tester le flux complet:**

### 1️⃣ Créer des produits via Product API
```powershell
# POST http://localhost:5001/api/products
```

### 2️⃣ Enregistrer des vues de produits
```powershell
Invoke-WebRequest `
  -Uri "http://localhost:5003/api/recommendations/view" `
  -Method POST `
  -Header @{"Content-Type" = "application/json"} `
  -Body (@{userId="user-1"; productId="prod-1"; duration=120; source="web"} | ConvertTo-Json) `
  -UseBasicParsing
```

### 3️⃣ Créer une commande via Order API
```powershell
# POST http://localhost:5002/api/orders
```

### 4️⃣ Enregistrer l'achat
```powershell
Invoke-WebRequest `
  -Uri "http://localhost:5003/api/recommendations/purchase" `
  -Method POST `
  -Header @{"Content-Type" = "application/json"} `
  -Body (@{userId="user-1"; orderId="order-1"; items=@(@{productId="prod-1"; quantity=1; price=99.99})} | ConvertTo-Json -Depth 3) `
  -UseBasicParsing
```

### 5️⃣ Récupérer les recommandations personnalisées
```powershell
Invoke-WebRequest `
  -Uri "http://localhost:5003/api/recommendations/user-1?limit=5" `
  -Method GET `
  -UseBasicParsing | Select-Object -ExpandProperty Content | ConvertFrom-Json
```

---

## 🐛 Troubleshooting

### Erreur 500 - "Error retrieving recommendations"

**Cause**: Neo4j n'est pas accessible

**Solution**:
```bash
# Vérifier que Neo4j est accessible
docker ps | grep neo4j

# Vérifier les logs
docker logs marketplace_neo4j

# Redémarrer Neo4j
docker restart marketplace_neo4j
```

### Erreur: "Connection timeout"

**Cause**: Les services ne sont pas sur le même réseau Docker

**Solution**:
```bash
# Vérifier le réseau
docker network ls | grep marketplace

# Relancer docker-compose
docker compose down
docker compose up -d
```

### RabbitMQ Connection Error (Warning)

**Note**: C'est normal en développement. Le service continue de fonctionner.

---

## 📊 Neo4j Browser

Visualisez les données dans Neo4j:

1. Accédez à: http://localhost:7474
2. Connectez-vous avec:
   - Username: `neo4j`
   - Password: `password`
3. Exécutez des Cypher queries:

```cypher
-- Voir tous les utilisateurs
MATCH (u:User) RETURN u;

-- Voir les relations d'achat
MATCH (u:User)-[r:PURCHASED]->(p:Product) RETURN u, r, p;

-- Voir les utilisateurs similaires
MATCH (u:User)-[:PURCHASED]->(p:Product)<-[:PURCHASED]-(similar:User)
WHERE u.userId = 'user-123'
RETURN similar, count(p) as commonPurchases
ORDER BY commonPurchases DESC;
```

---

## 🔗 RabbitMQ Management

Gérez les files d'attente:

1. Accédez à: http://localhost:15672
2. Connectez-vous avec:
   - Username: `guest`
   - Password: `guest`
3. Consultez:
   - Exchanges: `marketplace.events`
   - Queues: `recommendation-service`
   - Current messages

---

## 📝 Postman Collection

Importez cette URL dans Postman pour avoir tous les endpoints:

```
http://localhost:5003/swagger/v1/swagger.json
```

Ou utilisez le fichier POSTMAN.md pour les examples manuels.

---

## ✅ Checklist de vérification

- [ ] Recommendation API répond sur http://localhost:5003
- [ ] Neo4j est accessible (port 7687)
- [ ] RabbitMQ est accessible (port 5672)
- [ ] POST /view retourne 204
- [ ] POST /purchase retourne 204
- [ ] GET /trending retourne 200
- [ ] GET /similar/{id} retourne 200
- [ ] GET /{userId} retourne 200 (au moins une donnée)
- [ ] GET /history/{userId} retourne 200

---

## 🎯 Prochaines étapes

1. **Créer des données de test** dans Product API
2. **Simuler des vues et achats** via Recommendation API
3. **Vérifier les relations** dans Neo4j Browser
4. **Analyser les performances** des requêtes

Bon testing! 🚀
