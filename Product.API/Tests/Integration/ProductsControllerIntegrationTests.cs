using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Product.API.Application.DTOs;
using Xunit;

namespace Product.API.Tests.Integration
{
    public class ProductsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductsControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            
            // Configure JSON serialization options to handle enums as strings
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        [Fact]
        public async Task GetProducts_ReturnsListOfProducts()
        {
            // Act
            var response = await _client.GetAsync("/api/products");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>(_jsonOptions);
            Assert.NotNull(products);
            Assert.IsType<List<ProductDto>>(products);
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedProduct()
        {
            // Arrange
            var newProduct = new CreateProductDto
            {
                Name = "Integration Test Product",
                Description = "A product created during integration testing",
                Category = "Electronics",
                Price = 299.99m,
                Stock = 50,
                ImageUrl = "https://example.com/test-product.jpg"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/products", newProduct);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>(_jsonOptions);
            Assert.NotNull(createdProduct);
            Assert.Equal(newProduct.Name, createdProduct.Name);
            Assert.Equal(newProduct.Description, createdProduct.Description);
            Assert.Equal(newProduct.Category, createdProduct.Category);
            Assert.Equal(newProduct.Price, createdProduct.Price);
            Assert.Equal(newProduct.Stock, createdProduct.Stock);
            Assert.NotNull(createdProduct.Id);

            // Cleanup
            await _client.DeleteAsync($"/api/products/{createdProduct.Id}");
        }

        [Fact]
        public async Task GetProductById_ReturnsProduct_WhenProductExists()
        {
            // Arrange - First create a product
            var newProduct = new CreateProductDto
            {
                Name = "Test Product For Get",
                Description = "Testing GetById",
                Category = "TestCategory",
                Price = 99.99m,
                Stock = 10,
                ImageUrl = "https://example.com/test.jpg"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/products", newProduct);
            var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>(_jsonOptions);

            // Act
            var response = await _client.GetAsync($"/api/products/{createdProduct.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var product = await response.Content.ReadFromJsonAsync<ProductDto>(_jsonOptions);
            Assert.NotNull(product);
            Assert.Equal(createdProduct.Id, product.Id);
            Assert.Equal(newProduct.Name, product.Name);

            // Cleanup
            await _client.DeleteAsync($"/api/products/{createdProduct.Id}");
        }

        [Fact]
        public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var nonExistentId = "000000000000000000000000";

            // Act
            var response = await _client.GetAsync($"/api/products/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetProductsByCategory_ReturnsProductsInCategory()
        {
            // Arrange - Create products in a specific category
            var category = "IntegrationTestCategory";
            var product1 = new CreateProductDto
            {
                Name = "Category Test Product 1",
                Description = "First product in test category",
                Category = category,
                Price = 50.00m,
                Stock = 5,
                ImageUrl = "https://example.com/test1.jpg"
            };
            var product2 = new CreateProductDto
            {
                Name = "Category Test Product 2",
                Description = "Second product in test category",
                Category = category,
                Price = 75.00m,
                Stock = 10,
                ImageUrl = "https://example.com/test2.jpg"
            };

            var createResponse1 = await _client.PostAsJsonAsync("/api/products", product1);
            var createResponse2 = await _client.PostAsJsonAsync("/api/products", product2);
            var createdProduct1 = await createResponse1.Content.ReadFromJsonAsync<ProductDto>(_jsonOptions);
            var createdProduct2 = await createResponse2.Content.ReadFromJsonAsync<ProductDto>(_jsonOptions);

            // Act
            var response = await _client.GetAsync($"/api/products/category/{category}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>(_jsonOptions);
            Assert.NotNull(products);
            Assert.True(products.Count >= 2);
            Assert.Contains(products, p => p.Id == createdProduct1.Id);
            Assert.Contains(products, p => p.Id == createdProduct2.Id);

            // Cleanup
            await _client.DeleteAsync($"/api/products/{createdProduct1.Id}");
            await _client.DeleteAsync($"/api/products/{createdProduct2.Id}");
        }

        [Fact]
        public async Task SearchProducts_ReturnsMatchingProducts()
        {
            // Arrange - Create a product with a unique name
            var uniqueName = $"UniqueSearchableProduct{Guid.NewGuid().ToString().Substring(0, 8)}";
            var newProduct = new CreateProductDto
            {
                Name = uniqueName,
                Description = "Testing search functionality",
                Category = "SearchTest",
                Price = 123.45m,
                Stock = 3,
                ImageUrl = "https://example.com/search.jpg"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/products", newProduct);
            var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>(_jsonOptions);

            // Act
            var response = await _client.GetAsync($"/api/products/search?q={uniqueName}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>(_jsonOptions);
            Assert.NotNull(products);
            Assert.Contains(products, p => p.Id == createdProduct.Id);

            // Cleanup
            await _client.DeleteAsync($"/api/products/{createdProduct.Id}");
        }

        [Fact]
        public async Task SearchProducts_ReturnsBadRequest_WhenQueryIsEmpty()
        {
            // Act
            var response = await _client.GetAsync("/api/products/search?q=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateProduct_UpdatesSuccessfully()
        {
            // Arrange - Create a product first
            var newProduct = new CreateProductDto
            {
                Name = "Product To Update",
                Description = "Original description",
                Category = "UpdateTest",
                Price = 100.00m,
                Stock = 20,
                ImageUrl = "https://example.com/original.jpg"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/products", newProduct);
            var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>(_jsonOptions);

            var updateDto = new UpdateProductDto
            {
                Name = "Updated Product Name",
                Description = "Updated description",
                Category = "UpdatedCategory",
                Price = 150.00m,
                Stock = 30,
                ImageUrl = "https://example.com/updated.jpg"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/products/{createdProduct.Id}", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the update
            var getResponse = await _client.GetAsync($"/api/products/{createdProduct.Id}");
            var updatedProduct = await getResponse.Content.ReadFromJsonAsync<ProductDto>(_jsonOptions);

            Assert.Equal(updateDto.Name, updatedProduct.Name);
            Assert.Equal(updateDto.Description, updatedProduct.Description);
            Assert.Equal(updateDto.Price, updatedProduct.Price);

            // Cleanup
            await _client.DeleteAsync($"/api/products/{createdProduct.Id}");
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var nonExistentId = "000000000000000000000000";
            var updateDto = new UpdateProductDto
            {
                Name = "Updated Name",
                Description = "Updated description",
                Category = "Category",
                Price = 100.00m,
                Stock = 10,
                ImageUrl = "https://example.com/image.jpg"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/products/{nonExistentId}", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_DeletesSuccessfully()
        {
            // Arrange - Create a product to delete
            var newProduct = new CreateProductDto
            {
                Name = "Product To Delete",
                Description = "This product will be deleted",
                Category = "DeleteTest",
                Price = 50.00m,
                Stock = 5,
                ImageUrl = "https://example.com/delete.jpg"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/products", newProduct);
            var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>(_jsonOptions);

            // Act
            var response = await _client.DeleteAsync($"/api/products/{createdProduct.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify deletion
            var getResponse = await _client.GetAsync($"/api/products/{createdProduct.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var nonExistentId = "000000000000000000000000";

            // Act
            var response = await _client.DeleteAsync($"/api/products/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DecrementStock_DecrementsSuccessfully()
        {
            // Arrange - Create a product with stock
            var newProduct = new CreateProductDto
            {
                Name = "Product With Stock",
                Description = "Testing stock decrement",
                Category = "StockTest",
                Price = 25.00m,
                Stock = 100,
                ImageUrl = "https://example.com/stock.jpg"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/products", newProduct);
            var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>(_jsonOptions);

            var decrementDto = new DecrementStockDto { Quantity = 10 };

            // Act
            var response = await _client.PostAsJsonAsync($"/api/products/{createdProduct.Id}/decrement-stock", decrementDto);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify stock was decremented
            var getResponse = await _client.GetAsync($"/api/products/{createdProduct.Id}");
            var updatedProduct = await getResponse.Content.ReadFromJsonAsync<ProductDto>(_jsonOptions);

            Assert.Equal(90, updatedProduct.Stock);

            // Cleanup
            await _client.DeleteAsync($"/api/products/{createdProduct.Id}");
        }

        [Fact]
        public async Task DecrementStock_ReturnsBadRequest_WhenQuantityIsZeroOrNegative()
        {
            // Arrange
            var newProduct = new CreateProductDto
            {
                Name = "Stock Test Product",
                Description = "Testing invalid quantity",
                Category = "StockTest",
                Price = 30.00m,
                Stock = 50,
                ImageUrl = "https://example.com/stock.jpg"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/products", newProduct);
            var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>(_jsonOptions);

            var decrementDto = new DecrementStockDto { Quantity = 0 };

            // Act
            var response = await _client.PostAsJsonAsync($"/api/products/{createdProduct.Id}/decrement-stock", decrementDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // Cleanup
            await _client.DeleteAsync($"/api/products/{createdProduct.Id}");
        }

        [Fact]
        public async Task DecrementStock_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var nonExistentId = "000000000000000000000000";
            var decrementDto = new DecrementStockDto { Quantity = 5 };

            // Act
            var response = await _client.PostAsJsonAsync($"/api/products/{nonExistentId}/decrement-stock", decrementDto);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
