using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Recommendation.API.Application.DTOs;
using Recommendation.API.Application.Interfaces;
using Recommendation.API.Infrastructure.Data;

namespace Recommendation.API.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove real Neo4j context and repository
                var descriptorContext = services.FirstOrDefault(d => d.ServiceType == typeof(Neo4jContext));
                if (descriptorContext != null)
                    services.Remove(descriptorContext);

                var descriptorRepo = services.FirstOrDefault(d => d.ServiceType == typeof(IRecommendationRepository));
                if (descriptorRepo != null)
                    services.Remove(descriptorRepo);

                // Add mock repository
                var mockRepository = new Mock<IRecommendationRepository>();

                // Setup mock methods to return empty collections for integration tests
                mockRepository
                    .Setup(r => r.GetPersonalizedRecommendationsAsync(It.IsAny<string>(), It.IsAny<int>()))
                    .ReturnsAsync(new List<RecommendedProductDto>());

                mockRepository
                    .Setup(r => r.GetSimilarProductsAsync(It.IsAny<string>(), It.IsAny<int>()))
                    .ReturnsAsync(new List<SimilarProductDto>());

                mockRepository
                    .Setup(r => r.GetTrendingProductsAsync(It.IsAny<int>(), It.IsAny<int>()))
                    .ReturnsAsync(new List<TrendingProductDto>());

                mockRepository
                    .Setup(r => r.GetUserHistoryAsync(It.IsAny<string>(), It.IsAny<int>()))
                    .ReturnsAsync(new List<UserHistoryDto>());

                mockRepository
                    .Setup(r => r.RecordViewAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(Task.CompletedTask);

                mockRepository
                    .Setup(r => r.RecordPurchaseAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<PurchaseItemDto>>()))
                    .Returns(Task.CompletedTask);

                // Mock additional repository methods for algorithm support
                mockRepository
                    .Setup(r => r.GetUserPurchaseHistoryAsync(It.IsAny<string>()))
                    .ReturnsAsync(new Dictionary<string, int>());

                mockRepository
                    .Setup(r => r.GetSimilarUsersAsync(It.IsAny<string>(), It.IsAny<int>()))
                    .ReturnsAsync(new List<string>());

                mockRepository
                    .Setup(r => r.GetRecommendedProductIdsAsync(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<int>()))
                    .ReturnsAsync(new List<string>());

                mockRepository
                    .Setup(r => r.UpdateUserLastActiveAsync(It.IsAny<string>()))
                    .Returns(Task.CompletedTask);

                mockRepository
                    .Setup(r => r.CreateUserIfNotExistsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.CompletedTask);

                mockRepository
                    .Setup(r => r.CreateProductIfNotExistsAsync(It.IsAny<Recommendation.API.Domain.Entities.ProductNode>()))
                    .Returns(Task.CompletedTask);

                services.AddScoped<IRecommendationRepository>(_ => mockRepository.Object);
            });
        }
    }
}

