using Recommendation.API.Application.DTOs;
using Recommendation.API.Application.Interfaces;

namespace Recommendation.API.Application.Algorithms
{
    public class ContentBasedFilteringAlgorithm
    {
        private readonly IRecommendationRepository _repository;
        private readonly ILogger<ContentBasedFilteringAlgorithm> _logger;

        public ContentBasedFilteringAlgorithm(
            IRecommendationRepository repository,
            ILogger<ContentBasedFilteringAlgorithm> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<List<SimilarProductDto>> FindSimilarByCategoryAsync(
            string productId, int limit = 5)
        {
            _logger.LogInformation("Finding similar products by category for {ProductId}", productId);
            return await _repository.GetSimilarProductsAsync(productId, limit);
        }

        public double CalculateProductSimilarity(string category1, string category2, decimal price1, decimal price2)
        {
            double categoryScore = category1.Equals(category2, StringComparison.OrdinalIgnoreCase) ? 1.0 : 0.0;
            
            double priceDifference = Math.Abs((double)(price1 - price2));
            double avgPrice = (double)((price1 + price2) / 2);
            double priceScore = avgPrice > 0 ? 1.0 - Math.Min(priceDifference / avgPrice, 1.0) : 0;

            return (categoryScore * 0.7) + (priceScore * 0.3);
        }
    }
}
