# Postman Tests - Product Service

This file lists ready-to-use Postman requests for the Product API.

## Base URL (local)

- http://localhost:5007
- https://localhost:7243

## Common Headers

- Content-Type: application/json

## Products

## Liste complete (comme demande)

Categories

- GET /api/categories
- POST /api/categories
- GET /api/categories/{id}
- PUT /api/categories/{id}
- DELETE /api/categories/{id}

Products

- GET /api/products
- POST /api/products
- GET /api/products/{id}
- PUT /api/products/{id}
- DELETE /api/products/{id}
- GET /api/products/category/{category}
- GET /api/products/search
- POST /api/products/{id}/decrement-stock

## Tests Postman reels (exemples)

Astuce: remplace {id} par un vrai id et {category} par une categorie existante.

### Get all products

GET `/api/products`

Exemple URL:

- http://localhost:5007/api/products

### Get product by id

GET `/api/products/{id}`

Exemple URL:

- http://localhost:5007/api/products/607f1f77bcf86cd799439011

### Get products by category

GET `/api/products/category/{category}`

Exemple URL:

- http://localhost:5007/api/products/category/Electronics

### Search products

GET `/api/products/search?q=iphone`

Exemple URL:

- http://localhost:5007/api/products/search?q=iphone

### Create product

POST `/api/products`

Exemple URL:

- http://localhost:5007/api/products

```json
{
  "name": "iPhone 15 Pro",
  "description": "Latest flagship smartphone",
  "category": "Electronics",
  "price": 999.99,
  "stock": 50,
  "imageUrl": "https://example.com/iphone.jpg"
}
```

### Update product

PUT `/api/products/{id}`

Exemple URL:

- http://localhost:5007/api/products/607f1f77bcf86cd799439011

```json
{
  "name": "iPhone 15 Pro Max",
  "description": "Updated description",
  "category": "Electronics",
  "price": 1099.99,
  "stock": 45,
  "imageUrl": "https://example.com/iphone-pro-max.jpg"
}
```

### Delete product

DELETE `/api/products/{id}`

Exemple URL:

- http://localhost:5007/api/products/607f1f77bcf86cd799439011

### Decrement stock

POST `/api/products/{id}/decrement-stock`

Exemple URL:

- http://localhost:5007/api/products/607f1f77bcf86cd799439011/decrement-stock

```json
{
  "quantity": 5
}
```

## Categories

### Get all categories

GET `/api/categories`

Exemple URL:

- http://localhost:5007/api/categories

### Get category by id

GET `/api/categories/{id}`

Exemple URL:

- http://localhost:5007/api/categories/607f1f77bcf86cd799439012

### Create category

POST `/api/categories`

Exemple URL:

- http://localhost:5007/api/categories

```json
{
  "name": "Electronics",
  "description": "Phones, laptops, accessories",
  "imageUrl": "https://example.com/electronics.jpg"
}
```

### Update category

PUT `/api/categories/{id}`

Exemple URL:

- http://localhost:5007/api/categories/607f1f77bcf86cd799439012

```json
{
  "name": "Electronics",
  "description": "Updated category description",
  "imageUrl": "https://example.com/electronics-v2.jpg"
}
```

### Delete category

DELETE `/api/categories/{id}`

Exemple URL:

- http://localhost:5007/api/categories/607f1f77bcf86cd799439012
