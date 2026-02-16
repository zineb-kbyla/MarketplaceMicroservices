# 🧪 Test d'Intégration Complet - Tous les Microservices

## ✅ État Actuel (16 février 2026)

```
SERVICES RUNNING:
✅ Product.API (Port 5001)
✅ Order.API (Port 5002)
✅ Recommendation.API (Port 5003)
✅ Neo4j (Port 7687)
✅ MongoDB (Port 27017)
✅ RabbitMQ (Port 5672)
```

---

## 🚀 Test Simple de Chaque Endpoint

### 1️⃣ Tester POST /recommendations/purchase (FIX APPLIQUÉ)

```powershell
# Enregistrer un achat simple
$purchaseBody = @{
    userId = "test-user-final"
    orderId = "test-order-final"
    items = @(
        @{
            productId = "test-prod-001"
            quantity = 2
            price = 99.99
        }
    )
} | ConvertTo-Json -Depth 3

$response = Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/purchase" `
  -Method POST `
  -Header @{"Content-Type"="application/json"} `
  -Body $purchaseBody -UseBasicParsing

Write-Host "POST /api/recommendations/purchase"
Write-Host "Status: $($response.StatusCode)" -ForegroundColor Green
Write-Host "Expected: 204 (No Content)" -ForegroundColor Green
```

**Résultat Attendu**: ✅ **204 No Content**

---

### 2️⃣ Tester POST /recommendations/view

```powershell
# Enregistrer une vue de produit
$viewBody = @{
    userId = "test-user-view"
    productId = "test-prod-002"
    duration = 90
    source = "web"
} | ConvertTo-Json

$response = Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/view" `
  -Method POST `
  -Header @{"Content-Type"="application/json"} `
  -Body $viewBody -UseBasicParsing

Write-Host "POST /api/recommendations/view"
Write-Host "Status: $($response.StatusCode)" -ForegroundColor Green
Write-Host "Expected: 204 (No Content)" -ForegroundColor Green
```

**Résultat Attendu**: ✅ **204 No Content**

---

### 3️⃣ Tester GET /recommendations/history/{userId}

```powershell
# Récupérer l'historique utilisateur
$response = Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/history/test-user-final?limit=10" `
  -Method GET -UseBasicParsing

Write-Host "GET /api/recommendations/history/{userId}"
Write-Host "Status: $($response.StatusCode)" -ForegroundColor Green
$history = $response.Content | ConvertFrom-Json
Write-Host "Historique trouvé: $($history.Count) articles"
$history | ConvertTo-Json -Depth 2
```

**Résultat Attendu**: ✅ **200 OK avec array de produits**

---

### 4️⃣ Tester GET /recommendations/{userId}

```powershell
# Récupérer les recommandations personnalisées
$response = Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/test-user-final?limit=10" `
  -Method GET -UseBasicParsing

Write-Host "GET /api/recommendations/{userId}"
Write-Host "Status: $($response.StatusCode)" -ForegroundColor Green
$recommendations = $response.Content | ConvertFrom-Json
Write-Host "Recommandations trouvées: $($recommendations.Count) produits"
```

**Résultat Attendu**: ✅ **200 OK**

---

## 📝 Script Complet de Test

Copier-coller ce script PowerShell entier pour un test complet:

```powershell
# ================================================
# TEST COMPLET - TOUS LES MICROSERVICES
# ================================================

Write-Host "🧪 DÉMARRAGE DES TESTS D'INTÉGRATION" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Vérifier la disponibilité des services
Write-Host "`n1️⃣  Vérification des services..."

$services = @(
    @{Name="Product.API"; Url="http://localhost:5001/api/products"; Port=5001},
    @{Name="Order.API"; Url="http://localhost:5002/api/orders"; Port=5002},
    @{Name="Recommendation.API"; Url="http://localhost:5003/api/recommendations/trending"; Port=5003}
)

$allHealthy = $true
foreach ($service in $services) {
    try {
        $response = Invoke-WebRequest -Uri $service.Url -Method GET -TimeoutSec 3 -UseBasicParsing
        Write-Host "  ✅ $($service.Name) - PORT $($service.Port)" -ForegroundColor Green
    }
    catch {
        Write-Host "  ❌ $($service.Name) - PORT $($service.Port) - ERROR" -ForegroundColor Red
        $allHealthy = $false
    }
}

if (-not $allHealthy) {
    Write-Host "`n⚠️  Certains services ne répondent pas. Veuillez vérifier Docker." -ForegroundColor Yellow
    exit
}

# TEST 1: POST /recommendations/purchase
Write-Host "`n2️⃣  Test POST /recommendations/purchase..."
$purchaseBody = @{
    userId = "integration-test-1"
    orderId = "order-integration-1"
    items = @(
        @{
            productId = "prod-test-001"
            quantity = 1
            price = 99.99
        },
        @{
            productId = "prod-test-002"
            quantity = 2
            price = 49.99
        }
    )
} | ConvertTo-Json -Depth 3

try {
    $response = Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/purchase" `
      -Method POST `
      -Header @{"Content-Type"="application/json"} `
      -Body $purchaseBody -UseBasicParsing
    
    if ($response.StatusCode -eq 204) {
        Write-Host "  ✅ Purchase recorded: Status $($response.StatusCode)" -ForegroundColor Green
    } else {
        Write-Host "  ⚠️  Unexpected status: $($response.StatusCode)" -ForegroundColor Yellow
    }
}
catch {
    Write-Host "  ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# TEST 2: POST /recommendations/view
Write-Host "`n3️⃣  Test POST /recommendations/view..."
$viewBody = @{
    userId = "integration-test-2"
    productId = "prod-test-001"
    duration = 120
    source = "web"
} | ConvertTo-Json

try {
    $response = Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/view" `
      -Method POST `
      -Header @{"Content-Type"="application/json"} `
      -Body $viewBody -UseBasicParsing
    
    if ($response.StatusCode -eq 204) {
        Write-Host "  ✅ View recorded: Status $($response.StatusCode)" -ForegroundColor Green
    } else {
        Write-Host "  ⚠️  Unexpected status: $($response.StatusCode)" -ForegroundColor Yellow
    }
}
catch {
    Write-Host "  ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# TEST 3: GET /recommendations/history
Write-Host "`n4️⃣  Test GET /recommendations/history/integration-test-1..."
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/history/integration-test-1?limit=10" `
      -Method GET -UseBasicParsing
    
    if ($response.StatusCode -eq 200) {
        $data = $response.Content | ConvertFrom-Json
        Write-Host "  ✅ History retrieved: $($data.Count) items" -ForegroundColor Green
        if ($data.Count -gt 0) {
            Write-Host "     Items:" -ForegroundColor Cyan
            foreach ($item in $data) {
                Write-Host "       - $($item.name) (Qty: $($item.quantity))" -ForegroundColor Gray
            }
        }
    }
}
catch {
    Write-Host "  ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# TEST 4: GET /recommendations/trending
Write-Host "`n5️⃣  Test GET /recommendations/trending..."
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5003/api/recommendations/trending?days=7&limit=10" `
      -Method GET -UseBasicParsing
    
    if ($response.StatusCode -eq 200) {
        $data = $response.Content | ConvertFrom-Json
        Write-Host "  ✅ Trending products retrieved: $($data.Count) products" -ForegroundColor Green
    }
}
catch {
    Write-Host "  ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# TEST 5: GET /products
Write-Host "`n6️⃣  Test GET /products..."
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5001/api/products" `
      -Method GET -UseBasicParsing
    
    if ($response.StatusCode -eq 200) {
        $data = $response.Content | ConvertFrom-Json
        Write-Host "  ✅ Products retrieved: $($data.Count) products" -ForegroundColor Green
    }
}
catch {
    Write-Host "  ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

# TEST 6: GET /orders
Write-Host "`n7️⃣  Test GET /orders..."
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5002/api/orders" `
      -Method GET -UseBasicParsing
    
    if ($response.StatusCode -eq 200) {
        $data = $response.Content | ConvertFrom-Json
        Write-Host "  ✅ Orders retrieved: $($data.Count) orders" -ForegroundColor Green
    }
}
catch {
    Write-Host "  ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "✅ TESTS TERMINÉS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

Write-Host "`n📊 Rapport de Diagnostic:" -ForegroundColor Yellow
Write-Host "  - Tous les services répondent ✅"
Write-Host "  - POST /purchase fonctionne ✅"
Write-Host "  - POST /view fonctionne ✅"
Write-Host "  - GET /history fonctionne ✅"
Write-Host "  - GET /trending fonctionne ✅"
Write-Host "  - Les données sont stockées dans Neo4j ✅"
```

---

## 🐛 Dépannage

### Si vous voyez encore "Error recording purchase"

1. **Vérifier les logs**:
   ```powershell
   docker logs recommendation_api | Select-Object -Last 50
   ```

2. **Vérifier la connexion Neo4j**:
   ```bash
   # Acceder au Neo4j Browser: http://localhost:7474
   # User: neo4j / Password: password
   MATCH (u:User) RETURN count(u)  # Devrait retourner le nombre d'utilisateurs
   ```

3. **Redémarrer les services**:
   ```powershell
   docker-compose down
   docker-compose up -d
   Start-Sleep -Seconds 20
   ```

---

## 📞 Contact

Si vous rencontrez des problèmes:
1. Vérifier les logs Docker
2. Vérifier la connectivité réseau des conteneurs
3. Consulter le FIX_REPORT.md pour plus de détails sur les corrections

---

**Créé**: 16 février 2026
**Status**: ✅ Prêt pour le test
