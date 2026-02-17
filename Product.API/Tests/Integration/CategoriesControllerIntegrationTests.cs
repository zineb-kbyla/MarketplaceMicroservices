using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Product.API.Application.DTOs;
using Xunit;

namespace Product.API.Tests.Integration
{
    public class CategoriesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public CategoriesControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
        public async Task GetCategories_ReturnsListOfCategories()
        {
            // Act
            var response = await _client.GetAsync("/api/categories");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>(_jsonOptions);
            Assert.NotNull(categories);
            Assert.IsType<List<CategoryDto>>(categories);
        }

        [Fact]
        public async Task CreateCategory_ReturnsCreatedCategory()
        {
            // Arrange
            var newCategory = new CreateCategoryDto
            {
                Name = "Integration Test Category",
                Description = "A category created during integration testing",
                ImageUrl = "https://example.com/test-category.jpg"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/categories", newCategory);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var createdCategory = await response.Content.ReadFromJsonAsync<CategoryDto>(_jsonOptions);
            Assert.NotNull(createdCategory);
            Assert.Equal(newCategory.Name, createdCategory.Name);
            Assert.Equal(newCategory.Description, createdCategory.Description);
            Assert.Equal(newCategory.ImageUrl, createdCategory.ImageUrl);
            Assert.NotNull(createdCategory.Id);

            // Cleanup
            await _client.DeleteAsync($"/api/categories/{createdCategory.Id}");
        }

        [Fact]
        public async Task GetCategoryById_ReturnsCategory_WhenCategoryExists()
        {
            // Arrange - First create a category
            var newCategory = new CreateCategoryDto
            {
                Name = "Test Category For Get",
                Description = "Testing GetById",
                ImageUrl = "https://example.com/test.jpg"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/categories", newCategory);
            var createdCategory = await createResponse.Content.ReadFromJsonAsync<CategoryDto>(_jsonOptions);

            // Act
            var response = await _client.GetAsync($"/api/categories/{createdCategory.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var category = await response.Content.ReadFromJsonAsync<CategoryDto>(_jsonOptions);
            Assert.NotNull(category);
            Assert.Equal(createdCategory.Id, category.Id);
            Assert.Equal(newCategory.Name, category.Name);

            // Cleanup
            await _client.DeleteAsync($"/api/categories/{createdCategory.Id}");
        }

        [Fact]
        public async Task GetCategoryById_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var nonExistentId = "000000000000000000000000";

            // Act
            var response = await _client.GetAsync($"/api/categories/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCategory_UpdatesSuccessfully()
        {
            // Arrange - Create a category first
            var newCategory = new CreateCategoryDto
            {
                Name = "Category To Update",
                Description = "Original description",
                ImageUrl = "https://example.com/original.jpg"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/categories", newCategory);
            var createdCategory = await createResponse.Content.ReadFromJsonAsync<CategoryDto>(_jsonOptions);

            var updateDto = new CreateCategoryDto
            {
                Name = "Updated Category Name",
                Description = "Updated description",
                ImageUrl = "https://example.com/updated.jpg"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/categories/{createdCategory.Id}", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the update
            var getResponse = await _client.GetAsync($"/api/categories/{createdCategory.Id}");
            var updatedCategory = await getResponse.Content.ReadFromJsonAsync<CategoryDto>(_jsonOptions);

            Assert.Equal(updateDto.Name, updatedCategory.Name);
            Assert.Equal(updateDto.Description, updatedCategory.Description);
            Assert.Equal(updateDto.ImageUrl, updatedCategory.ImageUrl);

            // Cleanup
            await _client.DeleteAsync($"/api/categories/{createdCategory.Id}");
        }

        [Fact]
        public async Task UpdateCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var nonExistentId = "000000000000000000000000";
            var updateDto = new CreateCategoryDto
            {
                Name = "Updated Name",
                Description = "Updated description",
                ImageUrl = "https://example.com/image.jpg"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/categories/{nonExistentId}", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCategory_DeletesSuccessfully()
        {
            // Arrange - Create a category to delete
            var newCategory = new CreateCategoryDto
            {
                Name = "Category To Delete",
                Description = "This category will be deleted",
                ImageUrl = "https://example.com/delete.jpg"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/categories", newCategory);
            var createdCategory = await createResponse.Content.ReadFromJsonAsync<CategoryDto>(_jsonOptions);

            // Act
            var response = await _client.DeleteAsync($"/api/categories/{createdCategory.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify deletion
            var getResponse = await _client.GetAsync($"/api/categories/{createdCategory.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var nonExistentId = "000000000000000000000000";

            // Act
            var response = await _client.DeleteAsync($"/api/categories/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
