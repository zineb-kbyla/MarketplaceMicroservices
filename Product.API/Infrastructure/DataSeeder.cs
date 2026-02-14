using MongoDB.Driver;
using ProductEntity = Product.API.Domain.Entities.Product;
using CategoryEntity = Product.API.Domain.Entities.Category;
using Product.API.Infrastructure.Data;

namespace Product.API.Infrastructure
{
    public class DataSeeder
    {
        private readonly MongoDbContext _context;

        public DataSeeder(MongoDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Check if data already exists
            var productCount = await _context.Products.CountDocumentsAsync(_ => true);
            if (productCount > 0)
            {
                Console.WriteLine("Database already seeded, skipping...");
                return;
            }

            // Seed categories
            var categories = new List<CategoryEntity>
            {
                new CategoryEntity
                {
                    Name = "Electronics",
                    Description = "Electronic devices and gadgets",
                    ImageUrl = "https://example.com/electronics.jpg",
                    ProductCount = 3,
                    CreatedAt = DateTime.UtcNow
                },
                new CategoryEntity
                {
                    Name = "Fashion",
                    Description = "Clothing, shoes, and accessories",
                    ImageUrl = "https://example.com/fashion.jpg",
                    ProductCount = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new CategoryEntity
                {
                    Name = "Books",
                    Description = "Physical and digital books",
                    ImageUrl = "https://example.com/books.jpg",
                    ProductCount = 1,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await _context.Categories.InsertManyAsync(categories);
            Console.WriteLine($"Seeded {categories.Count} categories");

            // Seed products
            var products = new List<ProductEntity>
            {
                new ProductEntity
                {
                    Name = "iPhone 15 Pro",
                    Description = "Latest flagship smartphone with advanced camera system",
                    Category = "Electronics",
                    Price = 999.99m,
                    Stock = 50,
                    ImageUrl = "https://example.com/iphone15.jpg",
                    Rating = 4.8,
                    ReviewCount = 1250,
                    Status = "Available",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new ProductEntity
                {
                    Name = "Samsung Galaxy S24",
                    Description = "Powerful Android flagship with 5G connectivity",
                    Category = "Electronics",
                    Price = 899.99m,
                    Stock = 40,
                    ImageUrl = "https://example.com/galaxy.jpg",
                    Rating = 4.6,
                    ReviewCount = 980,
                    Status = "Available",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new ProductEntity
                {
                    Name = "MacBook Pro 16 inch",
                    Description = "Powerful laptop for creative professionals",
                    Category = "Electronics",
                    Price = 2499.99m,
                    Stock = 15,
                    ImageUrl = "https://example.com/macbook.jpg",
                    Rating = 4.9,
                    ReviewCount = 560,
                    Status = "Available",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new ProductEntity
                {
                    Name = "Premium Cashmere Sweater",
                    Description = "Luxury cashmere sweater in multiple colors",
                    Category = "Fashion",
                    Price = 199.99m,
                    Stock = 100,
                    ImageUrl = "https://example.com/sweater.jpg",
                    Rating = 4.7,
                    ReviewCount = 340,
                    Status = "Available",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new ProductEntity
                {
                    Name = "Designer Running Shoes",
                    Description = "High-performance running shoes with advanced cushioning",
                    Category = "Fashion",
                    Price = 149.99m,
                    Stock = 80,
                    ImageUrl = "https://example.com/shoes.jpg",
                    Rating = 4.5,
                    ReviewCount = 720,
                    Status = "Available",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new ProductEntity
                {
                    Name = "Clean Code by Robert C. Martin",
                    Description = "A Handbook of Agile Software Craftsmanship",
                    Category = "Books",
                    Price = 45.99m,
                    Stock = 200,
                    ImageUrl = "https://example.com/cleancode.jpg",
                    Rating = 4.9,
                    ReviewCount = 2100,
                    Status = "Available",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await _context.Products.InsertManyAsync(products);
            Console.WriteLine($"Seeded {products.Count} products");
        }
    }
}
