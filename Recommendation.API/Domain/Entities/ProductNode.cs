namespace Recommendation.API.Domain.Entities
{
    public class ProductNode
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int ViewCount { get; set; }
        public int PurchaseCount { get; set; }
        public double Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
