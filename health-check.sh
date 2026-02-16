#!/bin/bash

# Health Check Script for Marketplace Services

echo "╔════════════════════════════════════════════════════════════════╗"
echo "║        🏥 Marketplace Services - Health Check                 ║"
echo "╚════════════════════════════════════════════════════════════════╝"
echo ""

# Color codes
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check MongoDB
echo -n "Checking MongoDB (27017)... "
if docker exec marketplace_mongodb mongosh --eval "db.runCommand('ping')" &> /dev/null; then
    echo -e "${GREEN}✓ OK${NC}"
else
    echo -e "${RED}✗ FAILED${NC}"
fi

# Check RabbitMQ
echo -n "Checking RabbitMQ (5672)... "
if docker exec marketplace_rabbitmq rabbitmq-diagnostics -q ping &> /dev/null; then
    echo -e "${GREEN}✓ OK${NC}"
else
    echo -e "${RED}✗ FAILED${NC}"
fi

# Check RabbitMQ Management UI
echo -n "Checking RabbitMQ Management (15672)... "
if curl -s -o /dev/null -w "%{http_code}" http://localhost:15672/api/whoami 2>/dev/null | grep -q "200"; then
    echo -e "${GREEN}✓ OK${NC}"
else
    echo -e "${RED}✗ FAILED${NC}"
fi

# Check Product API
echo -n "Checking Product API (5001)... "
if curl -s http://localhost:5001/health &> /dev/null; then
    echo -e "${GREEN}✓ OK${NC}"
else
    echo -e "${YELLOW}⏳ Starting...${NC}"
fi

# Check Order API
echo -n "Checking Order API (5002)... "
if curl -s http://localhost:5002/health &> /dev/null; then
    echo -e "${GREEN}✓ OK${NC}"
else
    echo -e "${YELLOW}⏳ Starting...${NC}"
fi

echo ""
echo "╔════════════════════════════════════════════════════════════════╗"
echo "║                    Service Logs                                ║"
echo "╠════════════════════════════════════════════════════════════════╣"
echo "│                                                                ║"
echo "│  View logs:                                                    │"
echo "│    docker-compose logs -f [service-name]                      │"
echo "│                                                                │"
echo "│  Services:                                                     │"
echo "│    • mongodb                                                   │"
echo "│    • rabbitmq                                                  │"
echo "│    • product-api                                               │"
echo "│    • order-api                                                 │"
echo "│                                                                │"
echo "╚════════════════════════════════════════════════════════════════╝"
