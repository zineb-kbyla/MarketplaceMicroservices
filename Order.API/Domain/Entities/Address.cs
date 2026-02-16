using MongoDB.Bson.Serialization.Attributes;

namespace Order.API.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Address
    {
        [BsonElement("street")]
        public string Street { get; set; }

        [BsonElement("city")]
        public string City { get; set; }

        [BsonElement("state")]
        public string State { get; set; }

        [BsonElement("country")]
        public string Country { get; set; }

        [BsonElement("zipCode")]
        public string ZipCode { get; set; }

        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; }
    }
}
