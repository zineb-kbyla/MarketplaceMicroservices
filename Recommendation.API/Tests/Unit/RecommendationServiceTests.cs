using Moq;
using Recommendation.API.Application.Algorithms;
using Recommendation.API.Application.DTOs;
using Recommendation.API.Application.Interfaces;
using Recommendation.API.Application.Services;
using Xunit;

namespace Recommendation.API.Tests.Unit
{
    public class RecommendationServiceTests
    {
        private readonly Mock<IRecommendationRepository> _repositoryMock;
        private readonly Mock<ILogger<CollaborativeFilteringAlgorithm>> _collaborativeLoggerMock;
        private readonly Mock<ILogger<ContentBasedFilteringAlgorithm>> _contentLoggerMock;
        private readonly Mock<ILogger<RecommendationService>> _serviceLoggerMock;
        private readonly RecommendationService _service;

        public RecommendationServiceTests()
        {
            _repositoryMock = new Mock<IRecommendationRepository>();
            _collaborativeLoggerMock = new Mock<ILogger<CollaborativeFilteringAlgorithm>>();
            _contentLoggerMock = new Mock<ILogger<ContentBasedFilteringAlgorithm>>();
            _serviceLoggerMock = new Mock<ILogger<RecommendationService>>();

            var collaborativeFilter = new CollaborativeFilteringAlgorithm(_repositoryMock.Object, _collaborativeLoggerMock.Object);
            var contentFilter = new ContentBasedFilteringAlgorithm(_repositoryMock.Object, _contentLoggerMock.Object);

            _service = new RecommendationService(
                _repositoryMock.Object,
                collaborativeFilter,
                contentFilter,
                _serviceLoggerMock.Object
            );
        }

        [Fact]
        public async Task GetPersonalizedRecommendationsAsync_ReturnsRecommendations()
        {
            // Arrange
            var userId = "test-user";
            var recommendations = new List<RecommendedProductDto>
            {
                new RecommendedProductDto
                {
                    ProductId = "prod1",
                    Name = "Product 1",
                    Category = "Electronics",
                    Price = 99.99m,
                    Score = 0.9m,
                    Confidence = 0.8
                }
            };

            _repositoryMock.Setup(r => r.UpdateUserLastActiveAsync(userId))
                .Returns(Task.CompletedTask);
            
            // Mock all the algorithm dependencies
            _repositoryMock.Setup(r => r.GetUserPurchaseHistoryAsync(userId))
                .ReturnsAsync(new Dictionary<string, int> { { "prod1", 1 } });
            _repositoryMock.Setup(r => r.GetSimilarUsersAsync(userId, It.IsAny<int>()))
                .ReturnsAsync(new List<string> { "user2" });
            _repositoryMock.Setup(r => r.GetRecommendedProductIdsAsync(userId, It.IsAny<List<string>>(), It.IsAny<int>()))
                .ReturnsAsync(new List<string> { "prod1" });
            _repositoryMock.Setup(r => r.GetPersonalizedRecommendationsAsync(userId, It.IsAny<int>()))
                .ReturnsAsync(recommendations);

            // Act
            var result = await _service.GetPersonalizedRecommendationsAsync(userId);

            // Assert
            Assert.NotNull(result);
            _repositoryMock.Verify(r => r.UpdateUserLastActiveAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetSimilarProductsAsync_ReturnsSimilarProducts()
        {
            // Arrange
            var productId = "product-1";
            var similarProducts = new List<SimilarProductDto>
            {
                new SimilarProductDto
                {
                    ProductId = "product-2",
                    Name = "Similar Product",
                    Category = "Electronics",
                    Price = 89.99m,
                    SimilarityScore = 0.85
                }
            };

            _repositoryMock.Setup(r => r.GetSimilarProductsAsync(productId, It.IsAny<int>()))
                .ReturnsAsync(similarProducts);

            // Act
            var result = await _service.GetSimilarProductsAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("product-2", result[0].ProductId);
        }

        [Fact]
        public async Task GetTrendingProductsAsync_ReturnsTrendingProducts()
        {
            // Arrange
            var trendingProducts = new List<TrendingProductDto>
            {
                new TrendingProductDto
                {
                    ProductId = "trend-1",
                    Name = "Trending Product",
                    Category = "Electronics",
                    Price = 199.99m,
                    RecentPurchases = 150,
                    TrendScore = 0.95
                }
            };

            _repositoryMock.Setup(r => r.GetTrendingProductsAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(trendingProducts);

            // Act
            var result = await _service.GetTrendingProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("trend-1", result[0].ProductId);
        }

        [Fact]
        public async Task RecordPurchaseAsync_CalledSuccessfully()
        {
            // Arrange
            var userId = "test-user";
            var orderId = "order-1";
            var items = new List<PurchaseItemDto>
            {
                new PurchaseItemDto
                {
                    ProductId = "prod-1",
                    Quantity = 2,
                    Price = 50m
                }
            };

            _repositoryMock.Setup(r => r.RecordPurchaseAsync(userId, orderId, items))
                .Returns(Task.CompletedTask);

            // Act
            await _service.RecordPurchaseAsync(userId, orderId, items);

            // Assert
            _repositoryMock.Verify(r => r.RecordPurchaseAsync(userId, orderId, items), Times.Once);
        }

        [Fact]
        public async Task RecordViewAsync_CalledSuccessfully()
        {
            // Arrange
            var userId = "test-user";
            var productId = "prod-1";
            var duration = 45;
            var source = "web";

            _repositoryMock.Setup(r => r.RecordViewAsync(userId, productId, duration, source))
                .Returns(Task.CompletedTask);

            // Act
            await _service.RecordViewAsync(userId, productId, duration, source);

            // Assert
            _repositoryMock.Verify(r => r.RecordViewAsync(userId, productId, duration, source), Times.Once);
        }

        [Fact]
        public async Task GetUserHistoryAsync_ReturnsHistory()
        {
            // Arrange
            var userId = "test-user";
            var history = new List<UserHistoryDto>
            {
                new UserHistoryDto
                {
                    ProductId = "prod-1",
                    Name = "Product 1",
                    Category = "Electronics",
                    Quantity = 1,
                    Price = 99.99m
                }
            };

            _repositoryMock.Setup(r => r.GetUserHistoryAsync(userId, It.IsAny<int>()))
                .ReturnsAsync(history);

            // Act
            var result = await _service.GetUserHistoryAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("prod-1", result[0].ProductId);
        }
    }
}
