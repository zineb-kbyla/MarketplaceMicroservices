@echo off

REM Start Docker services
echo Starting MongoDB and RabbitMQ...
docker-compose up -d

REM Wait for services to be ready
echo Waiting for services to start...
timeout /t 5

REM Navigate to Product.API
cd Product.API

REM Restore dependencies
echo Restoring NuGet packages...
dotnet restore

REM Build the project
echo Building the project...
dotnet build

REM Run the application
echo Starting the application...
dotnet run

pause
