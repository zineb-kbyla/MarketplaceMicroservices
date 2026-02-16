namespace Recommendation.API.Domain.Events
{
    public class RecommendationGeneratedEvent
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> RecommendedProductIds { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
        public string AlgorithmUsed { get; set; } = string.Empty;
    }

    public class OrderCreatedEvent
    {
        public string OrderId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public List<OrderItemEvent> Items { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class OrderItemEvent
    {
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductViewedEvent
    {
        public string UserId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public DateTime ViewedAt { get; set; }
        public int Duration { get; set; }
        public string Source { get; set; } = string.Empty;
    }
}
