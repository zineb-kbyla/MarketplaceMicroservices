namespace Product.API.Domain.Events
{
    public class ProductCreatedEvent
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProductUpdatedEvent
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ProductViewedEvent
    {
        public string ProductId { get; set; }
        public string UserId { get; set; }
        public DateTime ViewedAt { get; set; }
    }

    public class StockUpdatedEvent
    {
        public string ProductId { get; set; }
        public int OldStock { get; set; }
        public int NewStock { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
