using MongoDB.Bson.Serialization.Attributes;

namespace Order.API.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class OrderItem
    {
        [BsonElement("productId")]
        public string ProductId { get; set; }

        [BsonElement("productName")]
        public string ProductName { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("unitPrice")]
        public decimal UnitPrice { get; set; }

        [BsonElement("totalPrice")]
        public decimal TotalPrice { get; set; }

        public decimal CalculateItemTotal()
        {
            return Quantity * UnitPrice;
        }
    }
}
