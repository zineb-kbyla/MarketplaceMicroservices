# 🏪 Marketplace Microservices - .NET 10.0

Architecture complète de **microservices avec API Gateway centralisé** pour une plateforme marketplace moderne utilisant **.NET 10.0**, **MongoDB**, **RabbitMQ**, et **Neo4j**.

## 📋 Vue d'Ensemble du Projet

Ce projet implémente une plateforme e-commerce réelle avec une architecture microservices où:
- ✅ Un **API Gateway unique** (YARP) gère toutes les requêtes externes
- ✅ Trois **services métier indépendants** (Product, Order, Recommendation)
- ✅ **Communication asynchrone** via message bus RabbitMQ
- ✅ **Isolation des données** (MongoDB pour Product/Order, Neo4j pour Recommendation)
- ✅ **Scalabilité** - Chaque service peut être déployé indépendamment

---

## 🏗️ Architecture Globale

```
┌───────────────────────────────────────────────────────────────────────────┐
│                                                                            │
│                     🚪 API GATEWAY (YARP)                               │
│                  🔗 Point d'Entrée Unique (Port 5000)                    │
│              Routage centralisé • Logging • Health Checks                │
│                                                                            │
└──────────┬─────────────────────────┬──────────────────┬───────────────────┘
           │                         │                  │
           ▼                         ▼                  ▼
    ┌─────────────────┐    ┌─────────────────┐   ┌──────────────────┐
    │ Product Service │    │  Order Service  │   │ Recommendation   │
    │  (Port 5001)    │    │  (Port 5002)    │   │ (Port 8004)      │
    │  • Catalogue    │    │  • Commandes    │   │ • Algorithmes ML │
    │  • Stock        │    │  • Paiements    │   │ • User Profiles  │
    │  • Pricing      │    │  • Suivi        │   │ • Suggestions    │
    └────────┬────────┘    └────────┬────────┘   └────────┬─────────┘
             │                      │                     │
             └──────────┬───────────┴─────────┬───────────┘
                        │                     │
         ┌──────────────┴────────────┐    ┌───┴──────────────┐
         │                           │    │                  │
         ▼                           ▼    ▼                  ▼
    ┌──────────────┐          ┌──────────────────┐    ┌──────────────┐
    │   MongoDB    │          │   RabbitMQ       │    │   Neo4j      │
    │  (27017)     │          │ (5672, 15672)    │    │ (7687,7474)  │
    │ • Products   │          │  Message Bus     │    │ • User Graph │
    │ • Orders     │          │  • Events        │    │ • Relations  │
    └──────────────┘          └──────────────────┘    └──────────────┘
```

### 🔷 Topologie Détaillée:
| Composant | Port | Rôle | Accès |
|-----------|------|------|-------|
| **API Gateway** | 5000 | Point d'entrée unique pour les clients externes | Public |
| **Product Service** | 5001 | Service produits & catalogue | Interne (via Gateway) |
| **Order Service** | 5002 | Service commandes & paiements | Interne (via Gateway) |
| **Recommendation Service** | 8004 | Système de recommandations IA | Interne (via Gateway) |
| **MongoDB** | 27017 | Base de données MongoDB | Interne (Services) |
| **RabbitMQ** | 5672 | Bus de messages | Interne (Services) |
| **RabbitMQ UI** | 15672 | Interface de gestion RabbitMQ | Interne |
| **Neo4j** | 7687 | Base de graphes Neo4j | Interne (Services) |
| **Neo4j Browser** | 7474 | Interface Neo4j | Interne |

## Services

### 1. Product Service (5001)
Service de gestion des produits et du catalogue.

**Responsabilités:**
- Gestion du catalogue de produits
- Gestion des catégories
- Gestion du stock
- Pricing

**Technologies:**
- .NET 10.0 (ASP.NET Core)
- MongoDB
- RabbitMQ

**API Endpoints:**
- `GET /api/products` - Lister les produits
- `GET /api/products/{id}` - Détails d'un produit
- `POST /api/products` - Créer un produit
- `PUT /api/products/{id}` - Modifier un produit
- `DELETE /api/products/{id}` - Supprimer un produit
- `GET /api/categories` - Lister les catégories
- `GET /api/products/{id}/stock` - Vérifier le stock

### 2. Order Service (5002) - NOUVEAU
Service de gestion des commandes.

**Responsabilités:**
- Création de commandes
- Gestion du statut des commandes
- Traitement des paiements
- Annulation de commandes
- Suivi des commandes

**Technologies:**
- .NET 10.0 (ASP.NET Core)
- MongoDB
- RabbitMQ
- MediatR (CQRS)

**API Endpoints:**
- `GET /api/orders?userId={userId}` - Commandes d'un utilisateur
- `GET /api/orders/{id}` - Détails d'une commande
- `POST /api/orders` - Créer une commande
- `PUT /api/orders/{id}/status` - Modifier le statut
- `DELETE /api/orders/{id}` - Annuler une commande
- `GET /api/orders/{id}/tracking` - Suivi de commande

## Architecture Technique

### Clean Architecture
```
Order.API/
├── Domain/              # Entités et règles métier
│   ├── Entities/
│   ├── Enums/
│   └── Events/
├── Application/         # Use cases et logique applicative
│   ├── Commands/        # CQRS Commands
│   ├── Queries/         # CQRS Queries
│   ├── Interfaces/
│   └── Services/
├── Infrastructure/      # Implémentations techniques
│   ├── Data/            # MongoDB
│   ├── Repositories/
│   ├── Messaging/       # RabbitMQ
│   └── ExternalServices/
└── API/                 # Contrôleurs et middleware
```

### Patterns Utilisés
- **CQRS** (Command Query Responsibility Segregation)
- **MediatR** pour la coordination des commandes/requêtes
- **Repository Pattern** pour l'accès aux données
- **Dependency Injection** pour la gestion des dépendances
- **AutoMapper** pour le mapping DTO
- **Event-Driven Architecture** avec RabbitMQ

## Installation et Configuration

### Prérequis
- .NET 10.0 SDK
- Docker & Docker Compose
- MongoDB 5.0+
- RabbitMQ 3.10+

### Démarrage Rapide avec Docker Compose

```bash
# Cloner le repository
git clone <repo-url>
cd ProjetMarktplace_Net

# Lancer tous les services
docker-compose up -d

# Vérifier les services
docker-compose ps
```

### Configuration des Variables d'Environnement

Voir les fichiers `appsettings.Development.json` dans chaque service pour la configuration locale.

### Ports

| Service | Port | Description |
|---------|------|-------------|
| Product API | 5001 | Service de produits |
| Order API | 5002 | Service de commandes |
| MongoDB | 27017 | Base de données |
| RabbitMQ AMQP | 5672 | Bus de messages |
| RabbitMQ UI | 15672 | Interface d'administration |

### Bases de Données MongoDB

- **marketplace_product** - Données des produits
- **marketplace_order** - Données des commandes

Collections:
- `products` - Produits
- `categories` - Catégories
- `orders` - Commandes
- `order_items` - Articles de commande

## Communication Entre Services

### Appels HTTP Synchrones
- Order Service → Product Service (vérification du stock)

### Événements Asynchrones (RabbitMQ)

**Événements Product Service:**
- `ProductCreatedEvent` - Produit créé
- `ProductUpdatedEvent` - Produit modifié
- `ProductDeletedEvent` - Produit supprimé
- `StockChangedEvent` - Stock modifié

**Événements Order Service:**
- `OrderCreatedEvent` - Commande créée
- `OrderStatusChangedEvent` - Statut modifié
- `OrderCancelledEvent` - Commande annulée

## Gestion des États (Order)

```
Pending → Confirmed → Processing → Shipped → Delivered → [Refunded]
   ↓         ↓            ↓           ↓
 [Cancelled]×────────────×─────────×
```

## Testing

### Tests Unitaires
```bash
# Product Service
cd Product.API/Tests
dotnet test

# Order Service
cd Order.API/Tests
dotnet test
```

### Tests d'Intégration
```bash
# Avec Docker Compose actif
docker-compose exec order-service dotnet test Tests/Integration
```

### Tests d'API avec Postman
- Importer `Order-Service.postman_collection.json`
- Importer `Product-Service.postman_collection.json`
- Exécuter les requêtes

## Déploiement

### Docker Compose (Développement)
```bash
docker-compose up -d
docker-compose logs -f
```

### Kubernetes (Production)
Voir [DEPLOYMENT_GUIDE.md](./Order.API/DEPLOYMENT_GUIDE.md) pour les configurations Kubernetes.

### Azure Container Registry
```bash
az acr build --registry <registry-name> --image order-service:latest ./Order.API
```

## Monitoring et Logging

- **Logs**: Stockés dans les conteneurs et fichiers (à configurer)
- **Métriques**: Via Application Insights (optionnel)
- **Health Checks**: Endpoints `/health` disponibles
- **API Documentation**: Swagger UI à `/swagger/index.html`

## Documentation

- [Order Service Architecture](./Order.API/ARCHITECTURE.md)
- [Order Service Deployment](./Order.API/DEPLOYMENT_GUIDE.md)
- [Product Service README](./Product.API/README.md)
- [Order Service README](./Order.API/README.md)

## Dépendances NuGet Principales

**Chaque service:**
- `MongoDB.Driver` - Driver MongoDB
- `RabbitMQ.Client` - Client RabbitMQ
- `MediatR` - CQRS pattern
- `AutoMapper` - DTO mapping
- `Swashbuckle.AspNetCore` - Swagger/OpenAPI
- `Microsoft.AspNetCore.*` - Framework ASP.NET Core

## Bonnes Pratiques

1. **Sécurité**
   - Valider toutes les entrées
   - Utiliser HTTPS en production
   - Gérer les secrets avec Azure Key Vault

2. **Performance**
   - Implémenter le caching (Redis)
   - Optimiser les requêtes MongoDB
   - Monitorer les performances RabbitMQ

3. **Scalabilité**
   - Services stateless
   - Database sharding ready
   - Load balancing capable

4. **Reliability**
   - Retry patterns
   - Circuit breakers
   - Health checks

## Troubleshooting

### Connection MongoDB
```bash
# Vérifier MongoDB
docker logs marketplace_mongodb

# Connecter avec mongosh
docker exec -it marketplace_mongodb mongosh -u root -p password
```

### RabbitMQ
```bash
# Accéder à RabbitMQ Management
Browser: http://localhost:15672
User: guest / Password: guest

# Vérifier les logs
docker logs marketplace_rabbitmq
```

### Services
```bash
# Vérifier les status
docker-compose ps

# Logs d'un service
docker-compose logs order-api
docker-compose logs product-api

# Redémarrer
docker-compose restart order-api
```

## Contribution

1. Fork le repository
2. Créer une branche feature (`git checkout -b feature/AmazingFeature`)
3. Commit vos changements (`git commit -m 'Add some AmazingFeature'`)
4. Push la branche (`git push origin feature/AmazingFeature`)
5. Ouvrir une Pull Request

## Licence

MIT License - Voir LICENSE file pour détails

## Support

Pour les problèmes ou questions:
1. Vérifier la documentation
2. Consulter les logs (docker logs)
3. Ouvrir une issue sur GitHub

## Feuille de Route

- [ ] Service d'Authentification
- [ ] Service de Paiement
- [ ] Service d'Inventaire avancé
- [ ] Notifications (Email, SMS)
- [ ] Analytics Service
- [ ] Système de Recommandations

---

**Auteur**: Jobintech Development Team
**Dernière mise à jour**: 2024-02-16
**Version**: 1.0.0
