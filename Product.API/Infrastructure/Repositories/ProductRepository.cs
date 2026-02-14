using MongoDB.Driver;
using Product.API.Application.Interfaces;
using ProductEntity = Product.API.Domain.Entities.Product;
using Product.API.Infrastructure.Data;

namespace Product.API.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly MongoDbContext _context;

        public ProductRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductEntity>> GetAllAsync()
        {
            return await _context.Products
                .Find(Builders<ProductEntity>.Filter.Empty)
                .ToListAsync();
        }

        public async Task<ProductEntity> GetByIdAsync(string id)
        {
            return await _context.Products
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ProductEntity>> GetByCategoryAsync(string category)
        {
            return await _context.Products
                .Find(p => p.Category == category)
                .ToListAsync();
        }

        public async Task<ProductEntity> CreateAsync(ProductEntity product)
        {
            product.Id ??= MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.Products.InsertOneAsync(product);
            return product;
        }

        public async Task<bool> UpdateAsync(ProductEntity product)
        {
            product.UpdatedAt = DateTime.UtcNow;

            var result = await _context.Products.ReplaceOneAsync(
                p => p.Id == product.Id,
                product);

            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _context.Products.DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<ProductEntity>> SearchAsync(string query)
        {
            var filter = Builders<ProductEntity>.Filter.Or(
                Builders<ProductEntity>.Filter.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                Builders<ProductEntity>.Filter.Regex(p => p.Description, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                Builders<ProductEntity>.Filter.Regex(p => p.Category, new MongoDB.Bson.BsonRegularExpression(query, "i"))
            );

            return await _context.Products
                .Find(filter)
                .ToListAsync();
        }
    }
}
