# 📋 Guide d'Intégration et Déploiement du Product Service

## Architecture Complète du Microservice

```
Product Service (ASP.NET Core 10.0)
├── MongoDB (Données produits/catégories)
└── RabbitMQ (Événements)
```

## Étapes de Démarrage Rapide

### Option 1: Démarrage Local avec Docker Compose

```bash
# 1. Démarrer les services
docker-compose up -d

# 2. Naviguer au projet
cd Product.API

# 3. Lancer l'application
dotnet run

# L'API sera accessible sur http://localhost:5070
```

### Option 2: Utiliser le script de démarrage (Windows)

```bash
start.bat
```

### Option 3: Déployer en Docker

```bash
# Construire l'image
docker build -f Product.API/Dockerfile -t product-service:latest .

# Lancer le conteneur
docker run -p 8080:8080 \
  -e "MongoDb__ConnectionString=mongodb://mongodb:27017" \
  -e "RabbitMq__HostName=rabbitmq" \
  -e "RabbitMq__Port=5672" \
  product-service:latest
```

## Configuration des Dépendances

### MongoDB

```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "products_db",
    "ProductsCollectionName": "products",
    "CategoriesCollectionName": "categories"
  }
}
```

**Authentification optionnelle:**
```
mongodb://username:password@host:27017/database
```

### RabbitMQ

```json
{
  "RabbitMq": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "ExchangeName": "products.exchange"
  }
}
```

## Intégration avec d'autres Microservices

### 1. Consommer les événements RabbitMQ

Les services suivants peuvent s'abonner aux événements publiés:

```csharp
// Exemple pour un service Order
var connection = factory.CreateConnection();
var channel = connection.CreateChannel();

channel.ExchangeDeclare(
    exchange: "products.exchange",
    type: ExchangeType.Topic,
    durable: true,
    autoDelete: false);

var queueName = channel.QueueDeclare().QueueName;

// S'abonner aux événements de produit
channel.QueueBind(
    queue: queueName,
    exchange: "products.exchange",
    routingKey: "product.*");
```

### 2. Appeler l'API Product Service

```csharp
// En HTTP
var client = new HttpClient();
var response = await client.GetAsync("http://product-service/api/products");

// Ou via gRPC (à implémenter)
```

### 3. Événements disponibles

**ProductCreatedEvent**
```json
{
  "productId": "507f1f77bcf86cd799439011",
  "name": "iPhone 15 Pro",
  "category": "Electronics",
  "price": 999.99,
  "createdAt": "2024-02-14T12:00:00Z"
}
```

**StockUpdatedEvent**
```json
{
  "productId": "507f1f77bcf86cd799439011",
  "oldStock": 50,
  "newStock": 45,
  "updatedAt": "2024-02-14T12:05:00Z"
}
```

**ProductUpdatedEvent**
```json
{
  "productId": "507f1f77bcf86cd799439011",
  "name": "iPhone 15 Pro Max",
  "price": 1099.99,
  "updatedAt": "2024-02-14T12:10:00Z"
}
```

## Validation des Endpoints

### 1. Tester avec Curl

```bash
# Récupérer tous les produits
curl http://localhost:5070/api/products

# Créer un produit
curl -X POST http://localhost:5070/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Product Test",
    "description": "Test product",
    "category": "Test",
    "price": 99.99,
    "stock": 10,
    "imageUrl": "http://test.com/image.jpg"
  }'
```

### 2. Importer dans Postman

1. Ouvrir Postman
2. Click sur "Import"
3. Sélectionner le fichier `Product-Service.postman_collection.json`
4. Tester les endpoints

### 3. Accéder à Swagger UI

```
http://localhost:5070/swagger
```

## Configuration pour la Production

### Variables d'environnement à définir

```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
MongoDb__ConnectionString=mongodb+srv://user:password@cluster.mongodb.net/products_db?retryWrites=true&w=majority
RabbitMq__HostName=rabbitmq.yourcompany.com
RabbitMq__Port=5672
RabbitMq__UserName=<production_user>
RabbitMq__Password=<production_password>
```

### Dockerfile pour production

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app
COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Product.API.dll"]
```

## Monitoring et Logs

### Configuration Serilog (futur)

```csharp
builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("logs/product-service-.txt", 
            rollingInterval: RollingInterval.Day);
});
```

### Métriques Prometheus (futur)

```csharp
builder.Services.AddPrometheus();
app.MapPrometheusScrapingEndpoint();
```

## Checklist de Déploiement

- [ ] MongoDB configurée et accessible
- [ ] RabbitMQ configurée et accessible
- [ ] Variables d'environnement définies
- [ ] Migrations de données exécutées
- [ ] Tests passent avec succès
- [ ] Health checks en place
- [ ] Logging configuré
- [ ] Monitoring en place
- [ ] SSL/HTTPS activé
- [ ] Rate limiting configuré

## Rollback en Cas de Problème

```bash
# Récupérer la version précédente
git revert <commit-hash>

# Redéployer
docker build -t product-service:latest .
docker service update --image product-service:latest product_service
```

## Support et Troubleshooting

### Problème: Connexion à MongoDB refusée

**Solution:**
```bash
docker-compose logs mongodb
# Vérifier que MongoDB est en cours d'exécution
docker ps | grep mongodb
```

### Problème: RabbitMQ ne publie pas les messages

**Solution:**
```bash
# Vérifier les logs
docker-compose logs rabbitmq

# Accéder à la console d'administration
http://localhost:15672 (user: guest, pass: guest)
```

### Problème: Erreur 500 sur les endpoints

```bash
# Vérifier les logs de l'application
dotnet run --project Product.API

# Vérifier les variables d'environnement
echo $env:MongoDb__ConnectionString
```

## Prochaines Améliorations

1. **Caching avec Redis**
   ```csharp
   services.AddStackExchangeRedisCache(options => {...});
   ```

2. **Validation avec FluentValidation**
   ```csharp
   services.AddValidatorsFromAssemblyContaining<Program>();
   ```

3. **Rate Limiting**
   ```csharp
   services.AddRateLimiter(rateLimiterOptions => {...});
   ```

4. **Documentation OpenAPI complète**
   - Ajouter les descriptions complètes
   - Exemples de réponses
   - Codes d'erreur documentés

5. **Tests d'intégration avec TestContainers**
   ```csharp
   // Utiliser TestContainers pour les tests automatisés
   ```

6. **CQRS Pattern** (si nécessaire)
   - MediatR pour les commandes/requêtes
   - Agrégats de domaine

---

**Dernière mise à jour:** 2024-02-14
**Version:** 1.0.0
