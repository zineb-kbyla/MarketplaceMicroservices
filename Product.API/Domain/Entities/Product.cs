using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Product.API.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("category")]
        public string Category { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("stock")]
        public int Stock { get; set; }

        [BsonElement("imageUrl")]
        public string ImageUrl { get; set; }

        [BsonElement("rating")]
        public double Rating { get; set; }

        [BsonElement("reviewCount")]
        public int ReviewCount { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "Available";

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Domain Methods
        public void DecrementStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            if (Stock < quantity)
                throw new InvalidOperationException("Insufficient stock");

            Stock -= quantity;
            UpdatedAt = DateTime.UtcNow;

            if (Stock == 0)
                Status = "OutOfStock";
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice < 0)
                throw new ArgumentException("Price cannot be negative");

            Price = newPrice;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddReview(double rating)
        {
            if (rating < 0 || rating > 5)
                throw new ArgumentException("Rating must be between 0 and 5");

            // Update average rating
            double totalRating = Rating * ReviewCount;
            ReviewCount++;
            Rating = (totalRating + rating) / ReviewCount;

            UpdatedAt = DateTime.UtcNow;
        }
    }
}
