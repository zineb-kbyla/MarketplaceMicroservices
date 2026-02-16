using Recommendation.API.Application.DTOs;
using Recommendation.API.Application.Interfaces;
using Recommendation.API.Application.Algorithms;

namespace Recommendation.API.Application.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IRecommendationRepository _repository;
        private readonly CollaborativeFilteringAlgorithm _collaborativeFilter;
        private readonly ContentBasedFilteringAlgorithm _contentFilter;
        private readonly ILogger<RecommendationService> _logger;

        public RecommendationService(
            IRecommendationRepository repository,
            CollaborativeFilteringAlgorithm collaborativeFilter,
            ContentBasedFilteringAlgorithm contentFilter,
            ILogger<RecommendationService> logger)
        {
            _repository = repository;
            _collaborativeFilter = collaborativeFilter;
            _contentFilter = contentFilter;
            _logger = logger;
        }

        public async Task<List<RecommendedProductDto>> GetPersonalizedRecommendationsAsync(
            string userId, int limit = 10)
        {
            _logger.LogInformation("Getting personalized recommendations for user {UserId}", userId);

            try
            {
                await _repository.UpdateUserLastActiveAsync(userId);
                
                var recommendations = await _collaborativeFilter.GetRecommendationsAsync(userId, limit);
                
                _logger.LogInformation(
                    "Generated {Count} recommendations for user {UserId}", 
                    recommendations.Count, 
                    userId
                );

                return recommendations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting personalized recommendations for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<SimilarProductDto>> GetSimilarProductsAsync(
            string productId, int limit = 5)
        {
            _logger.LogInformation("Getting similar products for {ProductId}", productId);

            try
            {
                var similarProducts = await _contentFilter.FindSimilarByCategoryAsync(productId, limit);
                
                _logger.LogInformation(
                    "Found {Count} similar products for {ProductId}", 
                    similarProducts.Count, 
                    productId
                );

                return similarProducts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting similar products for {ProductId}", productId);
                throw;
            }
        }

        public async Task<List<TrendingProductDto>> GetTrendingProductsAsync(
            int days = 7, int limit = 10)
        {
            _logger.LogInformation("Getting trending products for last {Days} days", days);

            try
            {
                var trending = await _repository.GetTrendingProductsAsync(days, limit);
                
                _logger.LogInformation("Found {Count} trending products", trending.Count);

                return trending;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trending products");
                throw;
            }
        }

        public async Task RecordPurchaseAsync(
            string userId, string orderId, List<PurchaseItemDto> items)
        {
            _logger.LogInformation(
                "Recording purchase for user {UserId}, order {OrderId}", 
                userId, 
                orderId
            );

            try
            {
                await _repository.RecordPurchaseAsync(userId, orderId, items);
                
                _logger.LogInformation(
                    "Successfully recorded {Count} items for order {OrderId}", 
                    items.Count, 
                    orderId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, 
                    "Error recording purchase for user {UserId}, order {OrderId}", 
                    userId, 
                    orderId
                );
                throw;
            }
        }

        public async Task RecordViewAsync(
            string userId, string productId, int duration = 0, string source = "web")
        {
            _logger.LogInformation(
                "Recording view for user {UserId}, product {ProductId}", 
                userId, 
                productId
            );

            try
            {
                await _repository.RecordViewAsync(userId, productId, duration, source);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, 
                    "Error recording view for user {UserId}, product {ProductId}", 
                    userId, 
                    productId
                );
                // Don't throw - views are non-critical
            }
        }

        public async Task<List<UserHistoryDto>> GetUserHistoryAsync(
            string userId, int limit = 20)
        {
            _logger.LogInformation("Getting history for user {UserId}", userId);

            try
            {
                var history = await _repository.GetUserHistoryAsync(userId, limit);
                
                _logger.LogInformation(
                    "Retrieved {Count} history items for user {UserId}", 
                    history.Count, 
                    userId
                );

                return history;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting history for user {UserId}", userId);
                throw;
            }
        }
    }
}
