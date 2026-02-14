#!/bin/bash

# Start Docker services
echo "Starting MongoDB and RabbitMQ..."
docker-compose up -d

# Wait for services to be ready
echo "Waiting for services to start..."
sleep 5

# Restore dependencies
echo "Restoring NuGet packages..."
cd Product.API
dotnet restore

# Build the project
echo "Building the project..."
dotnet build

# Run the application
echo "Starting the application..."
dotnet run
