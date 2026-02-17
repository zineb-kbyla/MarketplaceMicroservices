# 🚪 API Gateway - Marketplace Microservices

API Gateway (YARP) - Point d'entrée centralisé pour tous les microservices

## 📋 Vue d'ensemble

Le API Gateway agit comme point d'entrée unique pour toutes les requêtes clients vers les microservices.

## 🏗️ Architecture

```
Client/Frontend
    ↓
API Gateway (Port 5000)
    ├─→ Product API (Port 5001)
    ├─→ Order API (Port 5002)
    └─→ Recommendation API (Port 5003)
```

## 🛣️ Routes Disponibles

### Product Service
```
GET    /api/products              - Lister tous les produits
GET    /api/products/{id}         - Récupérer un produit
POST   /api/products              - Créer un produit
PUT    /api/products/{id}         - Mettre à jour un produit
DELETE /api/products/{id}         - Supprimer un produit
GET    /api/products/category/{cat} - Produits par catégorie
GET    /api/products/search       - Recherche
POST   /api/products/{id}/decrement-stock - Décrémenter stock
```

### Order Service
```
GET    /api/orders                - Lister commandes
GET    /api/orders/{id}           - Détail commande
POST   /api/orders                - Créer commande
PUT    /api/orders/{id}           - Mettre à jour commande
DELETE /api/orders/{id}           - Annuler commande
PUT    /api/orders/{id}/status    - Changer statut
```

### Recommendation Service
```
GET    /api/recommendations/{userId}  - Obtenir recommandations
GET    /api/recommendations/{userId}/similar-users - Utilisateurs similaires
POST   /api/recommendations/refresh    - Rafraîchir algorithme
```

## 🚀 Démarrage

### Mode Développement (Local)
```bash
dotnet run
```
- Gateway: http://localhost:5000
- OpenAPI: https://localhost:5000/scalar/v1

### Avec Docker
```bash
docker build -t marketplace-gateway .
docker run -p 5000:5000 \
  --network marketplace_network \
  marketplace-gateway
```

### Vérifier la santé des services
```bash
# Via Gateway
curl http://localhost:5000/api/products
curl http://localhost:5000/api/orders
curl http://localhost:5000/api/recommendations/user123
```

## 📝 Configuration

### appsettings.json
- Routes mapping vers les services
- Health checks
- Timeouts (30s par défaut)
- Load balancing future

### Services Adresses
```json
{
  "Product": "http://localhost:5001",
  "Order": "http://localhost:5002",
  "Recommendation": "http://localhost:5003"
}
```

## ✨ Fonctionnalités

- ✅ Routage par chemin (`/api/products/*`, `/api/orders/*`, etc.)
- ✅ CORS Activé
- ✅ Health Checks actifs
- ✅ Logging centralisé
- ✅ Gestion des timeouts
- 🔄 Load balancing pour scale (à implémenter)
- 🔐 Rate limiting (à implémenter)
- 🔄 Retry logic (à implémenter)

## 🔧 Extension Future

```csharp
// Load balancing
"LoadBalancingPolicy": "RoundRobin"

// Retry policy
"HttpClientConfig": {
  "DangerousAcceptAnyServerCertificate": false,
  "MaxRetries": 3
}

// Rate limiting
app.UseRateLimiter();
```

## 📊 Monitoring

Logs disponibles via console/fichier:
```
Gateway: GET /api/products
Gateway Response: 200
Gateway: POST /api/orders
Gateway Response: 201
```

## 🆘 Troubleshooting

### Service non accessible
```bash
# Vérifier service actif
curl http://localhost:5001/health
curl http://localhost:5002/health
curl http://localhost:5003/health
```

### Port déjà utilisé
```bash
# Changer port dans launchSettings.json
"applicationUrl": "https://localhost:5000"
```

### CORS issue
Le gateway accepte toutes les origines. En production:
```csharp
policy.WithOrigins("https://yourdomain.com")
      .AllowAnyMethod()
      .AllowAnyHeader();
```

## 📚 Ressources

- [YARP Documentation](https://microsoft.github.io/reverse-proxy/)
- [Scalar OpenAPI UI](https://scalar.com/)

