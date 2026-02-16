using Moq;
using Recommendation.API.Application.Algorithms;
using Recommendation.API.Application.Interfaces;
using Xunit;
using Microsoft.Extensions.Logging;

namespace Recommendation.API.Tests.Unit
{
    public class AlgorithmTests
    {
        [Fact]
        public void CollaborativeFiltering_CalculateUserSimilarity_WithCommonProducts()
        {
            // Arrange
            var repositoryMock = new Mock<IRecommendationRepository>();
            var loggerMock = new Mock<ILogger<CollaborativeFilteringAlgorithm>>();

            var algorithm = new CollaborativeFilteringAlgorithm(repositoryMock.Object, loggerMock.Object);

            var user1Purchases = new Dictionary<string, int> { { "prod-1", 1 }, { "prod-2", 1 }, { "prod-3", 1 } };
            var user2Purchases = new Dictionary<string, int> { { "prod-1", 1 }, { "prod-2", 1 }, { "prod-4", 1 } };

            // Act
            var score = algorithm.CalculateUserSimilarity(user1Purchases, user2Purchases);

            // Assert
            Assert.True(score >= 0 && score <= 1);
        }

        [Fact]
        public void CollaborativeFiltering_CalculateUserSimilarity_NoCommonProducts()
        {
            // Arrange
            var repositoryMock = new Mock<IRecommendationRepository>();
            var loggerMock = new Mock<ILogger<CollaborativeFilteringAlgorithm>>();

            var algorithm = new CollaborativeFilteringAlgorithm(repositoryMock.Object, loggerMock.Object);

            var user1 = new Dictionary<string, int> { { "prod-1", 1 } };
            var user2 = new Dictionary<string, int> { { "prod-2", 1 } };

            // Act
            var score = algorithm.CalculateUserSimilarity(user1, user2);

            // Assert
            Assert.Equal(0, score);
        }

        [Fact]
        public void ContentBased_CalculateProductSimilarity_SameCategory()
        {
            // Arrange
            var repositoryMock = new Mock<IRecommendationRepository>();
            var loggerMock = new Mock<ILogger<ContentBasedFilteringAlgorithm>>();

            var algorithm = new ContentBasedFilteringAlgorithm(repositoryMock.Object, loggerMock.Object);

            // Act
            var score = algorithm.CalculateProductSimilarity("Electronics", "Electronics", 100, 120);

            // Assert
            Assert.True(score > 0);
        }

        [Fact]
        public void ContentBased_CalculateProductSimilarity_DifferentCategory()
        {
            // Arrange
            var repositoryMock = new Mock<IRecommendationRepository>();
            var loggerMock = new Mock<ILogger<ContentBasedFilteringAlgorithm>>();

            var algorithm = new ContentBasedFilteringAlgorithm(repositoryMock.Object, loggerMock.Object);

            // Act
            var score = algorithm.CalculateProductSimilarity("Electronics", "Books", 100, 20);

            // Assert
            Assert.Equal(0, score);
        }
    }
}

