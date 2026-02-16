# Recommendation.API - Postman Testing Guide

## Table of Contents
1. [Base URL](#base-url)
2. [GET Endpoints](#get-endpoints)
3. [POST Endpoints](#post-endpoints)
4. [Test Scenarios](#test-scenarios)
5. [Error Cases](#error-cases)

---

## Base URL

```
http://localhost:5000/api/recommendations
```

**Port**: 5000 (default ASP.NET Core)  
**Api Prefix**: `/api/recommendations`

---

## GET Endpoints

### 1. Get Personalized Recommendations

**Endpoint**: `GET /api/recommendations/{userId}`

**Description**: Get personalized product recommendations for a specific user based on collaborative filtering.

**Parameters**:
- `userId` (path, required): The user identifier
- `limit` (query, optional): Maximum number of recommendations to return (default: 10, max: 50)

**Request**:
```http
GET /api/recommendations/user-123?limit=10
```

**Response** (200 OK):
```json
[
  {
    "productId": "prod-001",
    "name": "Laptop Dell XPS 15",
    "category": "Electronics",
    "price": 1299.99,
    "imageUrl": "https://example.com/laptop.jpg",
    "rating": 4.8,
    "score": 0.92,
    "reason": "15 utilisateurs similaires ont acheté ce produit",
    "confidence": 0.85
  },
  {
    "productId": "prod-002",
    "name": "Wireless Mouse",
    "category": "Accessories",
    "price": 49.99,
    "imageUrl": "https://example.com/mouse.jpg",
    "rating": 4.5,
    "score": 0.88,
    "reason": "Populaire parmi les utilisateurs ayant des goûts similaires",
    "confidence": 0.75
  }
]
```

**Postman Test**:
```javascript
// Status code test
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

// Response structure test
pm.test("Response is an array", function () {
    pm.expect(pm.response.json()).to.be.an('array');
});

// Array has items
pm.test("Array contains recommendations", function () {
    pm.expect(pm.response.json().length).to.be.greaterThan(0);
});

// Item properties test
pm.test("Each recommendation has required properties", function () {
    var recommendations = pm.response.json();
    recommendations.forEach(function (rec) {
        pm.expect(rec).to.have.property('productId');
        pm.expect(rec).to.have.property('name');
        pm.expect(rec).to.have.property('category');
        pm.expect(rec).to.have.property('price');
        pm.expect(rec).to.have.property('score');
        pm.expect(rec).to.have.property('confidence');
    });
});

// Limit test
pm.test("Returned items do not exceed limit", function () {
    var limit = parseInt(pm.request.url.query.get('limit')) || 10;
    pm.expect(pm.response.json().length).to.be.lessThanOrEqual(limit);
});
```

---

### 2. Get Similar Products

**Endpoint**: `GET /api/recommendations/similar/{productId}`

**Description**: Get products similar to a specific product based on category and content similarity.

**Parameters**:
- `productId` (path, required): The product identifier
- `limit` (query, optional): Maximum number of similar products (default: 5, max: 20)

**Request**:
```http
GET /api/recommendations/similar/prod-001?limit=5
```

**Response** (200 OK):
```json
[
  {
    "productId": "prod-003",
    "name": "Laptop HP Pavilion 15",
    "category": "Electronics",
    "price": 899.99,
    "similarityScore": 0.89,
    "reason": "Same category - High-performance laptop"
  },
  {
    "productId": "prod-004",
    "name": "Laptop ASUS ROG",
    "category": "Electronics",
    "price": 1499.99,
    "similarityScore": 0.85,
    "reason": "Same category - Gaming laptop"
  }
]
```

**Postman Test**:
```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Response is an array of similar products", function () {
    pm.expect(pm.response.json()).to.be.an('array');
});

pm.test("Similar products have required properties", function () {
    var products = pm.response.json();
    products.forEach(function (prod) {
        pm.expect(prod).to.have.property('productId');
        pm.expect(prod).to.have.property('name');
        pm.expect(prod).to.have.property('category');
        pm.expect(prod).to.have.property('price');
        pm.expect(prod).to.have.property('similarityScore');
    });
});

pm.test("Similarity score is between 0 and 1", function () {
    var products = pm.response.json();
    products.forEach(function (prod) {
        pm.expect(prod.similarityScore).to.be.within(0, 1);
    });
});
```

---

### 3. Get Trending Products

**Endpoint**: `GET /api/recommendations/trending`

**Description**: Get trending products based on recent purchase activity.

**Parameters**:
- `days` (query, optional): Number of days to look back (default: 7)
- `limit` (query, optional): Maximum number of products (default: 10, max: 50)

**Request**:
```http
GET /api/recommendations/trending?days=7&limit=10
```

**Response** (200 OK):
```json
[
  {
    "productId": "prod-005",
    "name": "iPhone 15 Pro",
    "category": "Electronics",
    "price": 999.99,
    "recentPurchases": 145,
    "trendScore": 1.0
  },
  {
    "productId": "prod-006",
    "name": "AirPods Pro",
    "category": "Accessories",
    "price": 249.99,
    "recentPurchases": 98,
    "trendScore": 0.98
  }
]
```

**Postman Test**:
```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Response contains trending products", function () {
    var products = pm.response.json();
    pm.expect(products).to.be.an('array');
    if (products.length > 0) {
        pm.expect(products[0]).to.have.property('productId');
        pm.expect(products[0]).to.have.property('trendScore');
    }
});

pm.test("TrendScore is between 0 and 1", function () {
    var products = pm.response.json();
    products.forEach(function (prod) {
        pm.expect(prod.trendScore).to.be.within(0, 1);
    });
});
```

---

### 4. Get User History

**Endpoint**: `GET /api/recommendations/history/{userId}`

**Description**: Get the purchase/view history for a user.

**Parameters**:
- `userId` (path, required): The user identifier
- `limit` (query, optional): Maximum number of history items (default: 20)

**Request**:
```http
GET /api/recommendations/history/user-123?limit=20
```

**Response** (200 OK):
```json
[
  {
    "productId": "prod-001",
    "name": "Laptop Dell XPS 15",
    "category": "Electronics",
    "purchaseDate": "2024-02-10T15:30:00Z",
    "quantity": 1,
    "price": 1299.99
  },
  {
    "productId": "prod-002",
    "name": "Wireless Mouse",
    "category": "Accessories",
    "purchaseDate": "2024-02-15T10:20:00Z",
    "quantity": 2,
    "price": 49.99
  }
]
```

**Postman Test**:
```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("History is an array", function () {
    pm.expect(pm.response.json()).to.be.an('array');
});

pm.test("History items have required properties", function () {
    var history = pm.response.json();
    history.forEach(function (item) {
        pm.expect(item).to.have.property('productId');
        pm.expect(item).to.have.property('name');
        pm.expect(item).to.have.property('category');
        pm.expect(item).to.have.property('purchaseDate');
        pm.expect(item).to.have.property('quantity');
        pm.expect(item).to.have.property('price');
    });
});

pm.test("Quantity and price are positive", function () {
    var history = pm.response.json();
    history.forEach(function (item) {
        pm.expect(item.quantity).to.be.greaterThan(0);
        pm.expect(item.price).to.be.greaterThan(0);
    });
});
```

---

## POST Endpoints

### 5. Record Product View

**Endpoint**: `POST /api/recommendations/view`

**Description**: Record when a user views a product (for tracking engagement).

**Request Body**:
```json
{
  "userId": "user-123",
  "productId": "prod-001",
  "duration": 45,
  "source": "web"
}
```

**Parameters**:
- `userId` (required): User identifier
- `productId` (required): Product identifier  
- `duration` (optional): How long the user viewed (in seconds)
- `source` (optional): Source of the view - "web", "mobile", "email" (default: "web")

**Response** (204 No Content):
```
(No body)
```

**Postman Test**:
```javascript
pm.test("Status code is 204 No Content", function () {
    pm.response.to.have.status(204);
});

pm.test("Response has no body", function () {
    pm.expect(pm.response.text()).to.be.empty;
});
```

**Sample Requests** (copy these into the request body):

**Web View**:
```json
{
  "userId": "user-123",
  "productId": "prod-001",
  "duration": 120,
  "source": "web"
}
```

**Mobile View**:
```json
{
  "userId": "user-456",
  "productId": "prod-002",
  "duration": 45,
  "source": "mobile"
}
```

**Email Referral**:
```json
{
  "userId": "user-789",
  "productId": "prod-003",
  "duration": 30,
  "source": "email"
}
```

---

### 6. Record Purchase

**Endpoint**: `POST /api/recommendations/purchase`

**Description**: Record a purchase order for recommendations tracking.

**Request Body**:
```json
{
  "userId": "user-123",
  "orderId": "order-2024-001",
  "items": [
    {
      "productId": "prod-001",
      "quantity": 1,
      "price": 1299.99
    },
    {
      "productId": "prod-002",
      "quantity": 2,
      "price": 49.99
    }
  ]
}
```

**Parameters**:
- `userId` (required): User identifier
- `orderId` (required): Order/Transaction identifier
- `items` (required, array): List of purchased items
  - `productId`: Product identifier
  - `quantity`: Number of items purchased (must be > 0)
  - `price`: Unit price of the product (must be > 0)

**Response** (204 No Content):
```
(No body)
```

**Postman Test**:
```javascript
pm.test("Status code is 204 No Content", function () {
    pm.response.to.have.status(204);
});

pm.test("Request body is valid", function () {
    var body = JSON.parse(pm.request.body.raw);
    pm.expect(body).to.have.property('userId');
    pm.expect(body).to.have.property('orderId');
    pm.expect(body).to.have.property('items');
    pm.expect(body.items).to.be.an('array');
});
```

**Sample Purchase Orders**:

**Single Item Purchase**:
```json
{
  "userId": "user-123",
  "orderId": "order-2024-001",
  "items": [
    {
      "productId": "prod-001",
      "quantity": 1,
      "price": 1299.99
    }
  ]
}
```

**Multi-Item Purchase**:
```json
{
  "userId": "user-456",
  "orderId": "order-2024-002",
  "items": [
    {
      "productId": "prod-002",
      "quantity": 2,
      "price": 49.99
    },
    {
      "productId": "prod-003",
      "quantity": 1,
      "price": 29.99
    },
    {
      "productId": "prod-004",
      "quantity": 3,
      "price": 15.99
    }
  ]
}
```

**Bulk Purchase**:
```json
{
  "userId": "user-789",
  "orderId": "order-2024-003",
  "items": [
    {
      "productId": "prod-001",
      "quantity": 5,
      "price": 1299.99
    },
    {
      "productId": "prod-005",
      "quantity": 10,
      "price": 999.99
    }
  ]
}
```

---

## Test Scenarios

### Scenario 1: Complete User Journey

**Goal**: Test the complete flow from view to recommendation

**Steps**:
1. **Record Product Views**
   - POST `/api/recommendations/view` - user views product-001
   - POST `/api/recommendations/view` - user views product-002

2. **Record Purchase**
   - POST `/api/recommendations/purchase` - user purchases viewed products

3. **Get User History**
   - GET `/api/recommendations/history/user-123` - verify purchase is recorded

4. **Get Personalized Recommendations**
   - GET `/api/recommendations/user-123?limit=5` - get recommendations based on activity

---

### Scenario 2: Similar Products Discovery

**Goal**: Test product similarity features

**Steps**:
1. **Get Similar Products**
   - GET `/api/recommendations/similar/prod-001?limit=5`

2. **Verify Results**
   - Check that returned products are in the same category
   - Verify similarity scores are realistic

---

### Scenario 3: Trending Analysis

**Goal**: Test trending products endpoint

**Steps**:
1. **Record Multiple Purchases**
   - POST `/api/recommendations/purchase` - multiple times with different products
   - POST `/api/recommendations/purchase` - same product multiple times

2. **Get Trending Products**
   - GET `/api/recommendations/trending?days=7&limit=10`
   - GET `/api/recommendations/trending?days=30&limit=5`

3. **Verify Trending Calculation**
   - Verify frequently purchased products appear
   - Check trend scores make sense

---

## Error Cases

### Test 400 - Bad Request

**Invalid UserID (empty)**:
```http
GET /api/recommendations/?limit=10
```
Expected Response (400):
```json
{
  "message": "UserId cannot be empty"
}
```

**Invalid ProductID (empty)**:
```http
GET /api/recommendations/similar/?limit=5
```
Expected Response (400):
```json
{
  "message": "ProductId cannot be empty"
}
```

**Invalid Purchase Request (missing userId)**:
```json
{
  "orderId": "order-001",
  "items": [
    {
      "productId": "prod-001",
      "quantity": 1,
      "price": 99.99
    }
  ]
}
```
Expected Response (400):
```
ModelState validation error
```

**Invalid View Request (missing productId)**:
```json
{
  "userId": "user-123",
  "duration": 45
}
```
Expected Response (400):
```
ModelState validation error
```

---

### Test 500 - Internal Server Error

**Database Connection Error**:
```http
GET /api/recommendations/user-123
```
If Neo4j is not running, expected response (500):
```json
{
  "message": "Error retrieving recommendations"
}
```

---

## Quick Reference - cURL Commands

```bash
# Get personalized recommendations
curl -X GET "http://localhost:5000/api/recommendations/user-123?limit=10" \
  -H "Content-Type: application/json"

# Get similar products
curl -X GET "http://localhost:5000/api/recommendations/similar/prod-001?limit=5" \
  -H "Content-Type: application/json"

# Get trending products
curl -X GET "http://localhost:5000/api/recommendations/trending?days=7&limit=10" \
  -H "Content-Type: application/json"

# Get user history
curl -X GET "http://localhost:5000/api/recommendations/history/user-123?limit=20" \
  -H "Content-Type: application/json"

# Record product view
curl -X POST "http://localhost:5000/api/recommendations/view" \
  -H "Content-Type: application/json" \
  -d '{"userId":"user-123","productId":"prod-001","duration":120,"source":"web"}'

# Record purchase
curl -X POST "http://localhost:5000/api/recommendations/purchase" \
  -H "Content-Type: application/json" \
  -d '{"userId":"user-123","orderId":"order-001","items":[{"productId":"prod-001","quantity":1,"price":99.99}]}'
```

---

## Postman Environment Variables

**Recommended Variables to Set** (for reusability):

```javascript
{
  "baseUrl": "http://localhost:5000",
  "apiPath": "/api/recommendations",
  "userId": "user-123",
  "productId": "prod-001",
  "orderId": "order-{{$timestamp}}"
}
```

**Usage in Requests**:
```http
GET {{baseUrl}}{{apiPath}}/{{userId}}?limit=10
POST {{baseUrl}}{{apiPath}}/purchase
```

---

## Health Check

**Endpoint**: `GET /api/recommendations/health` (if implemented)

To check if the API is running:
```bash
curl http://localhost:5000/health
```

---

## Notes

- All timestamps are in ISO 8601 format (UTC)
- Prices are in decimal format (e.g., 99.99)
- The API uses Neo4j as the database
- Port 5000 is the default; change accordingly if different
- All arrays are sorted by relevance/score by default
- RabbitMQ is optional; the API will retry connecting but won't block if unavailable
