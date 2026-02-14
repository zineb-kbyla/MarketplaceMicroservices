using MongoDB.Driver;
using ProductEntity = Product.API.Domain.Entities.Product;
using CategoryEntity = Product.API.Domain.Entities.Category;

namespace Product.API.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<ProductEntity> Products =>
            _database.GetCollection<ProductEntity>("products");

        public IMongoCollection<CategoryEntity> Categories =>
            _database.GetCollection<CategoryEntity>("categories");

        public async Task InitializeAsync()
        {
            // Create indexes for better query performance
            var productIndexModel = new CreateIndexModel<ProductEntity>(
                Builders<ProductEntity>.IndexKeys.Text(x => x.Name).Text(x => x.Description));
            await Products.Indexes.CreateOneAsync(productIndexModel);

            var categoryIndexModel = new CreateIndexModel<CategoryEntity>(
                Builders<CategoryEntity>.IndexKeys.Ascending(x => x.Name));
            await Categories.Indexes.CreateOneAsync(categoryIndexModel);
        }
    }
}
