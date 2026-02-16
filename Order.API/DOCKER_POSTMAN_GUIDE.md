# 🐳 Guide Docker + Postman - Order.API

Guide complet pour démarrer Order.API avec Docker et tester tous les endpoints avec Postman.

---

## 📋 Table des matières

1. [Prérequis](#prérequis)
2. [Démarrage avec Docker](#démarrage-avec-docker)
3. [Vérification des services](#vérification-des-services)
4. [Configuration Postman](#configuration-postman)
5. [Tests des endpoints](#tests-des-endpoints)
6. [Scénarios de test complets](#scénarios-de-test-complets)
7. [Troubleshooting](#troubleshooting)

---

## 🎯 Prérequis

### Logiciels requis

- ✅ **Docker Desktop** installé et démarré
- ✅ **Postman** installé (ou utiliser Postman Web)
- ✅ Ports disponibles: `5002`, `5001`, `27017`, `5672`, `15672`

### Vérifier Docker

```powershell
# Vérifier que Docker est actif
docker --version
docker-compose --version

# Vérifier que Docker Desktop est lancé
docker ps
```

---

## 🚀 Démarrage avec Docker

### Étape 1: Naviguer vers le projet

```powershell
cd "d:\Cours Jobintech\ProjetMarktplace_Net"
```

### Étape 2: Démarrer tous les services

```powershell
# Démarrer MongoDB, RabbitMQ, Product.API et Order.API
docker-compose up -d
```

**Sortie attendue:**
```
[+] Running 5/5
 ✔ Network projetmarktplace_net_marketplace_network  Created
 ✔ Container marketplace_mongodb                     Started
 ✔ Container marketplace_rabbitmq                    Started
 ✔ Container product_api                             Started
 ✔ Container order_api                               Started
```

### Étape 3: Attendre le démarrage (15-30 secondes)

```powershell
# Voir les logs en temps réel
docker-compose logs -f order-api
```

**Logs de succès attendus:**
```
order_api  | info: Microsoft.Hosting.Lifetime[14]
order_api  |       Now listening on: http://[::]:5002
order_api  | info: Microsoft.Hosting.Lifetime[0]
order_api  |       Application started.
```

**Appuyez sur `Ctrl+C` pour arrêter de suivre les logs.**

---

## ✅ Vérification des services

### Vérifier que tous les conteneurs sont actifs

```powershell
docker-compose ps
```

**Sortie attendue:**
```
NAME                  STATUS              PORTS
marketplace_mongodb   Up (healthy)        0.0.0.0:27017->27017/tcp
marketplace_rabbitmq  Up (healthy)        0.0.0.0:5672->5672/tcp, 0.0.0.0:15672->15672/tcp
product_api           Up                  0.0.0.0:5001->5001/tcp
order_api             Up                  0.0.0.0:5002->5002/tcp
```

**⚠️ Important:** Attendez que MongoDB et RabbitMQ affichent `(healthy)` avant de continuer.

### Test rapide de l'API

```powershell
# Test simple avec curl
curl http://localhost:5002/api/orders

# OU avec PowerShell
Invoke-WebRequest -Uri http://localhost:5002/api/orders -Method GET
```

**Réponse attendue:** 
- Status 200 OK
- Liste des commandes (peut être vide `[]` au premier démarrage)

### Ouvrir Swagger/Scalar UI

Ouvrir dans le navigateur:
```
http://localhost:5002/scalar/v1
```

Vous devriez voir l'interface interactive des endpoints.

---

## 🔧 Configuration Postman

### Option 1: Configuration manuelle

#### 1. Créer une nouvelle Collection

- Nom: `Order.API - Docker`
- Description: `Tests pour Order.API avec Docker`

#### 2. Créer un environnement

Nom: `Docker Local`

Variables:

| Variable | Initial Value | Current Value |
|----------|---------------|---------------|
| `baseUrl` | `http://localhost:5002` | `http://localhost:5002` |
| `productUrl` | `http://localhost:5001` | `http://localhost:5001` |
| `orderId` | _vide_ | _vide_ |
| `userId` | `user123` | `user123` |

#### 3. Headers par défaut

Dans la collection, onglet **Variables**:
- Key: `Content-Type`
- Value: `application/json`

### Option 2: Importer la collection (recommandé)

**Créer le fichier de collection Postman:**

Sauvegardez ce JSON dans `Order.API/Order-API-Docker.postman_collection.json`:

```json
{
  "info": {
    "name": "Order.API - Docker",
    "description": "Tests Order.API avec Docker",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "variable": [
    {
      "key": "baseUrl",
      "value": "http://localhost:5002",
      "type": "string"
    }
  ],
  "item": [
    {
      "name": "Get All Orders",
      "request": {
        "method": "GET",
        "header": [],
        "url": {
          "raw": "{{baseUrl}}/api/orders",
          "host": ["{{baseUrl}}"],
          "path": ["api", "orders"]
        }
      }
    },
    {
      "name": "Create Order",
      "request": {
        "method": "POST",
        "header": [
          {
            "key": "Content-Type",
            "value": "application/json"
          }
        ],
        "body": {
          "mode": "raw",
          "raw": "{\n  \"userId\": \"user123\",\n  \"userName\": \"John Doe\",\n  \"items\": [\n    {\n      \"productId\": \"prod001\",\n      \"productName\": \"iPhone 15 Pro\",\n      \"quantity\": 1,\n      \"unitPrice\": 999.99\n    }\n  ],\n  \"shippingAddress\": {\n    \"street\": \"123 Main St\",\n    \"city\": \"New York\",\n    \"state\": \"NY\",\n    \"country\": \"USA\",\n    \"zipCode\": \"10001\",\n    \"phoneNumber\": \"+1234567890\"\n  },\n  \"paymentInfo\": {\n    \"paymentMethod\": \"CreditCard\",\n    \"cardName\": \"John Doe\",\n    \"cardNumber\": \"4532123456789012\",\n    \"expiration\": \"12/25\",\n    \"cvv\": \"123\"\n  }\n}"
        },
        "url": {
          "raw": "{{baseUrl}}/api/orders",
          "host": ["{{baseUrl}}"],
          "path": ["api", "orders"]
        }
      }
    }
  ]
}
```

**Importer dans Postman:**
1. Ouvrir Postman
2. Cliquer sur **Import**
3. Sélectionner `Order-API-Docker.postman_collection.json`
4. Cliquer sur **Import**

---

## 🧪 Tests des endpoints

### Base URL Docker

**Important:** Avec Docker, l'URL de base reste la même car le port est mappé:
```
http://localhost:5002
```

Docker fait le mapping: `localhost:5002` → `container:5002`

---

### 1️⃣ GET - Tous les ordres

**URL Postman:**
```
GET http://localhost:5002/api/orders
```

**Headers:**
```
Content-Type: application/json
```

**Réponse attendue (200 OK):**
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

**Test Scripts Postman (onglet Tests):**
```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Response is an array", function () {
    pm.expect(pm.response.json()).to.be.an('array');
});
```

---

### 2️⃣ POST - Créer une commande

**URL Postman:**
```
POST http://localhost:5002/api/orders
```

**Headers:**
```
Content-Type: application/json
```

**Body (raw JSON):**
```json
{
  "userId": "docker_user_001",
  "userName": "Docker Test User",
  "items": [
    {
      "productId": "prod001",
      "productName": "MacBook Pro 16",
      "quantity": 1,
      "unitPrice": 2499.99
    },
    {
      "productId": "prod002",
      "productName": "Magic Mouse",
      "quantity": 1,
      "unitPrice": 79.99
    }
  ],
  "shippingAddress": {
    "street": "456 Docker Lane",
    "city": "Container City",
    "state": "DC",
    "country": "Dockerland",
    "zipCode": "90210",
    "phoneNumber": "+1555123456"
  },
  "paymentInfo": {
    "paymentMethod": "CreditCard",
    "cardName": "Docker Test",
    "cardNumber": "4532123456789012",
    "expiration": "12/26",
    "cvv": "456"
  }
}
```

**Réponse attendue (201 Created):**
```json
{
  "id": "675f8a3b1c9d440000abcdef",
  "orderNumber": "ORD-20260216-XYZ123",
  "userId": "docker_user_001",
  "userName": "Docker Test User",
  "totalAmount": 2579.98,
  "status": "Pending",
  "orderItems": [
    {
      "productId": "prod001",
      "productName": "MacBook Pro 16",
      "quantity": 1,
      "unitPrice": 2499.99,
      "totalPrice": 2499.99
    },
    {
      "productId": "prod002",
      "productName": "Magic Mouse",
      "quantity": 1,
      "unitPrice": 79.99,
      "totalPrice": 79.99
    }
  ],
  "shippingAddress": {...},
  "paymentInfo": {
    "paymentMethod": "CreditCard",
    "cardName": "Docker Test",
    "cardNumber": "****9012",
    "expiration": "12/26",
    "cvv": "***"
  },
  "createdAt": "2026-02-16T14:30:00Z",
  "updatedAt": "2026-02-16T14:30:00Z"
}
```

**Test Scripts Postman:**
```javascript
pm.test("Status code is 201", function () {
    pm.response.to.have.status(201);
});

pm.test("Order created with ID", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.id).to.exist;
    pm.environment.set("orderId", jsonData.id);
});

pm.test("Total amount is correct", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.totalAmount).to.eql(2579.98);
});

pm.test("Card number is masked", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.paymentInfo.cardNumber).to.include("****");
});
```

---

### 3️⃣ GET - Commande par ID

**URL Postman:**
```
GET http://localhost:5002/api/orders/{{orderId}}
```

_(Utilisez l'ID retourné lors de la création)_

**Réponse attendue (200 OK):**
```json
{
  "id": "675f8a3b1c9d440000abcdef",
  "orderNumber": "ORD-20260216-XYZ123",
  "userId": "docker_user_001",
  "userName": "Docker Test User",
  "totalAmount": 2579.98,
  "status": "Pending",
  ...
}
```

**Test Scripts:**
```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Order has correct ID", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.id).to.eql(pm.environment.get("orderId"));
});
```

---

### 4️⃣ PUT - Mettre à jour le statut

**URL Postman:**
```
PUT http://localhost:5002/api/orders/{{orderId}}/status
```

**Body (raw JSON):**
```json
{
  "status": "Confirmed"
}
```

**Réponse attendue (200 OK):**
```json
{
  "message": "Order status updated successfully"
}
```

**Statuts disponibles:**
- `Pending`
- `Confirmed`
- `Processing`
- `Shipped`
- `Delivered`
- `Cancelled`
- `Refunded`

**Test Scripts:**
```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Success message received", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.message).to.include("updated successfully");
});
```

---

### 5️⃣ GET - Tracking de commande

**URL Postman:**
```
GET http://localhost:5002/api/orders/{{orderId}}/tracking
```

**Réponse attendue (200 OK):**
```json
{
  "orderId": "675f8a3b1c9d440000abcdef",
  "orderNumber": "ORD-20260216-XYZ123",
  "status": "Confirmed",
  "createdAt": "2026-02-16T14:30:00Z",
  "updatedAt": "2026-02-16T14:35:00Z",
  "estimatedDelivery": "2026-02-22T14:30:00Z"
}
```

**Test Scripts:**
```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Has estimated delivery", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.estimatedDelivery).to.exist;
});
```

---

### 6️⃣ GET - Commandes par utilisateur

**URL Postman:**
```
GET http://localhost:5002/api/orders/user/docker_user_001
```

**Réponse attendue (200 OK):**
```json
[
  {
    "id": "675f8a3b1c9d440000abcdef",
    "orderNumber": "ORD-20260216-XYZ123",
    "userId": "docker_user_001",
    ...
  }
]
```

---

### 7️⃣ DELETE - Annuler une commande

**URL Postman:**
```
DELETE http://localhost:5002/api/orders/{{orderId}}
```

**Réponse attendue (200 OK):**
```json
{
  "message": "Order cancelled successfully"
}
```

**⚠️ Note:** Seulement possible si statut = `Pending`, `Confirmed` ou `Processing`

---

## 🎯 Scénarios de test complets

### Scénario 1: Workflow complet avec Docker

**Objectif:** Créer, confirmer, expédier et livrer une commande

1. **Vérifier que Docker est actif**
   ```powershell
   docker-compose ps
   ```

2. **Créer une commande**
   ```
   POST http://localhost:5002/api/orders
   ```
   ✅ Copier l'`id` retourné → `{{orderId}}`

3. **Vérifier la commande**
   ```
   GET http://localhost:5002/api/orders/{{orderId}}
   ```
   ✅ Status = "Pending"

4. **Confirmer**
   ```
   PUT http://localhost:5002/api/orders/{{orderId}}/status
   Body: { "status": "Confirmed" }
   ```

5. **Traiter**
   ```
   PUT http://localhost:5002/api/orders/{{orderId}}/status
   Body: { "status": "Processing" }
   ```

6. **Expédier**
   ```
   PUT http://localhost:5002/api/orders/{{orderId}}/status
   Body: { "status": "Shipped" }
   ```

7. **Vérifier le tracking**
   ```
   GET http://localhost:5002/api/orders/{{orderId}}/tracking
   ```
   ✅ `estimatedDelivery` = +3 jours

8. **Livrer**
   ```
   PUT http://localhost:5002/api/orders/{{orderId}}/status
   Body: { "status": "Delivered" }
   ```

9. **Vérifier dans MongoDB**
   ```powershell
   docker exec marketplace_mongodb mongosh -u root -p password --eval "use orders_db; db.orders.find({_id: ObjectId('{{orderId}}')}).pretty()"
   ```

---

### Scénario 2: Test de charge avec Docker

**Créer 10 commandes rapidement:**

Dans Postman:
1. Créer une requête POST `/api/orders`
2. Onglet **Tests**, ajouter:
   ```javascript
   pm.test("Order created", function () {
       pm.response.to.have.status(201);
   });
   ```
3. Clic droit sur la requête → **Run** → **Iterations: 10**

**Vérifier dans MongoDB:**
```powershell
docker exec marketplace_mongodb mongosh -u root -p password --eval "use orders_db; db.orders.countDocuments()"
```

---

### Scénario 3: Vérification des événements RabbitMQ

1. **Créer une commande**
   ```
   POST http://localhost:5002/api/orders
   ```

2. **Ouvrir RabbitMQ Management**
   ```
   http://localhost:15672
   Username: guest
   Password: guest
   ```

3. **Vérifier les exchanges**
   - Aller à **Exchanges**
   - Chercher `orders.exchange`
   - Voir les messages publiés

4. **Vérifier les queues**
   - Aller à **Queues**
   - Voir les messages dans les queues liées

---

## 🐛 Troubleshooting

### ❌ Problème: "Connection refused" sur localhost:5002

**Solution:**

```powershell
# Vérifier que le conteneur est actif
docker-compose ps order-api

# Si le conteneur n'est pas UP
docker-compose up -d order-api

# Voir les logs
docker-compose logs order-api
```

---

### ❌ Problème: 500 Internal Server Error

**Causes possibles:**
1. MongoDB n'est pas accessible
2. RabbitMQ n'est pas accessible
3. Erreur dans le code

**Solutions:**

```powershell
# 1. Vérifier MongoDB
docker exec marketplace_mongodb mongosh -u root -p password --eval "db.adminCommand('ping')"

# 2. Vérifier RabbitMQ
docker exec marketplace_rabbitmq rabbitmq-diagnostics ping

# 3. Voir les logs détaillés
docker-compose logs -f order-api

# 4. Redémarrer tous les services
docker-compose restart
```

---

### ❌ Problème: "Order must contain at least one item"

**Cause:** Le body de la requête est vide ou mal formaté

**Solution:**

1. Vérifier que le `Content-Type` est `application/json`
2. Vérifier que le JSON est valide (utilisez un validateur JSON)
3. Vérifier que `items` est un tableau non vide

**Body minimal valide:**
```json
{
  "userId": "test123",
  "userName": "Test User",
  "items": [
    {
      "productId": "p1",
      "productName": "Product 1",
      "quantity": 1,
      "unitPrice": 10.00
    }
  ],
  "shippingAddress": {
    "street": "123 St",
    "city": "City",
    "state": "ST",
    "country": "Country",
    "zipCode": "12345",
    "phoneNumber": "+123456789"
  },
  "paymentInfo": {
    "paymentMethod": "CreditCard",
    "cardName": "Test",
    "cardNumber": "4532123456789012",
    "expiration": "12/25",
    "cvv": "123"
  }
}
```

---

### ❌ Problème: Données MongoDB perdues après redémarrage

**Cause:** Le volume Docker a été supprimé

**Vérifier:**
```powershell
# Voir les volumes
docker volume ls | findstr mongodb

# Restaurer depuis un backup
docker exec marketplace_mongodb mongorestore -u root -p password --db orders_db /tmp/backup/orders_db
```

**Prévention:**
- Ne jamais utiliser `docker-compose down -v` en production
- Utiliser `docker-compose down` (sans -v) pour préserver les données

---

### ❌ Problème: PaymentMethod enum error

**Erreur:**
```json
{
  "message": "The JSON value could not be converted to PaymentMethod"
}
```

**Solution:** Utiliser les valeurs exactes:
- `CreditCard` (pas `creditcard` ou `credit_card`)
- `DebitCard`
- `PayPal`
- `BankTransfer`

---

## 📊 Monitoring avec Docker

### Voir les logs en direct

```powershell
# Order.API seulement
docker-compose logs -f order-api

# Tous les services
docker-compose logs -f

# MongoDB seulement
docker-compose logs -f mongodb

# RabbitMQ seulement
docker-compose logs -f rabbitmq
```

### Statistiques de ressources

```powershell
# CPU et RAM en temps réel
docker stats order_api

# Tous les conteneurs
docker stats
```

### Inspecter MongoDB

```powershell
# Compter les commandes
docker exec marketplace_mongodb mongosh -u root -p password --eval "use orders_db; db.orders.countDocuments()"

# Voir toutes les commandes
docker exec marketplace_mongodb mongosh -u root -p password --eval "use orders_db; db.orders.find().pretty()"

# Voir les commandes par statut
docker exec marketplace_mongodb mongosh -u root -p password --eval "use orders_db; db.orders.find({Status: 'Pending'}).pretty()"
```

---

## ✅ Checklist avant de commencer les tests

- [ ] Docker Desktop est démarré
- [ ] `docker-compose up -d` exécuté avec succès
- [ ] `docker-compose ps` montre tous les services UP et (healthy)
- [ ] `http://localhost:5002/api/orders` répond (même si `[]`)
- [ ] Postman est ouvert avec la collection importée
- [ ] L'environnement "Docker Local" est sélectionné dans Postman
- [ ] RabbitMQ Management accessible sur http://localhost:15672

---

## 🎓 Bonnes pratiques

### 1. Toujours vérifier les logs

Avant chaque session de tests:
```powershell
docker-compose logs -f order-api
```

### 2. Utiliser les variables Postman

Au lieu de:
```
GET http://localhost:5002/api/orders/675f8a3b1c9d440000abcdef
```

Utiliser:
```
GET {{baseUrl}}/api/orders/{{orderId}}
```

### 3. Créer des tests automatiques

Dans Postman, onglet **Tests**, toujours ajouter:
```javascript
pm.test("Status code is valid", function () {
    pm.expect(pm.response.code).to.be.oneOf([200, 201]);
});
```

### 4. Sauvegarder les résultats

Utiliser **Postman Collections Runner** pour:
- Exécuter tous les tests en séquence
- Exporter les résultats en JSON
- Partager les résultats avec l'équipe

---

## 📚 Ressources complémentaires

- **Guide Docker complet**: Voir `DOCKER_COMMANDS.md`
- **Tests Postman détaillés**: Voir `POSTMAN_TESTS.md`
- **Architecture du projet**: Voir `../ARCHITECTURE.md`

---

## 🔄 Commandes Docker essentielles

```powershell
# Démarrer
docker-compose up -d

# Voir les logs
docker-compose logs -f order-api

# Redémarrer après modification du code
docker-compose up -d --build order-api

# Arrêter
docker-compose down

# Nettoyer (⚠️ supprime les données)
docker-compose down -v

# Vérifier le statut
docker-compose ps
```

---

**✨ Vous êtes prêt à tester Order.API avec Docker et Postman !**

Pour toute question, consultez les logs avec `docker-compose logs -f order-api`
