using Moq;
using Recommendation.API.Application.Algorithms;
using Recommendation.API.Application.DTOs;
using Recommendation.API.Application.Interfaces;
using Xunit;

namespace Recommendation.API.Tests.Unit
{
    public class CollaborativeFilteringTests
    {
        private readonly Mock<IRecommendationRepository> _repositoryMock;
        private readonly Mock<ILogger<CollaborativeFilteringAlgorithm>> _loggerMock;
        private readonly CollaborativeFilteringAlgorithm _algorithm;

        public CollaborativeFilteringTests()
        {
            _repositoryMock = new Mock<IRecommendationRepository>();
            _loggerMock = new Mock<ILogger<CollaborativeFilteringAlgorithm>>();
            _algorithm = new CollaborativeFilteringAlgorithm(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetSimilarUsersAsync_ReturnsSimilarUsers()
        {
            // Arrange
            var userId = "user-1";
            var similarUsers = new List<string> { "user-2", "user-3", "user-4" };

            _repositoryMock.Setup(r => r.GetSimilarUsersAsync(userId, It.IsAny<int>()))
                .ReturnsAsync(similarUsers);

            // Act
            var result = await _algorithm.GetSimilarUsersAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains("user-2", result);
        }

        [Fact]
        public async Task GetRecommendationsAsync_WithNoHistory_ReturnsPopularProducts()
        {
            // Arrange
            var userId = "new-user";
            _repositoryMock.Setup(r => r.GetUserPurchaseHistoryAsync(userId))
                .ReturnsAsync(new Dictionary<string, int>());

            var trendingProducts = new List<TrendingProductDto>
            {
                new TrendingProductDto
                {
                    ProductId = "trending-1",
                    Name = "Popular Product",
                    RecentPurchases = 100,
                    TrendScore = 0.9
                }
            };

            _repositoryMock.Setup(r => r.GetTrendingProductsAsync(30, It.IsAny<int>()))
                .ReturnsAsync(trendingProducts);

            // Act
            var result = await _algorithm.GetRecommendationsAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result); // When no history, returns trending products
        }

        [Fact]
        public void CalculateUserSimilarity_WithCommonProducts_ReturnsScore()
        {
            // Arrange
            var user1Purchases = new Dictionary<string, int>
            {
                { "prod-1", 1 },
                { "prod-2", 1 },
                { "prod-3", 1 }
            };

            var user2Purchases = new Dictionary<string, int>
            {
                { "prod-1", 1 },
                { "prod-2", 1 },
                { "prod-4", 1 }
            };

            // Act
            var score = _algorithm.CalculateUserSimilarity(user1Purchases, user2Purchases);

            // Assert
            Assert.True(score > 0);
            Assert.True(score <= 1.0);
            Assert.Equal(0.5, score); // 2 common / 4 total unique = 0.5
        }

        [Fact]
        public void CalculateUserSimilarity_WithNoCommonProducts_ReturnsZero()
        {
            // Arrange
            var user1Purchases = new Dictionary<string, int>
            {
                { "prod-1", 1 },
                { "prod-2", 1 }
            };

            var user2Purchases = new Dictionary<string, int>
            {
                { "prod-3", 1 },
                { "prod-4", 1 }
            };

            // Act
            var score = _algorithm.CalculateUserSimilarity(user1Purchases, user2Purchases);

            // Assert
            Assert.Equal(0, score);
        }

        [Fact]
        public void CalculateUserSimilarity_WithEmptyHistory_ReturnsZero()
        {
            // Arrange
            var emptyPurchases = new Dictionary<string, int>();
            var userPurchases = new Dictionary<string, int>
            {
                { "prod-1", 1 }
            };

            // Act
            var score = _algorithm.CalculateUserSimilarity(emptyPurchases, userPurchases);

            // Assert
            Assert.Equal(0, score);
        }
    }
}
