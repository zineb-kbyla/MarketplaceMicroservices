using Neo4j.Driver;
using Recommendation.API.Application.DTOs;
using Recommendation.API.Application.Interfaces;
using Recommendation.API.Domain.Entities;
using System.Text.Json;

namespace Recommendation.API.Infrastructure.Repositories
{
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly Data.Neo4jContext _neo4jContext;
        private readonly ILogger<RecommendationRepository> _logger;

        public RecommendationRepository(
            Data.Neo4jContext neo4jContext,
            ILogger<RecommendationRepository> logger)
        {
            _neo4jContext = neo4jContext;
            _logger = logger;
        }

        public async Task CreateUserIfNotExistsAsync(string userId, string name, string email)
        {
            const string query = @"
                MERGE (u:User {userId: $userId})
                SET u.name = $name,
                    u.email = $email,
                    u.joinedDate = datetime(),
                    u.lastActive = datetime()
                RETURN u";

            var parameters = new Dictionary<string, object>
            {
                { "userId", userId },
                { "name", name },
                { "email", email }
            };

            try
            {
                var result = await _neo4jContext.ExecuteQueryAsync(query, parameters);
                await result.ConsumeAsync();
                _logger.LogInformation("Created or merged user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {UserId}", userId);
                throw;
            }
        }

        public async Task CreateProductIfNotExistsAsync(ProductNode product)
        {
            const string query = @"
                MERGE (p:Product {productId: $productId})
                SET p.name = $name,
                    p.category = $category,
                    p.price = $price,
                    p.viewCount = 0,
                    p.purchaseCount = 0,
                    p.rating = 0,
                    p.createdAt = datetime()
                RETURN p";

            var parameters = new Dictionary<string, object>
            {
                { "productId", product.ProductId },
                { "name", product.Name },
                { "category", product.Category },
                { "price", product.Price }
            };

            try
            {
                var result = await _neo4jContext.ExecuteQueryAsync(query, parameters);
                await result.ConsumeAsync();
                _logger.LogInformation("Created or merged product {ProductId}", product.ProductId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product {ProductId}", product.ProductId);
                throw;
            }
        }

        public async Task<List<RecommendedProductDto>> GetPersonalizedRecommendationsAsync(
            string userId, int limit)
        {
            const string query = @"
                MERGE (u:User {userId: $userId})
                OPTIONAL MATCH (similar:User)-[:PURCHASED]->(rec:Product)
                WHERE similar.userId <> $userId AND NOT (u)-[:PURCHASED]->(rec)
                WITH rec, u, COUNT(DISTINCT similar) as popularity
                ORDER BY popularity DESC, rec.purchaseCount DESC
                LIMIT $limit
                RETURN DISTINCT rec.productId as productId, rec.name as name, rec.category as category,
                       rec.price as price, rec.rating as rating, popularity";

            var parameters = new Dictionary<string, object>
            {
                { "userId", userId },
                { "limit", limit }
            };

            var recommendations = new List<RecommendedProductDto>();

            try
            {
                var result = await _neo4jContext.ExecuteQueryAsync(query, parameters);
                await foreach (var record in result)
                {
                    if (record["productId"] != null)
                    {
                        var popularity = record["popularity"] != null ? (int)(long)record["popularity"] : 0;
                        recommendations.Add(new RecommendedProductDto
                        {
                            ProductId = (string)record["productId"],
                            Name = (string)(record["name"] ?? "Unknown"),
                            Category = (string)(record["category"] ?? "Unknown"),
                            Price = record["price"] != null ? (decimal)(double)record["price"] : 0m,
                            Rating = record["rating"] != null ? (double)record["rating"] : 0,
                            Score = Math.Min(popularity / 10m, 1m),
                            ImageUrl = string.Empty,
                            Reason = $"{popularity} utilisateurs similaires l'ont acheté",
                            Confidence = Math.Min(popularity / 20.0, 1.0)
                        });
                    }
                }

                _logger.LogInformation("Retrieved {Count} recommendations for user {UserId}", 
                    recommendations.Count, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting personalized recommendations for user {UserId}", userId);
                throw;
            }

            return recommendations;
        }

        public async Task<List<SimilarProductDto>> GetSimilarProductsAsync(
            string productId, int limit)
        {
            const string query = @"
                MATCH (p:Product {productId: $productId})
                MATCH (similar:Product)
                WHERE similar.productId <> $productId AND similar.category = p.category
                RETURN similar.productId as productId, similar.name as name, 
                       similar.category as category, similar.price as price
                ORDER BY similar.purchaseCount DESC
                LIMIT $limit";

            var parameters = new Dictionary<string, object>
            {
                { "productId", productId },
                { "limit", limit }
            };

            var similarProducts = new List<SimilarProductDto>();

            try
            {
                var result = await _neo4jContext.ExecuteQueryAsync(query, parameters);
                await foreach (var record in result)
                {
                    similarProducts.Add(new SimilarProductDto
                    {
                        ProductId = (string)record["productId"],
                        Name = (string)record["name"],
                        Category = (string)record["category"],
                        Price = record["price"] != null ? (decimal)(double)record["price"] : 0m,
                        SimilarityScore = 0.85,
                        Reason = "Produit similaire par catégorie"
                    });
                }

                _logger.LogInformation("Retrieved {Count} similar products for {ProductId}", 
                    similarProducts.Count, productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting similar products for {ProductId}", productId);
                throw;
            }

            return similarProducts;
        }

        public async Task<List<TrendingProductDto>> GetTrendingProductsAsync(
            int days, int limit)
        {
            const string query = @"
                MATCH (u:User)-[r:PURCHASED]->(p:Product)
                WHERE r.purchaseDate >= datetime() - duration({days: $days})
                WITH p, COUNT(r) as recentPurchases
                ORDER BY recentPurchases DESC
                LIMIT $limit
                RETURN p.productId as productId, p.name as name, p.category as category,
                       p.price as price, recentPurchases";

            var parameters = new Dictionary<string, object>
            {
                { "days", days },
                { "limit", limit }
            };

            var trending = new List<TrendingProductDto>();

            try
            {
                var result = await _neo4jContext.ExecuteQueryAsync(query, parameters);
                await foreach (var record in result)
                {
                    var recentPurchases = (int)(long)record["recentPurchases"];
                    trending.Add(new TrendingProductDto
                    {
                        ProductId = (string)record["productId"],
                        Name = (string)record["name"],
                        Category = (string)record["category"],
                        Price = record["price"] != null ? (decimal)(double)record["price"] : 0m,
                        RecentPurchases = recentPurchases,
                        TrendScore = Math.Min(recentPurchases / 10.0, 1.0)
                    });
                }

                _logger.LogInformation("Retrieved {Count} trending products for {Days} days", 
                    trending.Count, days);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trending products");
                throw;
            }

            return trending;
        }

        public async Task RecordPurchaseAsync(
            string userId, string orderId, List<PurchaseItemDto> items)
        {
            const string query = @"
                MERGE (u:User {userId: $userId})
                SET u.lastActive = datetime()
                MERGE (p:Product {productId: $productId})
                MERGE (u)-[r:PURCHASED]->(p)
                SET r.orderId = $orderId,
                    r.purchaseDate = datetime(),
                    r.quantity = $quantity,
                    r.price = $price
                WITH p, r
                SET p.purchaseCount = COALESCE(p.purchaseCount, 0) + 1
                RETURN p";

            try
            {
                _logger.LogInformation("Recording {ItemCount} items for user {UserId}, order {OrderId}", 
                    items.Count, userId, orderId);

                foreach (var item in items)
                {
                    var parameters = new Dictionary<string, object>
                    {
                        { "userId", userId },
                        { "orderId", orderId },
                        { "productId", item.ProductId },
                        { "quantity", item.Quantity },
                        { "price", (double)item.Price }
                    };

                    var result = await _neo4jContext.ExecuteQueryAsync(query, parameters);
                    await result.ConsumeAsync();
                    
                    _logger.LogInformation("Recorded purchase item {ProductId} for user {UserId}", 
                        item.ProductId, userId);
                }

                _logger.LogInformation("Successfully recorded {ItemCount} items for order {OrderId}", 
                    items.Count, orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording purchase for user {UserId}, order {OrderId}", 
                    userId, orderId);
                throw;
            }
        }

        public async Task RecordViewAsync(
            string userId, string productId, int duration, string source)
        {
            const string query = @"
                MERGE (u:User {userId: $userId})
                SET u.lastActive = datetime()
                MERGE (p:Product {productId: $productId})
                MERGE (u)-[r:VIEWED]->(p)
                SET r.viewedAt = datetime(),
                    r.duration = $duration,
                    r.source = $source
                WITH p, r
                SET p.viewCount = COALESCE(p.viewCount, 0) + 1
                RETURN p";

            var parameters = new Dictionary<string, object>
            {
                { "userId", userId },
                { "productId", productId },
                { "duration", duration },
                { "source", source }
            };

            try
            {
                var result = await _neo4jContext.ExecuteQueryAsync(query, parameters);
                await result.ConsumeAsync();
                _logger.LogInformation("Recorded view for user {UserId}, product {ProductId} from {Source}", 
                    userId, productId, source);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording view for user {UserId}, product {ProductId}", 
                    userId, productId);
                // Don't throw - views are non-critical
            }
        }

        public async Task<List<UserHistoryDto>> GetUserHistoryAsync(string userId, int limit)
        {
            const string query = @"
                MERGE (u:User {userId: $userId})
                WITH u
                OPTIONAL MATCH (u)-[r:PURCHASED]->(p:Product)
                RETURN p.productId as productId, p.name as name, p.category as category,
                       toString(r.purchaseDate) as purchaseDate, r.quantity as quantity, r.price as price
                ORDER BY r.purchaseDate DESC
                LIMIT $limit";

            var parameters = new Dictionary<string, object>
            {
                { "userId", userId },
                { "limit", limit }
            };

            var history = new List<UserHistoryDto>();

            try
            {
                var result = await _neo4jContext.ExecuteQueryAsync(query, parameters);
                await foreach (var record in result)
                {
                    if (record["productId"] != null)
                    {
                        var purchaseDate = DateTime.UtcNow;
                        if (record["purchaseDate"] != null)
                        {
                            var dateStr = record["purchaseDate"].ToString();
                            if (DateTime.TryParse(dateStr, out var parsedDate))
                            {
                                purchaseDate = parsedDate;
                            }
                        }

                        history.Add(new UserHistoryDto
                        {
                            ProductId = (string)record["productId"],
                            Name = (string)(record["name"] ?? "Unknown"),
                            Category = (string)(record["category"] ?? "Unknown"),
                            PurchaseDate = purchaseDate,
                            Quantity = record["quantity"] != null ? (int)(long)record["quantity"] : 0,
                            Price = record["price"] != null ? (decimal)(double)record["price"] : 0m
                        });
                    }
                }

                _logger.LogInformation("Retrieved {Count} history items for user {UserId}", 
                    history.Count, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user history for {UserId}", userId);
                throw;
            }

            return history;
        }

        public async Task<List<string>> GetSimilarUsersAsync(string userId, int limit)
        {
            const string query = @"
                MATCH (u:User {userId: $userId})-[:PURCHASED]->(p:Product)
                      <-[:PURCHASED]-(similar:User)
                WHERE similar.userId <> $userId
                WITH similar, COUNT(DISTINCT p) as commonPurchases
                ORDER BY commonPurchases DESC
                LIMIT $limit
                RETURN similar.userId";

            var parameters = new Dictionary<string, object>
            {
                { "userId", userId },
                { "limit", limit }
            };

            var similarUsers = new List<string>();

            try
            {
                var result = await _neo4jContext.ExecuteQueryAsync(query, parameters);
                await foreach (var record in result)
                {
                    similarUsers.Add((string)record["userId"]);
                }

                _logger.LogInformation("Retrieved {Count} similar users for {UserId}", 
                    similarUsers.Count, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting similar users for {UserId}", userId);
                throw;
            }

            return similarUsers;
        }

        public async Task<Dictionary<string, int>> GetUserPurchaseHistoryAsync(string userId)
        {
            const string query = @"
                MATCH (u:User {userId: $userId})-[:PURCHASED]->(p:Product)
                RETURN p.category as category, COUNT(p) as count";

            var parameters = new Dictionary<string, object>
            {
                { "userId", userId }
            };

            var purchaseHistory = new Dictionary<string, int>();

            try
            {
                var result = await _neo4jContext.ExecuteQueryAsync(query, parameters);
                await foreach (var record in result)
                {
                    purchaseHistory[(string)record["category"]] = (int)(long)record["count"];
                }

                _logger.LogInformation("Retrieved purchase history for user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchase history for {UserId}", userId);
                throw;
            }

            return purchaseHistory;
        }

        public async Task<List<string>> GetRecommendedProductIdsAsync(
            string userId, List<string> similarUserIds, int limit)
        {
            const string query = @"
                MATCH (u:User {userId: $userId})
                MATCH (similar:User)-[:PURCHASED]->(rec:Product)
                WHERE similar.userId IN $similarUserIds AND NOT (u)-[:PURCHASED]->(rec)
                RETURN DISTINCT rec.productId
                ORDER BY rec.purchaseCount DESC
                LIMIT $limit";

            var parameters = new Dictionary<string, object>
            {
                { "userId", userId },
                { "similarUserIds", similarUserIds },
                { "limit", limit }
            };

            var productIds = new List<string>();

            try
            {
                var result = await _neo4jContext.ExecuteQueryAsync(query, parameters);
                await foreach (var record in result)
                {
                    productIds.Add((string)record["productId"]);
                }

                _logger.LogInformation("Retrieved {Count} recommended product IDs", productIds.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recommended product IDs");
                throw;
            }

            return productIds;
        }

        public async Task UpdateUserLastActiveAsync(string userId)
        {
            const string query = @"
                MATCH (u:User {userId: $userId})
                SET u.lastActive = datetime()
                RETURN u";

            var parameters = new Dictionary<string, object>
            {
                { "userId", userId }
            };

            try
            {
                var result = await _neo4jContext.ExecuteQueryAsync(query, parameters);
                await result.ConsumeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last active for user {UserId}", userId);
                // Don't throw - this is non-critical
            }
        }
    }
}
