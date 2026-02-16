using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Recommendation.API.Application.DTOs;
using Recommendation.API.Application.Interfaces;
using Recommendation.API.Infrastructure.Data;
using Xunit;

namespace Recommendation.API.Tests.Integration
{
    public class RecommendationsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public RecommendationsControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        [Fact]
        public async Task GetPersonalizedRecommendations_ReturnsRecommendations()
        {
            // Arrange
            var userId = "user-integration-test-1";
            
            // Act
            var response = await _client.GetAsync($"/api/recommendations/{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var recommendations = await response.Content.ReadFromJsonAsync<List<RecommendedProductDto>>(_jsonOptions);
            Assert.NotNull(recommendations);
            Assert.IsType<List<RecommendedProductDto>>(recommendations);
        }

        [Fact]
        public async Task GetPersonalizedRecommendations_WithLimit_ReturnsLimitedRecommendations()
        {
            // Arrange
            var userId = "user-integration-test-2";
            var limit = 5;

            // Act
            var response = await _client.GetAsync($"/api/recommendations/{userId}?limit={limit}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var recommendations = await response.Content.ReadFromJsonAsync<List<RecommendedProductDto>>(_jsonOptions);
            Assert.NotNull(recommendations);
            Assert.True(recommendations.Count <= limit);
        }

        [Fact]
        public async Task GetPersonalizedRecommendations_WithEmptyUserId_ReturnsBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/recommendations/");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetSimilarProducts_ReturnsSimilarProducts()
        {
            // Arrange
            var productId = "product-test-1";

            // Act
            var response = await _client.GetAsync($"/api/recommendations/similar/{productId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var similar = await response.Content.ReadFromJsonAsync<List<SimilarProductDto>>(_jsonOptions);
            Assert.NotNull(similar);
            Assert.IsType<List<SimilarProductDto>>(similar);
        }

        [Fact]
        public async Task GetSimilarProducts_WithLimit_ReturnsLimitedProducts()
        {
            // Arrange
            var productId = "product-test-2";
            var limit = 3;

            // Act
            var response = await _client.GetAsync($"/api/recommendations/similar/{productId}?limit={limit}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var similar = await response.Content.ReadFromJsonAsync<List<SimilarProductDto>>(_jsonOptions);
            Assert.NotNull(similar);
            Assert.True(similar.Count <= limit);
        }

        [Fact]
        public async Task GetTrendingProducts_ReturnsTrendingProducts()
        {
            // Act
            var response = await _client.GetAsync("/api/recommendations/trending");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var trending = await response.Content.ReadFromJsonAsync<List<TrendingProductDto>>(_jsonOptions);
            Assert.NotNull(trending);
            Assert.IsType<List<TrendingProductDto>>(trending);
        }

        [Fact]
        public async Task GetTrendingProducts_WithDaysAndLimit_ReturnsFilteredTrending()
        {
            // Arrange
            var days = 14;
            var limit = 5;

            // Act
            var response = await _client.GetAsync($"/api/recommendations/trending?days={days}&limit={limit}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var trending = await response.Content.ReadFromJsonAsync<List<TrendingProductDto>>(_jsonOptions);
            Assert.NotNull(trending);
            Assert.True(trending.Count <= limit);
        }

        [Fact]
        public async Task GetUserHistory_ReturnsUserHistory()
        {
            // Arrange
            var userId = "user-integration-test-3";

            // Act
            var response = await _client.GetAsync($"/api/recommendations/history/{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var history = await response.Content.ReadFromJsonAsync<List<UserHistoryDto>>(_jsonOptions);
            Assert.NotNull(history);
            Assert.IsType<List<UserHistoryDto>>(history);
        }

        [Fact]
        public async Task GetUserHistory_WithLimit_ReturnsLimitedHistory()
        {
            // Arrange
            var userId = "user-integration-test-4";
            var limit = 10;

            // Act
            var response = await _client.GetAsync($"/api/recommendations/history/{userId}?limit={limit}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var history = await response.Content.ReadFromJsonAsync<List<UserHistoryDto>>(_jsonOptions);
            Assert.NotNull(history);
            Assert.True(history.Count <= limit);
        }

        [Fact]
        public async Task RecordProductView_ReturnsNoContent()
        {
            // Arrange
            var viewDto = new RecordViewDto
            {
                UserId = "user-view-test",
                ProductId = "product-view-test",
                Duration = 45,
                Source = "web"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/recommendations/view", viewDto);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task RecordProductView_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var viewDto = new RecordViewDto
            {
                UserId = string.Empty,
                ProductId = "product-test",
                Duration = 30,
                Source = "web"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/recommendations/view", viewDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RecordPurchase_ReturnsNoContent()
        {
            // Arrange
            var purchaseDto = new RecordPurchaseDto
            {
                UserId = "user-purchase-test",
                OrderId = "order-purchase-test-1",
                Items = new List<PurchaseItemDto>
                {
                    new PurchaseItemDto
                    {
                        ProductId = "product-purchase-1",
                        Quantity = 2,
                        Price = 99.99m
                    }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/recommendations/purchase", purchaseDto);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task RecordPurchase_WithMultipleItems_ReturnsNoContent()
        {
            // Arrange
            var purchaseDto = new RecordPurchaseDto
            {
                UserId = "user-purchase-test-2",
                OrderId = "order-purchase-test-2",
                Items = new List<PurchaseItemDto>
                {
                    new PurchaseItemDto
                    {
                        ProductId = "product-purchase-2",
                        Quantity = 1,
                        Price = 49.99m
                    },
                    new PurchaseItemDto
                    {
                        ProductId = "product-purchase-3",
                        Quantity = 3,
                        Price = 29.99m
                    }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/recommendations/purchase", purchaseDto);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task RecordPurchase_WithEmptyItems_ReturnsBadRequest()
        {
            // Arrange
            var purchaseDto = new RecordPurchaseDto
            {
                UserId = "user-purchase-test-3",
                OrderId = "order-purchase-test-3",
                Items = new List<PurchaseItemDto>()
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/recommendations/purchase", purchaseDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RecordPurchase_WithInvalidPrice_ReturnsBadRequest()
        {
            // Arrange
            var purchaseDto = new RecordPurchaseDto
            {
                UserId = "user-purchase-test-4",
                OrderId = "order-purchase-test-4",
                Items = new List<PurchaseItemDto>
                {
                    new PurchaseItemDto
                    {
                        ProductId = "product-purchase-4",
                        Quantity = 1,
                        Price = -10m // Invalid negative price
                    }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/recommendations/purchase", purchaseDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RecordPurchase_WithInvalidQuantity_ReturnsBadRequest()
        {
            // Arrange
            var purchaseDto = new RecordPurchaseDto
            {
                UserId = "user-purchase-test-5",
                OrderId = "order-purchase-test-5",
                Items = new List<PurchaseItemDto>
                {
                    new PurchaseItemDto
                    {
                        ProductId = "product-purchase-5",
                        Quantity = 0, // Invalid zero quantity
                        Price = 99.99m
                    }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/recommendations/purchase", purchaseDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
