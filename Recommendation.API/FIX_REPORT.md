# 🔧 Fix Report - Recommendation.API Neo4j Issues

**Date**: 16 février 2026
**Status**: ✅ FIXED

---

## 📋 Problème Identifié

Erreur dans les endpoints `POST /api/recommendations/purchase` et `POST /api/recommendations/view`:

```
"Error recording purchase" (500 Internal Server Error)
```

### Root Cause (Cause Racine)

Les requêtes Cypher Neo4j avaient un **scope issue** - la variable `r` n'était pas disponible après la clause `WITH` :

**Requête Cassée:**
```cypher
MERGE (u)-[r:PURCHASED]->(p)
SET r.orderId = $orderId, ...
WITH p                    # ❌ Variable `r` perdue ici
SET p.purchaseCount = ... 
RETURN r                  # ❌ Erreur: `r` non défini
```

---

## ✅ Solution Appliquée

### Fix 1: RecordViewAsync (Ligne 289-302)

**AVANT:**
```csharp
const string query = @"
    MERGE (u)-[r:VIEWED]->(p)
    SET r.viewedAt = datetime(), ...
    WITH p          // ❌ r is lost here
    SET p.viewCount = COALESCE(p.viewCount, 0) + 1
    RETURN r        // ❌ Error: r not defined
```

**APRÈS:**
```csharp
const string query = @"
    MERGE (u)-[r:VIEWED]->(p)
    SET r.viewedAt = datetime(), ...
    WITH p, r       // ✅ Keep r in scope
    SET p.viewCount = COALESCE(p.viewCount, 0) + 1
    RETURN p        // ✅ Return p instead of r
```

---

### Fix 2: RecordPurchaseAsync (Ligne 239-252)

**AVANT:**
```csharp
const string query = @"
    MERGE (u)-[r:PURCHASED]->(p)
    SET r.orderId = $orderId, ...
    WITH p          // ❌ r is lost here
    SET p.purchaseCount = ...
    RETURN r        // ❌ Error: r not defined
```

**APRÈS:**
```csharp
const string query = @"
    MERGE (u)-[r:PURCHASED]->(p)
    SET r.orderId = $orderId, ...
    WITH p, r       // ✅ Keep r in scope
    SET p.purchaseCount = ...
    RETURN p        // ✅ Return p instead of r
```

---

## 🧪 Tests de Validation

### Test 1: Enregistrer une Vue ✅

```powershell
$viewBody = @{
    userId = "user-123"
    productId = "prod-001"
    duration = 120
    source = "web"
} | ConvertTo-Json

$response = Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/view" `
  -Method POST `
  -Header @{"Content-Type"="application/json"} `
  -Body $viewBody -UseBasicParsing

$response.StatusCode  # ✅ 204 No Content
```

**Résultat**: ✅ **204 No Content** (SUCCESS)

---

### Test 2: Enregistrer un Achat ✅

```powershell
$purchaseBody = @{
    userId = "user-123"
    orderId = "order-001"
    items = @(
        @{
            productId = "prod-001"
            quantity = 1
            price = 1299.99
        }
    )
} | ConvertTo-Json -Depth 3

$response = Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/purchase" `
  -Method POST `
  -Header @{"Content-Type"="application/json"} `
  -Body $purchaseBody -UseBasicParsing

$response.StatusCode  # ✅ 204 No Content
```

**Résultat**: ✅ **204 No Content** (SUCCESS)

---

### Test 3: Flux Complet d'Intégration ✅

```powershell
# Étape 1: Créer un produit
$product = @{
    name = "Test Product"
    description = "Test Description"
    category = "Test"
    price = 99.99
    stock = 10
    imageUrl = "http://example.com/test.jpg"
} | ConvertTo-Json

$prodResponse = Invoke-WebRequest -Uri "http://localhost:5001/api/products" `
  -Method POST `
  -Header @{"Content-Type"="application/json"} `
  -Body $product -UseBasicParsing
$prodId = ($prodResponse.Content | ConvertFrom-Json).id
Write-Host "✅ Produit créé: $prodId"

# Étape 2: Enregistrer une vue
$view = @{
    userId = "integration-test-user"
    productId = $prodId
    duration = 60
    source = "web"
} | ConvertTo-Json

$viewResponse = Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/view" `
  -Method POST `
  -Header @{"Content-Type"="application/json"} `
  -Body $view -UseBasicParsing
Write-Host "✅ Vue enregistrée: Status $($viewResponse.StatusCode)"

# Étape 3: Créer une commande
$order = @{
    userId = "integration-test-user"
    userName = "Test User"
    items = @(
        @{
            productId = $prodId
            productName = "Test Product"
            quantity = 1
            unitPrice = 99.99
            totalPrice = 99.99
        }
    )
    shippingAddress = @{
        street = "123 Test St"
        city = "Paris"
        state = "Île-de-France"
        country = "France"
        zipCode = "75001"
        phoneNumber = "+33123456789"
    }
    paymentInfo = @{
        cardName = "Test User"
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

# Étape 4: Enregistrer l'achat
$purchase = @{
    userId = "integration-test-user"
    orderId = $orderId
    items = @(
        @{
            productId = $prodId
            quantity = 1
            price = 99.99
        }
    )
} | ConvertTo-Json -Depth 3

$purchaseResponse = Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/purchase" `
  -Method POST `
  -Header @{"Content-Type"="application/json"} `
  -Body $purchase -UseBasicParsing
Write-Host "✅ Achat enregistré: Status $($purchaseResponse.StatusCode)"

# Étape 5: Récupérer l'historique
$historyResponse = Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/history/integration-test-user" `
  -Method GET -UseBasicParsing
$history = $historyResponse.Content | ConvertFrom-Json
Write-Host "✅ Historique récupéré: $($history.Count) articles"
$history | ConvertTo-Json -Depth 2
```

**Résultat**: ✅ **Tous les endpoints fonctionnent correctement**

---

## 📊 Comparaison Avant/Après

| Endpoint | Avant Fix | Après Fix |
|---|---|---|
| `POST /api/recommendations/view` | ❌ 500 Error | ✅ 204 Success |
| `POST /api/recommendations/purchase` | ❌ 500 Error | ✅ 204 Success |
| `GET /api/recommendations/history/{userId}` | ⚠️ Données vides | ✅ Fonctionnel |
| `GET /api/recommendations/{userId}` | ⚠️ Données vides | ✅ Fonctionnel |

---

## 🔧 Détails Techniques

### Fichiers Modifiés

1. **[Recommendation.API/Infrastructure/Repositories/RecommendationRepository.cs](Recommendation.API/Infrastructure/Repositories/RecommendationRepository.cs)**
   - Ligne 252: `WITH p, r` (ajout de `r`)
   - Ligne 253: `RETURN p` (changement de `r` à `p`)
   - Ligne 289: `WITH p, r` (ajout de `r`)
   - Ligne 290: `RETURN p` (changement de `r` à `p`)

### Cypher Query Patterns

**Pattern Correct:**
```cypher
MERGE (u:User {userId: $userId})-[r:RELATIONSHIP]->(p:Product)
SET r.prop = value          -- Setup the relationship
WITH r, p                   -- Keep BOTH r and p in scope
SET p.count = ...          -- Modify the product
RETURN p                   -- Return what you need
```

**Pattern Cassé (ÉVITER):**
```cypher
MERGE (u:User)-[r:R]->(p:Product)
SET r.prop = value
WITH p                     -- ❌ r is lost here
SET p.count = ...
RETURN r                   -- ❌ Error: r undefined
```

---

## 🚀 Déploiement

### Docker Build & Deployment
```bash
cd d:\Cours Jobintech\ProjetMarktplace_Net
docker-compose build --no-cache recommendation-api
docker-compose up -d recommendation-api
```

### Verification
```bash
docker logs recommendation_api | grep -i error
# Should NOT see: "Variable `r` not defined"
```

---

## 📋 Checklist de Post-Fix

- ✅ Neo4j Cypher queries corriges
- ✅ Dockerfile rebuilded
- ✅ Container restarted
- ✅ POST /view endpoint tested (204 Success)
- ✅ POST /purchase endpoint tested (204 Success)
- ✅ GET /history endpoint working
- ✅ GET /recommendations endpoint working
- ✅ Full integration flow validated

---

## 💡 Leçons Apprises

1. **Neo4j Scope Rules**: Variables introduites par `MERGE` ou `MATCH` doivent être explicitement incluses dans les clauses `WITH` pour rester accessibles après
2. **Cypher Best Practices**: Toujours inclure toutes les variables nécessaires dans `WITH` clause
3. **Error Messages**: Le message d'erreur Neo4j indiquait clairement le problème (ligne/colonne du query)

---

## 📞 Support

Si vous rencontrez d'autres erreurs avec les Neo4j queries:

1. Vérifier le **scope des variables** après WITH
2. Vérifier que les clauses SET ne référencent que des variables en scope
3. Utiliser `RETURN` pour retourner uniquement les variables nécessaires

---

**Statut Final**: ✅ **RÉSOLU ET TESTÉ**

Tous les endpoints Recommendation.API fonctionnent correctement et enregistrent les données dans Neo4j sans erreurs.
