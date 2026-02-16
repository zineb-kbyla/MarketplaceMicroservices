using MassTransit;
using Recommendation.API.Application.DTOs;
using Recommendation.API.Application.Interfaces;
using Recommendation.API.Domain.Events;

namespace Recommendation.API.Infrastructure.Messaging
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly IRecommendationService _service;
        private readonly ILogger<OrderCreatedEventConsumer> _logger;

        public OrderCreatedEventConsumer(
            IRecommendationService service,
            ILogger<OrderCreatedEventConsumer> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var @event = context.Message;

            _logger.LogInformation(
                "Processing order {OrderId} for user {UserId}",
                @event.OrderId,
                @event.UserId
            );

            try
            {
                var items = @event.Items.Select(i => new PurchaseItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList();

                await _service.RecordPurchaseAsync(
                    @event.UserId,
                    @event.OrderId,
                    items
                );

                _logger.LogInformation(
                    "Successfully recorded purchase for order {OrderId}",
                    @event.OrderId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error recording purchase for order {OrderId}",
                    @event.OrderId
                );
                throw;
            }
        }
    }
}
