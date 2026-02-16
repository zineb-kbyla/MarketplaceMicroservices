using Recommendation.API.Application.DTOs;
using Recommendation.API.Application.Interfaces;
using Recommendation.API.Domain.Entities;

namespace Recommendation.API.Application.Algorithms
{
    public class CollaborativeFilteringAlgorithm
    {
        private readonly IRecommendationRepository _repository;
        private readonly ILogger<CollaborativeFilteringAlgorithm> _logger;

        public CollaborativeFilteringAlgorithm(
            IRecommendationRepository repository,
            ILogger<CollaborativeFilteringAlgorithm> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<List<string>> GetSimilarUsersAsync(string userId, int limit = 20)
        {
            _logger.LogInformation("Finding similar users for {UserId}", userId);
            return await _repository.GetSimilarUsersAsync(userId, limit);
        }

        public async Task<List<RecommendedProductDto>> GetRecommendationsAsync(
            string userId, int limit = 10)
        {
            _logger.LogInformation("Generating collaborative filtering recommendations for {UserId}", userId);

            // 1. Get user's purchase history
            var userHistory = await _repository.GetUserPurchaseHistoryAsync(userId);

            if (!userHistory.Any())
            {
                _logger.LogInformation("No purchase history for {UserId}, returning popular products", userId);
                var trending = await _repository.GetTrendingProductsAsync(30, limit);
                return ConvertTrendingToRecommended(trending);
            }

            // 2. Find similar users
            var similarUsers = await GetSimilarUsersAsync(userId, 20);

            if (!similarUsers.Any())
            {
                _logger.LogInformation("No similar users found for {UserId}", userId);
                return new List<RecommendedProductDto>();
            }

            // 3. Get candidate products from similar users
            var candidateProductIds = await _repository.GetRecommendedProductIdsAsync(
                userId, 
                similarUsers, 
                limit * 3
            );

            // 4. Get personalized recommendations with scores
            var recommendations = await _repository.GetPersonalizedRecommendationsAsync(userId, limit);

            // 5. Calculate confidence based on similar users count
            foreach (var rec in recommendations)
            {
                rec.Confidence = CalculateConfidence(similarUsers.Count);
                rec.Reason = GenerateReason(similarUsers.Count);
            }

            return recommendations;
        }

        public double CalculateUserSimilarity(
            Dictionary<string, int> user1Purchases,
            Dictionary<string, int> user2Purchases)
        {
            if (!user1Purchases.Any() || !user2Purchases.Any())
                return 0;

            var commonProducts = user1Purchases.Keys
                .Intersect(user2Purchases.Keys)
                .Count();

            var totalProducts = user1Purchases.Keys
                .Union(user2Purchases.Keys)
                .Count();

            return totalProducts > 0 ? (double)commonProducts / totalProducts : 0;
        }

        private double CalculateConfidence(int similarUsersCount)
        {
            return Math.Min(similarUsersCount / 20.0, 1.0);
        }

        private string GenerateReason(int similarUsersCount)
        {
            if (similarUsersCount > 10)
                return $"{similarUsersCount} utilisateurs similaires ont acheté ce produit";
            else if (similarUsersCount > 5)
                return "Populaire parmi les utilisateurs ayant des goûts similaires";
            else
                return "Basé sur votre historique d'achats";
        }

        private List<RecommendedProductDto> ConvertTrendingToRecommended(List<TrendingProductDto> trending)
        {
            return trending.Select(t => new RecommendedProductDto
            {
                ProductId = t.ProductId,
                Name = t.Name,
                Category = t.Category,
                Price = t.Price,
                Score = (decimal)t.TrendScore,
                Reason = "Produit populaire",
                Confidence = 0.5
            }).ToList();
        }
    }
}
