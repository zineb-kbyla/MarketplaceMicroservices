# Marketplace Microservices - Useful Commands

## 🚀 Getting Started

### Start All Services
```bash
docker-compose up -d
```

### Stop All Services
```bash
docker-compose down
```

### View Running Services
```bash
docker-compose ps
```

---

## 📊 Logs

### View All Logs
```bash
docker-compose logs -f
```

### View Service-Specific Logs
```bash
# Product API logs
docker-compose logs -f product-api

# Order API logs
docker-compose logs -f order-api

# MongoDB logs
docker-compose logs -f mongodb

# RabbitMQ logs
docker-compose logs -f rabbitmq
```

### View Last 100 Lines
```bash
docker-compose logs --tail=100 order-api
```

---

## 🗄️ MongoDB Commands

### Connect to MongoDB
```bash
docker exec -it marketplace_mongodb mongosh
```

### Inside mongosh:

```javascript
// Show databases
show databases

// Switch to order database
use marketplace_order

// Show collections
show collections

// Count orders
db.orders.countDocuments()

// Find all orders
db.orders.find().pretty()

// Find orders by userId
db.orders.find({ userId: "user123" }).pretty()

// Find specific order
db.orders.findOne({ _id: ObjectId("...") })

// Check indexes
db.orders.getIndexes()

// Delete all orders (careful!)
db.orders.deleteMany({})

// Exit
exit
```

---

## 🐰 RabbitMQ Commands

### Access RabbitMQ Management UI
```
http://localhost:15672
Username: guest
Password: guest
```

### Check RabbitMQ Status
```bash
docker exec -it marketplace_rabbitmq rabbitmq-diagnostics status
```

### List Queues
```bash
docker exec -it marketplace_rabbitmq rabbitmqctl list_queues
```

### List Exchanges
```bash
docker exec -it marketplace_rabbitmq rabbitmqctl list_exchanges
```

### List Bindings
```bash
docker exec -it marketplace_rabbitmq rabbitmqctl list_bindings
```

---

## 🔌 API Testing with cURL

### Create Order
```bash
curl -X POST http://localhost:5002/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user123",
    "userName": "John Doe",
    "items": [
      {
        "productId": "prod1",
        "productName": "Product 1",
        "quantity": 1,
        "unitPrice": 99.99,
        "totalPrice": 99.99
      }
    ],
    "shippingAddress": {
      "street": "123 Main St",
      "city": "New York",
      "state": "NY",
      "country": "USA",
      "zipCode": "10001",
      "phoneNumber": "1234567890"
    },
    "paymentInfo": {
      "cardName": "John Doe",
      "cardNumber": "4111111111111111",
      "expiration": "12/26",
      "cvv": "123",
      "paymentMethod": "CreditCard"
    }
  }'
```

### Get User Orders
```bash
curl http://localhost:5002/api/orders?userId=user123
```

### Get Order Details
```bash
curl http://localhost:5002/api/orders/{orderId}
```

### Update Order Status
```bash
curl -X PUT http://localhost:5002/api/orders/{orderId}/status \
  -H "Content-Type: application/json" \
  -d '{"status": "Confirmed"}'
```

### Cancel Order
```bash
curl -X DELETE http://localhost:5002/api/orders/{orderId}
```

### Track Order
```bash
curl http://localhost:5002/api/orders/{orderId}/tracking
```

---

## 🏗️ Build Commands

### Build All Projects
```bash
dotnet build
```

### Build Specific Project
```bash
dotnet build Order.API/Order.API.csproj
```

### Clean Build
```bash
dotnet clean
dotnet build
```

---

## 🧪 Test Commands

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Project
```bash
dotnet test Order.API/Tests/Unit/
```

### Run with Code Coverage
```bash
dotnet test /p:CollectCoverage=true
```

---

## 🐳 Docker Commands

### Build Order.API Image
```bash
docker build -t order-service:latest Order.API/
```

### Run Order.API Container
```bash
docker run -p 5002:5002 -e MongoDb__ConnectionString="mongodb://host.docker.internal:27017" order-service:latest
```

### List Docker Images
```bash
docker images | grep order
docker images | grep product
```

### Remove Image
```bash
docker rmi order-service:latest
```

### View Container Stats
```bash
docker stats
```

---

## 📦 NuGet Package Management

### Add NuGet Package
```bash
cd Order.API
dotnet add package <PackageName>
```

### Update Package
```bash
dotnet package update <PackageName>
```

### List Installed Packages
```bash
dotnet list package
```

---

## 🔐 Secrets Management

### Set Secret (Development)
```bash
dotnet user-secrets init
dotnet user-secrets set "MongoDb:ConnectionString" "mongodb://..."
```

### List Secrets
```bash
dotnet user-secrets list
```

### Remove Secret
```bash
dotnet user-secrets remove "MongoDb:ConnectionString"
```

---

## 📈 Performance Commands

### Monitor Services
```bash
docker stats
```

### Check Disk Usage
```bash
docker system df
```

### Cleanup Unused Resources
```bash
docker system prune -a
```

---

## 🔧 Troubleshooting

### Check Service Connectivity
```bash
# From Order API container to Product API
docker exec -it order_api curl http://product-api:5001/health

# From Order API container to MongoDB
docker exec -it order_api mongosh mongodb://mongodb:27017
```

### Check Port Usage
```bash
# Linux/Mac
lsof -i :5002
lsof -i :27017

# Windows
netstat -ano | findstr :5002
```

### Restart Service
```bash
docker-compose restart order-api
docker-compose restart product-api
```

### Force Recreate Containers
```bash
docker-compose up -d --force-recreate
```

---

## 📚 Documentation

### View Service README
```bash
cat Order.API/README.md
cat Product.API/README.md
```

### View Architecture
```bash
cat Order.API/ARCHITECTURE.md
```

### View Deployment Guide
```bash
cat Order.API/DEPLOYMENT_GUIDE.md
```

---

## 🎯 Quick Checklist

- [ ] Services running: `docker-compose ps`
- [ ] MongoDB connected: `curl -s http://localhost:27017`
- [ ] Product API ready: `curl -s http://localhost:5001/health`
- [ ] Order API ready: `curl -s http://localhost:5002/health`
- [ ] RabbitMQ UI: http://localhost:15672
- [ ] Swagger Order API: http://localhost:5002/swagger
- [ ] Swagger Product API: http://localhost:5001/swagger

---

## 💡 Pro Tips

1. **Use `docker-compose ps -a`** to see all containers including stopped ones
2. **Use `docker-compose logs -f --tail=50`** to see last 50 lines in real-time
3. **Use health checks** to verify services are ready
4. **Use `.env` files** for environment variables in docker-compose
5. **Always backup MongoDB** before major operations
6. **Monitor RabbitMQ** to prevent message queue buildup

---

Last Updated: 2024-02-16
For more help: See README.md and DEPLOYMENT_GUIDE.md
