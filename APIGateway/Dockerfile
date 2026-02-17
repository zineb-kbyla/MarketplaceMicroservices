# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file
COPY ["APIGateway/APIGateway.csproj", "APIGateway/"]

# Restore dependencies
RUN dotnet restore "APIGateway/APIGateway.csproj"

# Copy source code
COPY . .

# Build the application
RUN dotnet build "APIGateway/APIGateway.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "APIGateway/APIGateway.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "APIGateway.dll"]
