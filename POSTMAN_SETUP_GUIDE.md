# 🚀 Utiliser la Collection Postman pour Tester

## 📥 Importer la Collection

### Étape 1: Ouvrir Postman
```bash
# Si Postman n'est pas installé:
# Télécharger depuis: https://www.postman.com/downloads/
```

### Étape 2: Importer la Collection

**Méthode A - Via le fichier:**
1. Cliquez sur **File** → **Import**
2. Sélectionnez le fichier `Selective-Microservices-Testing.postman_collection.json`
3. La collection est importée ✅

**Méthode B - Via URL (recommandé):**
1. Cliquez sur **Import**
2. Choisissez l'onglet **Link**
3. Collez l'URL si disponible ou uploadez le fichier

---

## ⚙️ Configurer les Variables

Après import, **définir les variables d'environnement:**

| Variable | Valeur | Exemple |
|----------|--------|---------|
| `base_url` | URL du gateway | `http://localhost:5000` |
| `product_id` | ID d'un produit | `product1` ou `507f1f77bcf86cd799439011` |
| `order_id` | ID d'une commande | Sera généré après création |
| `user_id` | ID utilisateur test | `test_user_001` ou `user123` |

**Dans Postman:**
1. Cliquez sur **Environments** (à gauche)
2. Créez un nouvel environnement: **"Marketplace-Test"**
3. Remplissez les variables ci-dessus
4. Sélectionnez cet environnement (coin supérieur droit)

---

## 🧪 Test Options

### ✅ Option 1: Product + Order (Recommandé pour débuter)

```bash
# Terminal 1: Démarrer les services
cd ProjetMarktplace_Net
docker-compose up -d api-gateway mongodb rabbitmq product-api order-api

# Attendre 20 secondes que tout démarre
```

**Tests Postman:**
1. ✅ Setup → Health Check - APIGateway
2. ✅ Product Service → Get All Products
3. ✅ Order Service → Create Order
4. ✅ Order Service → Get User Orders
5. ✅ Order Service → Update Order Status

---

### ✅ Option 2: Product + Recommendation

```bash
# Terminal 1: Démarrer les services
cd ProjetMarktplace_Net
docker-compose up -d api-gateway mongodb neo4j rabbitmq product-api recommendation-api

# Attendre 30 secondes (Neo4j plus lent au démarrage)
```

**Tests Postman:**
1. ✅ Setup → Health Check - APIGateway
2. ✅ Product Service → Get All Products
3. ✅ Product Service → Record Product View
4. ✅ Recommendation Service → Get Recommendations for User
5. ✅ Recommendation Service → Get Trending Products

---

### ✅ Option 3: Tous les 3 Services

```bash
# Terminal 1: Démarrer tous les services
cd ProjetMarktplace_Net
docker-compose up -d

# Attendre 30 secondes
```

**Tester les deux flows:**
- Integration Flows → Flow 1: Product + Order
- Integration Flows → Flow 2: Product + Recommendation

---

## 📍 Étapes Détaillées pour Product + Order

### 1️⃣ Lancer les services

```bash
docker-compose up -d api-gateway mongodb rabbitmq product-api order-api
docker-compose ps
```

**Vérifier que tous sont en état "healthy" ou "Up"**

---

### 2️⃣ Dans Postman

#### A. Récupérer les produits
```
GET http://localhost:5000/api/products
```

Réponse:
```json
{
  "success": true,
  "data": [{
    "id": "507f1f77bcf86cd799439011",
    "name": "Laptop",
    "price": 999.99,
    "stock": 10
  }]
}
```

**Copier l'ID du produit dans `{{product_id}}`**

---

#### B. Créer une commande

Dans Postman, aller à **Order Service → Create Order**

Modifier le body si nécessaire:
- `productId`: utiliser l'ID récupéré
- `userId`: garder `{{user_id}}` ou changer
- Les montants doivent correspondre au produit

**Vérification:**
- Status: **201** (Created)
- Réponse: contient `"orderId"` et `"id"`
- **Copier l'`id` dans `{{order_id}}`**

---

#### C. Récupérer la commande

```
GET http://localhost:5000/api/orders/{{order_id}}
```

Réponse doit afficher la commande créée

---

#### D. Lister les commandes de l'utilisateur

```
GET http://localhost:5000/api/orders/user/{{user_id}}
```

Réponse doit afficher un array avec la commande

---

#### E. Mettre à jour le statut

```
PUT http://localhost:5000/api/orders/{{order_id}}/status
```

Body:
```json
{
  "status": "Shipped",
  "notes": "Commande en route"
}
```

---

## 📊 Vérifier l'Intégration via Logs

### Voir les logs en temps réel

```bash
# Tous les services
docker-compose logs -f

# Service spécifique
docker-compose logs -f order-api
docker-compose logs -f product-api

# Voir RabbitMQ
docker-compose logs -f rabbitmq
```

---

## 🔍 Déboguer les Problèmes

### Le gateway ne répond pas

```bash
# Vérifier le port
netstat -ano | findstr :5000

# Regarder les logs
docker-compose logs api-gateway
```

### Erreur de création de commande

```bash
# Vérifier que MongoDB est actif
docker-compose ps mongodb

# Vérifier les logs d'Order.API
docker-compose logs order-api
```

### Les recommandations sont vides

```bash
# Neo4j peut être lent au démarrage
docker-compose logs neo4j

# Vérifier l'événement a été publié
docker exec marketplace_rabbitmq rabbitmqctl list_queues
```

---

## 💡 Trucs & Astuces Postman

### Sauvegarder automatiquement les IDs

Dans Postman, aller à **Tests** et ajouter:

```javascript
// Pour Create Order
if (pm.response.code === 201) {
    var jsonData = pm.response.json();
    pm.environment.set("order_id", jsonData.data.id);
}
```

### Créer un test automatisé

Dans chaque requête, onglet **Tests**:

```javascript
pm.test("Status is OK", function () {
    pm.response.to.have.status(200);
});

pm.test("Response has data", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.success).to.be.true;
});
```

### Exécuter une collection automatiquement

```bash
# Installer Newman (CLI Postman)
npm install -g newman

# Exécuter la collection
newman run Selective-Microservices-Testing.postman_collection.json \
  -e environment.json \
  --suppress-warnings
```

---

## 📋 Checklist Final

- [ ] Services démarrés et sains (`docker-compose ps`)
- [ ] Collection importée dans Postman
- [ ] Variables d'environnement configurées
- [ ] Health check répond (Status 200)
- [ ] Get Products répond
- [ ] Create Order réussit (Status 201)
- [ ] Récupérer la commande fonctionne
- [ ] Listes des commandes de l'utilisateur fonctionne

---

**Besoin d'aide? Vérifiez les logs ou contactez l'équipe dev!** 🎉
