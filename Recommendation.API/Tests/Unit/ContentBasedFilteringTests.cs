using Moq;
using Recommendation.API.Application.Algorithms;
using Recommendation.API.Application.Interfaces;
using Xunit;

namespace Recommendation.API.Tests.Unit
{
    public class ContentBasedFilteringTests
    {
        private readonly Mock<IRecommendationRepository> _repositoryMock;
        private readonly Mock<ILogger<ContentBasedFilteringAlgorithm>> _loggerMock;
        private readonly ContentBasedFilteringAlgorithm _algorithm;

        public ContentBasedFilteringTests()
        {
            _repositoryMock = new Mock<IRecommendationRepository>();
            _loggerMock = new Mock<ILogger<ContentBasedFilteringAlgorithm>>();
            _algorithm = new ContentBasedFilteringAlgorithm(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void CalculateProductSimilarity_SameCategory_ReturnsHighScore()
        {
            // Arrange
            var category = "Electronics";
            var price1 = 100m;
            var price2 = 110m;

            // Act
            var similarity = _algorithm.CalculateProductSimilarity(category, category, price1, price2);

            // Assert
            Assert.True(similarity > 0.5);
            Assert.True(similarity <= 1.0);
        }

        [Fact]
        public void CalculateProductSimilarity_DifferentCategory_ReturnsLowScore()
        {
            // Arrange
            var category1 = "Electronics";
            var category2 = "Books";
            var price1 = 100m;
            var price2 = 100m;

            // Act
            var similarity = _algorithm.CalculateProductSimilarity(category1, category2, price1, price2);

            // Assert
            Assert.True(similarity < 0.5);
        }

        [Fact]
        public void CalculateProductSimilarity_SameCategoryAndPrice_ReturnsMaxScore()
        {
            // Arrange
            var category = "Electronics";
            var price = 100m;

            // Act
            var similarity = _algorithm.CalculateProductSimilarity(category, category, price, price);

            // Assert
            Assert.Equal(1.0, similarity);
        }

        [Fact]
        public void CalculateProductSimilarity_LargePriceDifference_ReducesScore()
        {
            // Arrange
            var category = "Electronics";
            var price1 = 100m;
            var price2 = 500m;

            // Act
            var similarity = _algorithm.CalculateProductSimilarity(category, category, price1, price2);

            // Assert
            Assert.True(similarity > 0.0);
            Assert.True(similarity < 1.0);
        }
    }
}
