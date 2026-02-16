using MongoDB.Driver;
using OrderEntity = Order.API.Domain.Entities.Order;

namespace Order.API.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly MongoDbSettings _settings;

        public MongoDbContext(MongoDbSettings settings)
        {
            _settings = settings;
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<OrderEntity> Orders => 
            _database.GetCollection<OrderEntity>(_settings.OrdersCollectionName);

        public async Task InitializeAsync()
        {
            // Create indexes
            var orderIndexKeys = Builders<OrderEntity>.IndexKeys
                .Ascending(o => o.UserId)
                .Ascending(o => o.OrderNumber)
                .Descending(o => o.CreatedAt);

            var orderIndexModel = new CreateIndexModel<OrderEntity>(orderIndexKeys);
            await Orders.Indexes.CreateOneAsync(orderIndexModel);

            // Index for status
            var statusIndexKeys = Builders<OrderEntity>.IndexKeys.Ascending(o => o.Status);
            var statusIndexModel = new CreateIndexModel<OrderEntity>(statusIndexKeys);
            await Orders.Indexes.CreateOneAsync(statusIndexModel);
        }
    }
}
