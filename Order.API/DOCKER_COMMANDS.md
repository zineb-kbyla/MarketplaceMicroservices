# 🐳 Docker Commands - Order.API

Guide complet des commandes Docker pour déployer et gérer le microservice Order.API.

---

## 📋 Prérequis

- **Docker Desktop** installé et en cours d'exécution
- **Docker Compose** v2.0+ (inclus avec Docker Desktop)
- Ports disponibles: `5002`, `27017`, `5672`, `15672`

---

## 🚀 Démarrage rapide

### Option 1: Démarrer tous les services (Recommandé)

Depuis la racine du projet `ProjetMarktplace_Net`:

```bash
# Démarrer tous les services (MongoDB, RabbitMQ, Product.API, Order.API)
docker-compose up -d

# Voir les logs en temps réel
docker-compose logs -f order-api
```

### Option 2: Démarrer seulement Order.API avec ses dépendances

```bash
# Démarrer MongoDB, RabbitMQ et Order.API
docker-compose up -d mongodb rabbitmq order-api
```

### Option 3: Démarrer Order.API seul (MongoDB et RabbitMQ doivent déjà être actifs)

```bash
docker-compose up -d order-api
```

---

## 🛠️ Commandes de base

### Build et démarrage

```bash
# Build l'image Order.API sans cache (pour forcer une reconstruction complète)
docker-compose build --no-cache order-api

# Build et démarrer Order.API
docker-compose up -d --build order-api

# Démarrer Order.API (utilise l'image existante)
docker-compose up -d order-api
```

### Arrêt et nettoyage

```bash
# Arrêter Order.API
docker-compose stop order-api

# Arrêter et supprimer le conteneur Order.API
docker-compose down order-api

# Arrêter tous les services
docker-compose down

# Arrêter tous les services ET supprimer les volumes (⚠️ SUPPRIME LES DONNÉES MongoDB)
docker-compose down -v

# Supprimer l'image Order.API
docker rmi projetmarktplace_net-order-api
```

### Redémarrage

```bash
# Redémarrer Order.API
docker-compose restart order-api

# Redémarrer tous les services
docker-compose restart
```

---

## 📊 Monitoring et logs

### Voir les logs

```bash
# Logs en temps réel de Order.API
docker-compose logs -f order-api

# Logs des 100 dernières lignes
docker-compose logs --tail=100 order-api

# Logs de tous les services
docker-compose logs -f

# Logs de Order.API, MongoDB et RabbitMQ
docker-compose logs -f order-api mongodb rabbitmq
```

### Vérifier le statut des services

```bash
# Voir tous les conteneurs actifs
docker-compose ps

# Voir les processus dans le conteneur Order.API
docker-compose top order-api

# Vérifier l'état de santé de MongoDB
docker exec marketplace_mongodb mongosh --eval "db.adminCommand('ping')"

# Vérifier l'état de santé de RabbitMQ
docker exec marketplace_rabbitmq rabbitmq-diagnostics ping
```

### Statistiques de ressources

```bash
# Voir l'utilisation CPU/RAM en temps réel
docker stats order_api

# Voir l'utilisation de tous les conteneurs
docker stats
```

---

## 🔍 Debugging et inspection

### Accéder au conteneur

```bash
# Ouvrir un shell bash dans le conteneur Order.API
docker exec -it order_api bash

# Exécuter une commande dans le conteneur
docker exec order_api ls -la /app
```

### Inspecter la configuration

```bash
# Inspecter le conteneur Order.API
docker inspect order_api

# Voir les variables d'environnement
docker exec order_api env | grep -E "ASPNETCORE|MongoDb|RabbitMq"

# Voir les fichiers de configuration
docker exec order_api cat /app/appsettings.json
```

### Vérifier la connectivité réseau

```bash
# Voir les réseaux Docker
docker network ls

# Inspecter le réseau marketplace
docker network inspect projetmarktplace_net_marketplace_network

# Tester la connexion depuis Order.API vers MongoDB
docker exec order_api ping mongodb

# Tester la connexion depuis Order.API vers RabbitMQ
docker exec order_api ping rabbitmq
```

---

## 🧪 Tests de l'API

### Vérifier que l'API est accessible

```bash
# Health check (depuis l'hôte Windows)
curl http://localhost:5002/api/orders

# Depuis PowerShell
Invoke-WebRequest -Uri http://localhost:5002/api/orders -Method GET
```

### Créer une commande de test

```bash
# PowerShell
$body = @{
  userId = "user123"
  userName = "Test User"
  items = @(
    @{
      productId = "prod001"
      productName = "Test Product"
      quantity = 1
      unitPrice = 99.99
    }
  )
  shippingAddress = @{
    street = "123 Test St"
    city = "Test City"
    state = "TC"
    country = "Test Country"
    zipCode = "12345"
    phoneNumber = "+1234567890"
  }
  paymentInfo = @{
    paymentMethod = "CreditCard"
    cardName = "Test User"
    cardNumber = "4532123456789012"
    expiration = "12/25"
    cvv = "123"
  }
} | ConvertTo-Json -Depth 5

Invoke-WebRequest -Uri http://localhost:5002/api/orders -Method POST -Body $body -ContentType "application/json"
```

### Accéder à Swagger/Scalar

Ouvrir dans le navigateur:
```
http://localhost:5002/scalar/v1
```

---

## 🗄️ Gestion MongoDB

### Accéder à MongoDB

```bash
# Se connecter à MongoDB via mongosh
docker exec -it marketplace_mongodb mongosh -u root -p password

# Lister les bases de données
docker exec marketplace_mongodb mongosh -u root -p password --eval "show dbs"

# Voir les commandes dans la base orders_db
docker exec marketplace_mongodb mongosh -u root -p password --eval "use orders_db; db.orders.find().pretty()"

# Compter le nombre de commandes
docker exec marketplace_mongodb mongosh -u root -p password --eval "use orders_db; db.orders.countDocuments()"
```

### Backup et restore

```bash
# Backup de la base de données orders_db
docker exec marketplace_mongodb mongodump -u root -p password --db orders_db --out /tmp/backup

# Copier le backup vers l'hôte
docker cp marketplace_mongodb:/tmp/backup ./mongodb_backup

# Restore depuis un backup
docker cp ./mongodb_backup marketplace_mongodb:/tmp/restore
docker exec marketplace_mongodb mongorestore -u root -p password --db orders_db /tmp/restore/orders_db
```

---

## 🐰 Gestion RabbitMQ

### Accéder à l'interface web RabbitMQ

Ouvrir dans le navigateur:
```
http://localhost:15672
Username: guest
Password: guest
```

### Commandes RabbitMQ

```bash
# Voir les queues
docker exec marketplace_rabbitmq rabbitmqctl list_queues

# Voir les exchanges
docker exec marketplace_rabbitmq rabbitmqctl list_exchanges

# Voir les connexions actives
docker exec marketplace_rabbitmq rabbitmqctl list_connections

# Purger une queue
docker exec marketplace_rabbitmq rabbitmqctl purge_queue <queue_name>
```

---

## 🔧 Rebuild après modification du code

```bash
# 1. Arrêter le conteneur
docker-compose stop order-api

# 2. Rebuild l'image
docker-compose build --no-cache order-api

# 3. Redémarrer le conteneur
docker-compose up -d order-api

# 4. Voir les logs pour vérifier
docker-compose logs -f order-api
```

**OU en une seule commande:**

```bash
docker-compose up -d --build --force-recreate order-api
```

---

## 🌐 URLs et ports

| Service | Port | URL |
|---------|------|-----|
| **Order.API** | 5002 | http://localhost:5002 |
| **Order.API Swagger** | 5002 | http://localhost:5002/scalar/v1 |
| **Product.API** | 5001 | http://localhost:5001 |
| **MongoDB** | 27017 | mongodb://root:password@localhost:27017 |
| **RabbitMQ AMQP** | 5672 | amqp://guest:guest@localhost:5672 |
| **RabbitMQ Management** | 15672 | http://localhost:15672 |

---

## 🔐 Variables d'environnement (configurées dans docker-compose.yml)

```yaml
ASPNETCORE_ENVIRONMENT: Docker
ASPNETCORE_URLS: http://+:5002
MongoDb__ConnectionString: mongodb://root:password@mongodb:27017
MongoDb__DatabaseName: marketplace_order
RabbitMq__HostName: rabbitmq
RabbitMq__Port: 5672
RabbitMq__UserName: guest
RabbitMq__Password: guest
Services__ProductService__Url: http://product-api:5001
```

---

## 🐛 Troubleshooting

### Problème: Le conteneur Order.API ne démarre pas

```bash
# Vérifier les logs
docker-compose logs order-api

# Vérifier que MongoDB et RabbitMQ sont en santé
docker-compose ps
```

### Problème: "Connection refused" vers MongoDB

```bash
# Vérifier que MongoDB est accessible
docker exec order_api ping mongodb

# Vérifier les logs MongoDB
docker-compose logs mongodb

# Redémarrer MongoDB
docker-compose restart mongodb
```

### Problème: "Connection refused" vers RabbitMQ

```bash
# Vérifier que RabbitMQ est accessible
docker exec order_api ping rabbitmq

# Vérifier les logs RabbitMQ
docker-compose logs rabbitmq

# Redémarrer RabbitMQ
docker-compose restart rabbitmq
```

### Problème: Port 5002 déjà utilisé

```bash
# Trouver le processus qui utilise le port 5002
netstat -ano | findstr :5002

# Tuer le processus (remplacer <PID> par le numéro trouvé)
taskkill /PID <PID> /F

# OU modifier le port dans docker-compose.yml
# Changer "5002:5002" en "5003:5002" par exemple
```

### Problème: L'image est obsolète après modification du code

```bash
# Forcer une reconstruction complète
docker-compose down order-api
docker rmi projetmarktplace_net-order-api
docker-compose up -d --build order-api
```

### Problème: Volumes MongoDB corrompus

```bash
# ⚠️ ATTENTION: Ceci supprime TOUTES les données MongoDB
docker-compose down -v
docker volume rm projetmarktplace_net_mongodb_data
docker-compose up -d
```

---

## 📚 Workflow complet de développement

### 1. Premier démarrage

```bash
# Démarrer tous les services
cd "d:\Cours Jobintech\ProjetMarktplace_Net"
docker-compose up -d

# Vérifier que tout fonctionne
docker-compose ps
docker-compose logs -f order-api
```

### 2. Développement et tests

```bash
# Modifier le code dans Order.API/...

# Rebuild et redémarrer
docker-compose up -d --build order-api

# Voir les logs
docker-compose logs -f order-api

# Tester avec Postman
# Importer POSTMAN_TESTS.md et tester les endpoints
```

### 3. Debugging

```bash
# Voir les logs détaillés
docker-compose logs -f order-api

# Accéder au conteneur
docker exec -it order_api bash

# Vérifier la configuration
docker exec order_api cat /app/appsettings.json
```

### 4. Arrêt en fin de journée

```bash
# Arrêter tous les services
docker-compose stop

# OU arrêter et supprimer les conteneurs (les volumes sont conservés)
docker-compose down
```

### 5. Redémarrage le lendemain

```bash
# Redémarrer tous les services (les données sont conservées)
docker-compose up -d
```

---

## ✅ Checklist de déploiement

- [ ] Docker Desktop est démarré
- [ ] Les ports 5002, 27017, 5672, 15672 sont disponibles
- [ ] `docker-compose up -d` exécuté avec succès
- [ ] `docker-compose ps` montre tous les services "Up (healthy)"
- [ ] http://localhost:5002/api/orders retourne une réponse
- [ ] http://localhost:5002/scalar/v1 accessible
- [ ] http://localhost:15672 accessible (RabbitMQ Management)
- [ ] MongoDB accessible: `docker exec marketplace_mongodb mongosh -u root -p password --eval "db.adminCommand('ping')"`

---

## 🎯 Commandes les plus utilisées

```bash
# Démarrer tout
docker-compose up -d

# Voir les logs Order.API
docker-compose logs -f order-api

# Rebuild après modification
docker-compose up -d --build order-api

# Arrêter tout
docker-compose down

# Nettoyer tout (⚠️ supprime les données)
docker-compose down -v

# Voir le statut
docker-compose ps

# Redémarrer Order.API
docker-compose restart order-api
```

---

## 📞 Support

Pour plus d'informations:
- **Postman Tests**: Voir `POSTMAN_TESTS.md`
- **Architecture**: Voir `ARCHITECTURE.md` (racine du projet)
- **Commandes utiles**: Voir `USEFUL_COMMANDS.md` (racine du projet)
