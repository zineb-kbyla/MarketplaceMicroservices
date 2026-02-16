@echo off
REM Quick Start Script for Marketplace Microservices (Windows)

echo.
echo ╔════════════════════════════════════════════════════════════════╗
echo ║    🛍️  Marketplace Microservices - Quick Start (Windows)      ║
echo ╚════════════════════════════════════════════════════════════════╝
echo.

REM Check Docker
echo Checking Docker installation...
where docker >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ✗ Docker is not installed. Please install Docker Desktop first.
    pause
    exit /b 1
)

echo ✓ Docker found

REM Check Docker Compose
echo Checking Docker Compose installation...
where docker-compose >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ✗ Docker Compose is not installed. Please install Docker Desktop first.
    pause
    exit /b 1
)

echo ✓ Docker Compose found

REM Start services
echo.
echo 📦 Starting marketplace services...
echo    - MongoDB (Port 27017)
echo    - RabbitMQ (Port 5672, UI: 15672)
echo    - Product API (Port 5001)
echo    - Order API (Port 5002)
echo.

docker-compose up -d

REM Wait for services
echo ⏳ Waiting for services to start (10 seconds)...
timeout /t 10 /nobreak

REM Success message
echo.
echo ╔════════════════════════════════════════════════════════════════╗
echo ║              ✅ Services Started Successfully!                ║
echo ╠════════════════════════════════════════════════════════════════╣
echo ║                                                                ║
echo ║  📍 Service URLs:                                             ║
echo ║     • Product API: http://localhost:5001                      ║
echo ║     • Order API: http://localhost:5002                        ║
echo ║     • MongoDB: localhost:27017                                ║
echo ║     • RabbitMQ: http://localhost:15672                        ║
echo ║       (UserName: guest, Password: guest)                      ║
echo ║                                                                ║
echo ║  📚 Documentation:                                            ║
echo ║     • Product API Swagger: http://localhost:5001/swagger      ║
echo ║     • Order API Swagger: http://localhost:5002/swagger        ║
echo ║                                                                ║
echo ║  📮 Testing:                                                  ║
echo ║     • Import Postman collections from:                        ║
echo ║       Product.API\Product-Service.postman_collection.json     ║
echo ║       Order.API\Order-Service.postman_collection.json         ║
echo ║                                                                ║
echo ║  🛑 Stop services:                                            ║
echo ║     docker-compose down                                       ║
echo ║                                                                ║
echo ║  📋 View logs:                                                ║
echo ║     docker-compose logs -f [service-name]                   ║
echo ║                                                                ║
echo ╚════════════════════════════════════════════════════════════════╝
echo.
echo 💡 Quick Test Commands:
echo.
echo  # Get orders by user
echo  curl http://localhost:5002/api/orders?userId=user123
echo.
echo  # Get products
echo  curl http://localhost:5001/api/products
echo.
echo  # RabbitMQ Management UI
echo  Start: http://localhost:15672
echo.
pause
