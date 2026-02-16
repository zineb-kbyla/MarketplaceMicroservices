using Recommendation.API.Application.DTOs;
using Recommendation.API.Domain.Entities;

namespace Recommendation.API.Application.Interfaces
{
    public interface IRecommendationRepository
    {
        Task<List<RecommendedProductDto>> GetPersonalizedRecommendationsAsync(string userId, int limit);
        Task<List<SimilarProductDto>> GetSimilarProductsAsync(string productId, int limit);
        Task<List<TrendingProductDto>> GetTrendingProductsAsync(int days, int limit);
        Task RecordPurchaseAsync(string userId, string orderId, List<PurchaseItemDto> items);
        Task RecordViewAsync(string userId, string productId, int duration, string source);
        Task<List<UserHistoryDto>> GetUserHistoryAsync(string userId, int limit);
        Task<List<string>> GetSimilarUsersAsync(string userId, int limit);
        Task<Dictionary<string, int>> GetUserPurchaseHistoryAsync(string userId);
        Task<List<string>> GetRecommendedProductIdsAsync(string userId, List<string> similarUserIds, int limit);
        Task CreateUserIfNotExistsAsync(string userId, string name, string email);
        Task CreateProductIfNotExistsAsync(ProductNode product);
        Task UpdateUserLastActiveAsync(string userId);
    }

    public interface IRecommendationService
    {
        Task<List<RecommendedProductDto>> GetPersonalizedRecommendationsAsync(string userId, int limit = 10);
        Task<List<SimilarProductDto>> GetSimilarProductsAsync(string productId, int limit = 5);
        Task<List<TrendingProductDto>> GetTrendingProductsAsync(int days = 7, int limit = 10);
        Task RecordPurchaseAsync(string userId, string orderId, List<PurchaseItemDto> items);
        Task RecordViewAsync(string userId, string productId, int duration = 0, string source = "web");
        Task<List<UserHistoryDto>> GetUserHistoryAsync(string userId, int limit = 20);
    }
}
