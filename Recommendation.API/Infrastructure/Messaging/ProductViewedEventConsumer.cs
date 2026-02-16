using MassTransit;
using Recommendation.API.Application.Interfaces;
using Recommendation.API.Domain.Events;

namespace Recommendation.API.Infrastructure.Messaging
{
    public class ProductViewedEventConsumer : IConsumer<ProductViewedEvent>
    {
        private readonly IRecommendationService _service;
        private readonly ILogger<ProductViewedEventConsumer> _logger;

        public ProductViewedEventConsumer(
            IRecommendationService service,
            ILogger<ProductViewedEventConsumer> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductViewedEvent> context)
        {
            var @event = context.Message;

            _logger.LogInformation(
                "User {UserId} viewed product {ProductId}",
                @event.UserId,
                @event.ProductId
            );

            try
            {
                await _service.RecordViewAsync(
                    @event.UserId,
                    @event.ProductId,
                    @event.Duration,
                    @event.Source
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error recording product view"
                );
                // Don't throw - views are non-critical
            }
        }
    }
}
