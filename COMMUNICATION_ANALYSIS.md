# 🔄 Analyse de la Communication entre Microservices

## ✅ Communication EXISTANTE

### 1. **Product.API → RabbitMQ** ✅ Existante
```
Événements publiés:
├─ ProductCreatedEvent
├─ ProductUpdatedEvent
└─ StockUpdatedEvent

Configuration:
├─ Exchange: products.exchange (Type: Topic)
├─ Client: RabbitMQ.Client (natif)
├─ Routing keys: product.productcreatedevent, product.*
└─ Durable: true
```

### 2. **Order.API → RabbitMQ** ✅ Existante
```
Événements publiés:
├─ OrderCreatedEvent
├─ OrderStatusChangedEvent
└─ OrderCancelledEvent

Configuration:
├─ Exchange: orders.exchange (Type: Topic)
├─ Client: RabbitMQ.Client (natif)
├─ Routing keys: order.ordercreatedevent, order.*
└─ Durable: true
```

### 3. **Recommendation.API ← RabbitMQ** ✅ Partiellement Existante
```
Événements consommés:
├─ OrderCreatedEventConsumer (MassTransit)
└─ ProductViewedEventConsumer (MassTransit)

Configuration:
├─ Client: MassTransit (abstraction)
├─ Endpoint: recommendation-service
└─ Transport: RabbitMQ
```

---

## ❌ Problèmes CRITIQUES

### PROBLÈME 1: Incompatibilité des Exchanges
```
❌ Product.API publie dans: products.exchange
❌ Order.API publie dans: orders.exchange
❌ Recommendation.API consomme via MassTransit

💥 RÉSULTAT: Les événements n'arrivent JAMAIS à Recommendation.API!
   - Product.API utilise RabbitMQ.Client natif → exchange "products.exchange"
   - MassTransit crée sa propre topologie (exchanges/queues différentes)
   - Les routing keys ne MATCHENT PAS
```

### PROBLÈME 2: ProductViewedEvent N'EST PAS PUBLIÉ
```
❌ Event défini: Product.API/Domain/Events/DomainEvents.cs
❌ Aucun endpoint dans ProductsController pour publier l'événement
❌ Code jamais exécuté

💥 RÉSULTAT: Recommendation.API attend ProductViewedEventConsumer 
   mais ne recevra JAMAIS cet événement!
```

### PROBLÈME 3: Order.API Ignore les Stocks du Product
```
❌ Order.API n'appelle PAS Product.API avant de créer une commande
❌ Aucune vérification: if (product.stock >= quantity)
❌ Aucun appel HTTP/gRPC à Product.API

💥 RÉSULTAT: Vente de produits OUT OF STOCK possible!
   - OrderService.CreateOrderAsync() ne vérifie PAS les stocks
   - DecrementStock() n'est appelé NULLE PART
```

### PROBLÈME 4: Order.API Ne Consomme Rien
```
❌ Order.API n'a PAS de Consumers
❌ Order.API n'écoute PAS ProductCreatedEvent
❌ Order.API n'écoute PAS ProductUpdatedEvent

💥 RÉSULTAT: Order.API ne peut pas mettre à jour sa cache
   de produits quand le stock change!
```

### PROBLÈME 5: Recommendation Ne Peut Pas Récupérer les Produits
```
❌ Aucun HttpClient dans Recommendation.API
❌ Aucune méthode pour GET /products/{id} depuis Product.API
❌ Recommendation ne peut pas enrichir les recommandations avec prix/détails

💥 RÉSULTAT: Recommandations incomplètes sans détails produits!
```

---

## 📊 Comparaison: Architecture Documentée vs Réalité

### ✅ DOCUMENTÉ
```
Product → MQ → Order (consomme ProductCreatedEvent)
Product → MQ → Recommendation (consomme ProductViewedEvent)
Order → MQ → Recommendation (consomme OrderCreatedEvent)
Recommendation → HTTP → Product (GET /products/{id})
```

### ❌ RÉALITÉ
```
Product → MQ (publie OK)
   ↳ Mais: Exchange incompatible avec Recommendation.API
   ↳ ProductViewedEvent N'EST JAMAIS PUBLIÉ

Order → MQ (publie OK)
   ↳ Mais: Exchange incompatible avec Recommendation.API
   ↳ Aucun Consumer dans Order.API
   ↳ Aucune vérification de stock avant création

Recommendation ← MQ (Consumers existent)
   ↳ Mais: Aucun message n'arrive (exchanecs incompatibles!)
   ↳ Aucun HttpClient pour produits
```

---

## 🔧 Actions Requises

### 1. **Uniformiser RabbitMQ** (HIGH)
   - [ ] Convertir Product.API et Order.API à MassTransit OU l'inverse
   - [ ] Utiliser le même exchange pour tous (ex: `marketplace.events`)
   - [ ] Configurer les bindings correctement

### 2. **Implémenter ProductViewedEvent** (HIGH)
   - [ ] Ajouter endpoint: `POST /api/products/{id}/view`
   - [ ] Publier ProductViewedEvent

### 3. **Implémenter Order → Product Communication** (HIGH)
   - [ ] HttpClient dans Order.API
   - [ ] Vérifier stocks avant de créer commande
   - [ ] Appeler `POST /api/products/{id}/decrement-stock`

### 4. **Ajouter Consumers à Order.API** (MEDIUM)
   - [ ] Consumer pour ProductCreatedEvent
   - [ ] Consumer pour ProductUpdatedEvent
   - [ ] Manager cache local des produits

### 5. **Implémenter Recommendation → Product Communication** (MEDIUM)
   - [ ] HttpClient pour récupérer détails produits
   - [ ] Enrichir recommandations

---

## 📝 Résumé

| Aspect | Statut | Détail |
|--------|--------|--------|
| **Product → RabbitMQ** | ✅ OK | Publie 3 événements |
| **Order → RabbitMQ** | ✅ OK | Publie 3 événements |
| **Recommendation ← RabbitMQ** | ❌ CASSÉ | Exchanges incompatibles |
| **ProductViewedEvent** | ❌ MANQUANT | N'est jamais publié |
| **Order → Product (Stocks)** | ❌ MANQUANT | Pas vérification ni décrémentation |
| **Order ← Product (Événements)** | ❌ MANQUANT | Pas de Consumers |
| **Recommendation → Product (HTTP)** | ❌ MANQUANT | Pas d'HttpClient |

