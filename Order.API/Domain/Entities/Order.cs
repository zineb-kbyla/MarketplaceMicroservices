using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Order.API.Domain.Enums;

namespace Order.API.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("orderNumber")]
        public string OrderNumber { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("userName")]
        public string UserName { get; set; }

        [BsonElement("totalAmount")]
        public decimal TotalAmount { get; set; }

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public OrderStatus Status { get; set; }

        [BsonElement("orderItems")]
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        [BsonElement("shippingAddress")]
        public Address ShippingAddress { get; set; }

        [BsonElement("paymentInfo")]
        public Payment PaymentInfo { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Domain Methods
        public decimal CalculateTotal()
        {
            return OrderItems?.Sum(item => item.TotalPrice) ?? 0;
        }

        public void UpdateStatus(OrderStatus newStatus)
        {
            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool CanBeCancelled()
        {
            return Status == OrderStatus.Pending || 
                   Status == OrderStatus.Confirmed || 
                   Status == OrderStatus.Processing;
        }
    }
}
