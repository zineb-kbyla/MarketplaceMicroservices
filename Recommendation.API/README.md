# Recommendation.API - Microservice de Recommandations

## Vue d'ensemble

Service de recommandations basé sur **Neo4j** (Graph Database) utilisant des algorithmes de **filtrage collaboratif** et **filtrage basé sur le contenu** pour fournir des recommandations personnalisées aux utilisateurs.

## Architecture

### Couches d'Architecture

#### 1. **Domain Layer**
- **UserNode**: Représente les utilisateurs dans le graphe Neo4j
- **ProductNode**: Représente les produits
- **PurchaseRelation**: Relations d'achat entre utilisateurs et produits
- **ViewRelation**: Historique de visualisation des produits
- **SimilarityRelation**: Relations de similarité entre produits

#### 2. **Application Layer**
- **RecommendationService**: Service principal orchestrant les recommandations
- **CollaborativeFilteringAlgorithm**: Algorithme de filtrage collaboratif
- **ContentBasedFilteringAlgorithm**: Algorithme basé sur le contenu
- **DTOs**: Objets de transfert de données

#### 3. **Infrastructure Layer**
- **Neo4jContext**: Gestion de connexion Neo4j
- **RecommendationRepository**: Requêtes Cypher
- **Event Consumers**: Consommateurs RabbitMQ

#### 4. **API Layer**
- **RecommendationsController**: Endpoints REST

## Endpoints API

### Recommandations

| Méthode | Route | Description |
|---------|-------|-------------|
| GET | `/api/recommendations/{userId}` | Recommandations personnalisées |
| GET | `/api/recommendations/{userId}?limit=10` | Avec limite custom |
| GET | `/api/recommendations/similar/{productId}` | Produits similaires |
| GET | `/api/recommendations/trending` | Produits tendances |
| GET | `/api/recommendations/trending?days=14&limit=5` | Tendances filtrées |
| GET | `/api/recommendations/history/{userId}` | Historique utilisateur |
| POST | `/api/recommendations/view` | Enregistrer une vue |
| POST | `/api/recommendations/purchase` | Enregistrer un achat |

### Exemples de Requêtes

#### Obtenir des recommandations personnalisées
```bash
curl -X GET "http://localhost:8004/api/recommendations/user123?limit=10"
```

#### Enregistrer une vue
```bash
curl -X POST "http://localhost:8004/api/recommendations/view" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user123",
    "productId": "prod456",
    "duration": 45,
    "source": "web"
  }'
```

#### Enregistrer un achat
```bash
curl -X POST "http://localhost:8004/api/recommendations/purchase" \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user123",
    "orderId": "order789",
    "items": [
      {
        "productId": "prod456",
        "quantity": 2,
        "price": 99.99
      }
    ]
  }'
```

## Algorithmes

### Filtrage Collaboratif

Le filtrage collaboratif trouve des utilisateurs ayant des achats similaires et recommande les produits qu'ils ont achetés.

**Processus**:
1. Trouver les 20 utilisateurs les plus similaires
2. Récupérer les produits achetés par ces utilisateurs
3. Exclure les produits déjà achetés
4. Classer par popularité

**Requête Cypher** ("Utilisateurs similaires"):
```cypher
MATCH (u:User {userId: $userId})-[:PURCHASED]->(p:Product)
      <-[:PURCHASED]-(similar:User)
WHERE similar.userId <> $userId
WITH similar, COUNT(DISTINCT p) as commonPurchases
ORDER BY commonPurchases DESC
LIMIT 20
RETURN similar.userId, commonPurchases
```

### Filtrage Basé sur le Contenu

Le filtrage basé sur le contenu recommande des produits similaires basés sur les catégories et attributs.

**Processus**:
1. Trouver les produits de la même catégorie
2. Calculer la similarité basée sur les prix
3. Retourner les plus similaires

**Requête Cypher**:
```cypher
MATCH (p:Product {productId: $productId})
MATCH (similar:Product)
WHERE similar.productId <> $productId
  AND similar.category = p.category
RETURN similar.productId, similar.name, similar.price
ORDER BY similar.purchaseCount DESC
LIMIT 5
```

## Modèle de Graphe Neo4j

### Nodes
- **:User** - Utilisateurs
- **:Product** - Produits

### Relations
- **PURCHASED** - Achat d'un produit
  - Propriétés: `orderId`, `purchaseDate`, `quantity`, `price`
- **VIEWED** - Visualisation d'un produit
  - Propriétés: `viewedAt`, `duration`, `source`
- **SIMILAR_TO** - Produits similaires
  - Propriétés: `score`, `calculatedAt`

### Exemple de Graphe

```
User1 --[PURCHASED]--> Product1
User1 --[PURCHASED]--> Product3
User1 --[VIEWED]-----> Product2

User2 --[PURCHASED]--> Product1
User2 --[PURCHASED]--> Product2

Product1 --[SIMILAR_TO]--> Product3 (score: 0.85)
```

## Configuration

### appsettings.json

```json
{
  "Neo4j": {
    "Uri": "bolt://neo4j:7687",
    "Username": "neo4j",
    "Password": "password",
    "Database": "neo4j",
    "ConnectionTimeout": 30
  },
  "RabbitMQ": {
    "Host": "rabbitmq",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest"
  }
}
```

### Construction du Projet

```bash
cd Recommendation.API
dotnet build
```

### Exécution Locale

```bash
dotnet run --environment Development
```

### Tests

**Tests Unitaires**:
```bash
dotnet test --filter "FullyQualifiedName~Unit"
```

**Tests d'Intégration**:
```bash
dotnet test --filter "FullyQualifiedName~Integration"
```

**Tous les tests**:
```bash
dotnet test
```

## Docker Compose

Le service s'exécute sur le port `8004` avec:
- **Neo4j**: Port 7687 (bolt)
- **RabbitMQ**: Port 5672

### Services Dépendants

```yaml
neo4j:
  image: neo4j:5.x
  ports:
    - "7687:7687"
  environment:
    NEO4J_AUTH: neo4j/password

rabbitmq:
  image: rabbitmq:3.12-management
  ports:
    - "5672:5672"
    - "15672:15672"

recommendation-api:
  build:
    context: .
    dockerfile: Recommendation.API/Dockerfile
  ports:
    - "8004:8004"
  depends_on:
    - neo4j
    - rabbitmq
```

## Requêtes Cypher Principales

### 1. Créer un utilisateur
```cypher
CREATE (u:User {
  userId: $userId,
  name: $name,
  email: $email,
  joinedDate: datetime(),
  lastActive: datetime()
})
```

### 2. Enregistrer un achat
```cypher
MATCH (u:User {userId: $userId})
MATCH (p:Product {productId: $productId})
CREATE (u)-[:PURCHASED {
  orderId: $orderId,
  purchaseDate: datetime(),
  quantity: $quantity,
  price: $price
}]->(p)
```

### 3. Trouver les utilisateurs similaires
```cypher
MATCH (u:User {userId: $userId})-[:PURCHASED]->(p:Product)
      <-[:PURCHASED]-(similar:User)
WHERE similar.userId <> $userId
WITH similar, COUNT(DISTINCT p) as commonPurchases
ORDER BY commonPurchases DESC
LIMIT 20
RETURN similar.userId, commonPurchases
```

### 4. Obtenir des recommandations personnalisées
```cypher
MATCH (u:User {userId: $userId})
MATCH (similar:User)-[:PURCHASED]->(rec:Product)
WHERE similar.userId IN $similarUserIds
  AND NOT (u)-[:PURCHASED]->(rec)
WITH rec, COUNT(DISTINCT similar) as popularity
ORDER BY popularity DESC, rec.purchaseCount DESC
LIMIT 10
RETURN rec.productId, rec.name, rec.category, rec.price, popularity
```

### 5. Obtenir des produits tendances
```cypher
MATCH (u:User)-[r:PURCHASED]->(p:Product)
WHERE r.purchaseDate >= datetime() - duration({days: 7})
WITH p, COUNT(r) as recentPurchases
ORDER BY recentPurchases DESC
LIMIT 10
RETURN p.productId, p.name, p.category, recentPurchases
```

## Événements Consommés

### OrderCreatedEvent
Déclenché lorsqu'une commande est créée. Enregistre tous les achats dans Neo4j.

```csharp
public class OrderCreatedEvent
{
    public string OrderId { get; set; }
    public string UserId { get; set; }
    public List<OrderItemEvent> Items { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### ProductViewedEvent
Événement optionnel pour tracker les visualisations de produits.

```csharp
public class ProductViewedEvent
{
    public string UserId { get; set; }
    public string ProductId { get; set; }
    public DateTime ViewedAt { get; set; }
    public int Duration { get; set; }
    public string Source { get; set; }
}
```

## Structure du Projet

```
Recommendation.API/
├── Domain/
│   ├── Entities/
│   │   ├── UserNode.cs
│   │   ├── ProductNode.cs
│   │   ├── PurchaseRelation.cs
│   │   ├── ViewRelation.cs
│   │   └── SimilarityRelation.cs
│   └── Events/
│       └── DomainEvents.cs
│
├── Application/
│   ├── DTOs/
│   │   └── RecommendationDtos.cs
│   ├── Interfaces/
│   │   └── IRepositories.cs
│   ├── Services/
│   │   └── RecommendationService.cs
│   └── Algorithms/
│       ├── CollaborativeFilteringAlgorithm.cs
│       └── ContentBasedFilteringAlgorithm.cs
│
├── Infrastructure/
│   ├── Data/
│   │   ├── Neo4jContext.cs
│   │   └── Neo4jSettings.cs
│   ├── Repositories/
│   │   └── RecommendationRepository.cs
│   └── Messaging/
│       ├── OrderCreatedEventConsumer.cs
│       └── ProductViewedEventConsumer.cs
│
├── API/
│   ├── Controllers/
│   │   └── RecommendationsController.cs
│   └── Program.cs
│
├── Tests/
│   ├── Unit/
│   │   ├── RecommendationServiceTests.cs
│   │   ├── CollaborativeFilteringTests.cs
│   │   └── ContentBasedFilteringTests.cs
│   └── Integration/
│       └── RecommendationsControllerIntegrationTests.cs
│
├── appsettings.json
├── appsettings.Development.json
├── Dockerfile
└── Recommendation.API.csproj
```

## Performance et Optimisation

### Considérations Neo4j
- Les requêtes Cypher sont optimisées avec les index sur `userId` et `productId`
- Les relations `PURCHASED` et `VIEWED` sont pondérées pour les calculs de similarité
- Les résultats sont limités pour éviter les requêtes trop coûteuses

### Caching
- Les recommandations personnalisées pourraient être cachées dans Redis
- Les produits tendances sont recalculés toutes les 24h

### Scalabilité
- Partition par utilisateur pour les graphes volumineux
- Réplication Neo4j pour la haute disponibilité
- Load balancing avec plusieurs instances de Recommendation.API

## Troubleshooting

### Erreur: "Cannot establish connection to Neo4j"
- Vérifier que Neo4j est en cours d'exécution sur le port 7687
- Vérifier les identifiants: `neo4j` / `password`
- Vérifier l'URI: `bolt://neo4j:7687` ou `bolt://localhost:7687`

### Pas de recommandations retournées
- Vérifier que des achats ont été enregistrés
- Vérifier que l'utilisateur a des pairs similaires
- Consulter les logs pour plus de détails

## Références

- [Neo4j Driver Documentation](https://neo4j.com/docs/driver-manual/5.0)
- [Cypher Query Language](https://neo4j.com/docs/cypher-manual/5.0)
- [MassTransit Documentation](https://masstransit.io)
