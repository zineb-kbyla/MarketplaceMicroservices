namespace Order.API.Infrastructure.Data
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string OrdersCollectionName { get; set; } = "orders";
    }
}
