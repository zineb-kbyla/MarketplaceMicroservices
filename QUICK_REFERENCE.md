# 📊 Quick Reference - API Gateway Setup

## 🎯 Architecture Finale

```
┌─────────────────────────────────────────────────────────────────┐
│                     🌐 EXTERNAL CLIENTS                          │
│                  (Frontend, Web, Mobile)                         │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                  http://localhost:5000
                  or http://api-gateway:5000
                             │
        ┌────────────────────▼────────────────────┐
        │        🚪 API GATEWAY (YARP)            │
        │          Port 5000                      │
        │  ┌──────────────────────────────────┐   │
        │  │  • Routing (Path-based)          │   │
        │  │  • Load Balancing                │   │
        │  │  • Health Checks                 │   │
        │  │  • Logging                       │   │
        │  │  • CORS                          │   │
        │  │  • Rate Limiting (future)        │   │
        │  └──────────────────────────────────┘   │
        └──┬────────────────────┬─────────────┬───┘
           │                    │             │
    /api/products/*     /api/orders/*   /api/recommendations/*
           │                    │             │
    ┌──────▼─────────┐  ┌──────▼─────────┐  ┌──────▼──────────┐
    │  📦 PRODUCT    │  │  📦 ORDER      │  │ 📦 RECOMMENDATION│
    │  API           │  │  API           │  │ API              │
    │  Port 5001     │  │  Port 5002     │  │ Port 8004        │
    │                │  │                │  │                  │
    │ • MongoDB      │  │ • MongoDB      │  │ • Neo4j          │
    │ • RabbitMQ     │  │ • RabbitMQ     │  │ • RabbitMQ       │
    │ • Cache        │  │ • Cache        │  │ • Algorithms     │
    └────────────────┘  └────────────────┘  └──────────────────┘
           │                    │             │
           └────────┬───────────┴─────────────┘
                    │
         ┌──────────┼──────────┐
         │          │          │
         ▼          ▼          ▼
    ┌────────┐ ┌────────┐ ┌────────┐
    │MongoDB │ │RabbitMQ│ │ Neo4j  │
    │27017   │ │5672    │ │7687    │
    └────────┘ └────────┘ └────────┘
```

---

## 🚀 Commandes Essentielles

### Docker Mode
```bash
# Démarrer tout
docker-compose up --build

# Vérifier le statut
docker-compose ps

# Logs
docker logs api_gateway -f
docker logs product_api
docker logs order_api
docker logs recommendation_api

# Arrêter
docker-compose down
```

### Local Mode
```bash
# Terminal 1: Gateway
cd APIGateway && dotnet run

# Terminal 2: Product  
cd Product.API && dotnet run

# Terminal 3: Order
cd Order.API && dotnet run

# Terminal 4: Recommendation
cd Recommendation.API && dotnet run
```

---

## 🧪 Tests Rapides

```bash
# Health Check
curl http://localhost:5000/health

# Products
curl http://localhost:5000/api/products

# Orders
curl http://localhost:5000/api/orders

# Recommendations
curl http://localhost:5000/api/recommendations/user123

# Créer un produit
curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -d '{"name":"Test","description":"Test","category":"Electronics","price":99.99,"stock":10,"imageUrl":"http://test.com"}'
```

---

## 📡 Endpoints Par Service

### Product Service (`/api/products/*`)
```
GET    /api/products
GET    /api/products/{id}
POST   /api/products
PUT    /api/products/{id}
DELETE /api/products/{id}
GET    /api/products/category/{name}
GET    /api/products/search?q={query}
POST   /api/products/{id}/decrement-stock
```

### Order Service (`/api/orders/*`)
```
GET    /api/orders
GET    /api/orders/{id}
POST   /api/orders
PUT    /api/orders/{id}
DELETE /api/orders/{id}
PUT    /api/orders/{id}/status
```

### Recommendation Service (`/api/recommendations/*`)
```
GET    /api/recommendations/{userId}
GET    /api/recommendations/{userId}/similar-users
POST   /api/recommendations/refresh
```

---

## 🔧 Configuration Files

### Gateway Development
- `APIGateway/appsettings.json` - Dev settings (localhost)
- `APIGateway/appsettings.Development.json` - Debug settings

### Gateway Docker
- `APIGateway/appsettings.Docker.json` - Docker DNS (product-api:5001, etc)

### Services
- `Product.API/appsettings.json`
- `Order.API/appsettings.json`
- `Recommendation.API/appsettings.json`

### Orchestration
- `docker-compose.yml` - Main orchestration
- `Dockerfile` - Individual service builds

---

## 📊 Ports

| Service | Port | Access |
|---------|------|--------|
| API Gateway | 5000 | http://localhost:5000 |
| Product API | 5001 | Via Gateway |
| Order API | 5002 | Via Gateway |
| Recommendation API | 8004 | Via Gateway |
| MongoDB | 27017 | Direct (dev only) |
| RabbitMQ | 5672 | Internal |
| RabbitMQ UI | 15672 | http://localhost:15672 |
| Neo4j | 7687 | Internal |
| Neo4j UI | 7474 | http://localhost:7474 |

---

## ⚡ Quick Troubleshooting

### Gateway not responding
```bash
# Check if running
netstat -an | grep 5000

# Check health
curl http://localhost:5000/health

# Check logs
docker logs api_gateway
```

### Service not found (504 Bad Gateway)
```bash
# Verify service is running
docker ps | grep product_api

# Check service health
curl http://localhost:5001/health  # Direct access

# Check gateway logs
docker logs api_gateway -f
```

### Port already in use
```bash
# Find process using port
lsof -i :5000

# Kill process or change port in launchSettings.json
```

### Docker network issues
```bash
# Check network
docker network ls
docker network inspect marketplace_network

# Restart containers
docker-compose restart
```

---

## 📚 Documentation Quick Links

| Document | Purpose |
|----------|---------|
| [GATEWAY_SETUP.md](GATEWAY_SETUP.md) | Detailed configuration |
| [GATEWAY_TESTING.md](GATEWAY_TESTING.md) | Complete test suite |
| [API_ENDPOINTS.md](API_ENDPOINTS.md) | All endpoints with examples |
| [IMPLEMENTATION_CHECKLIST.md](IMPLEMENTATION_CHECKLIST.md) | What was done |
| [COMMUNICATION_ANALYSIS.md](COMMUNICATION_ANALYSIS.md) | Microservices analysis |

---

## ✅ Production Checklist

- [ ] Enable HTTPS/TLS
- [ ] Configure JWT Authentication
- [ ] Implement Rate Limiting
- [ ] Set up Logging/Monitoring (ELK)
- [ ] Configure Secrets Management
- [ ] Health Check monitoring
- [ ] Load testing (ab, locust)
- [ ] Security audit
- [ ] Documentation review
- [ ] Team training

---

## 🎉 Summary

✅ **4 Microservices** - All running on .NET 10.0
✅ **API Gateway** - Single entry point (port 5000)
✅ **Docker Compose** - Complete stack orchestration
✅ **Documentation** - Comprehensive setup guides
✅ **Ready to Deploy** - Just run `docker-compose up`

---

**For more details, see the documentation files listed above.** 📚

