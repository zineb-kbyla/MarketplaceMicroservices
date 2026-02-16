using MongoDB.Bson.Serialization.Attributes;
using Order.API.Domain.Enums;

namespace Order.API.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Payment
    {
        [BsonElement("cardName")]
        public string CardName { get; set; }

        [BsonElement("cardNumber")]
        public string CardNumber { get; set; }

        [BsonElement("expiration")]
        public string Expiration { get; set; }

        [BsonElement("cvv")]
        public string CVV { get; set; }

        [BsonElement("paymentMethod")]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public PaymentMethod PaymentMethod { get; set; }
    }
}
