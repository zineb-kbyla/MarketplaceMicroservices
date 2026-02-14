using MongoDB.Driver;
using Product.API.Application.Interfaces;
using CategoryEntity = Product.API.Domain.Entities.Category;
using Product.API.Infrastructure.Data;

namespace Product.API.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MongoDbContext _context;

        public CategoryRepository(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryEntity>> GetAllAsync()
        {
            return await _context.Categories
                .Find(Builders<CategoryEntity>.Filter.Empty)
                .ToListAsync();
        }

        public async Task<CategoryEntity> GetByIdAsync(string id)
        {
            return await _context.Categories
                .Find(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<CategoryEntity> CreateAsync(CategoryEntity category)
        {
            category.Id ??= MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            category.CreatedAt = DateTime.UtcNow;

            await _context.Categories.InsertOneAsync(category);
            return category;
        }

        public async Task<bool> UpdateAsync(CategoryEntity category)
        {
            var result = await _context.Categories.ReplaceOneAsync(
                c => c.Id == category.Id,
                category);

            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _context.Categories.DeleteOneAsync(c => c.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
