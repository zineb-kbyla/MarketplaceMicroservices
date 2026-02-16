# Order Service API

Service de gestion des commandes pour la marketplace.

## Description

Le service Order gère le cycle de vie complet des commandes:
- Création de commande
- Suivi du statut
- Annulation de commande
- Historique des commandes par utilisateur

## Endpoints API

### Orders

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/orders` | Liste toutes les commandes |
| GET | `/api/orders?userId={userId}` | Commandes filtrées par utilisateur |
| GET | `/api/orders/{id}` | Détails d'une commande |
| GET | `/api/orders/user/{userId}` | Commandes d'un utilisateur |
| GET | `/api/orders/{id}/tracking` | Suivi de commande |
| POST | `/api/orders` | Créer une commande |
| PUT | `/api/orders/{id}/status` | Modifier le statut |
| DELETE | `/api/orders/{id}` | Annuler une commande |

## Modèle de Données

### Order

```json
{
  "id": "string",
  "orderNumber": "ORD-20240201-ABC12345",
  "userId": "string",
  "userName": "string",
  "totalAmount": 1299.99,
  "status": "Pending",
  "orderItems": [
    {
      "productId": "string",
      "productName": "string",
      "quantity": 1,
      "unitPrice": 999.99,
      "totalPrice": 999.99
    }
  ],
  "shippingAddress": {
    "street": "string",
    "city": "string",
    "state": "string",
    "country": "string",
    "zipCode": "string",
    "phoneNumber": "string"
  },
  "paymentInfo": {
    "cardName": "string",
    "cardNumber": "****1234",
    "expiration": "12/26",
    "cvv": "***",
    "paymentMethod": "CreditCard"
  },
  "createdAt": "2024-02-01T10:30:00Z",
  "updatedAt": "2024-02-01T11:00:00Z"
}
```

### Statuts de Commande

- **Pending**: Commande créée, en attente de confirmation
- **Confirmed**: Paiement confirmé
- **Processing**: Commande en cours de préparation
- **Shipped**: Commande expédiée
- **Delivered**: Commande livrée
- **Cancelled**: Commande annulée
- **Refunded**: Commande remboursée

## Événements RabbitMQ

Le service publie les événements suivants:

### OrderCreatedEvent
```json
{
  "orderId": "string",
  "userId": "string",
  "items": [...],
  "totalAmount": 1299.99,
  "createdAt": "2024-02-01T10:30:00Z"
}
```

### OrderStatusChangedEvent
```json
{
  "orderId": "string",
  "oldStatus": "Pending",
  "newStatus": "Confirmed",
  "changedAt": "2024-02-01T11:00:00Z"
}
```

### OrderCancelledEvent
```json
{
  "orderId": "string",
  "reason": "Cancelled by user",
  "cancelledAt": "2024-02-01T12:00:00Z"
}
```

## Configuration

### appsettings.json

```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "orders_db",
    "OrdersCollectionName": "orders"
  },
  "RabbitMq": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "ExchangeName": "orders.exchange"
  }
}
```

## Démarrage

### Prérequis
- .NET 10.0 SDK
- MongoDB
- RabbitMQ

### Lancer le service

```bash
cd Order.API
dotnet restore
dotnet run
```

L'API sera disponible sur:
- HTTPS: https://localhost:7002
- HTTP: http://localhost:5002
- Documentation: https://localhost:7002/scalar/v1

## Docker

```bash
docker build -t order-api .
docker run -p 5002:8080 order-api
```

## Tests

```bash
dotnet test
```

## Architecture

Le service suit une architecture Clean Architecture avec:

- **Domain Layer**: Entités, Enums, Events
- **Application Layer**: DTOs, Services, Interfaces
- **Infrastructure Layer**: Repositories, MongoDB, RabbitMQ
- **API Layer**: Controllers, Middleware

## Dépendances

- MongoDB.Driver
- RabbitMQ.Client
- AutoMapper
- MediatR (pour CQRS)
