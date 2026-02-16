namespace Recommendation.API.Domain.Entities
{
    public class PurchaseRelation
    {
        public string OrderId { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
