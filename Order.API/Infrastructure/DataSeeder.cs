using MongoDB.Driver;
using OrderEntity = Order.API.Domain.Entities.Order;
using Order.API.Domain.Entities;
using Order.API.Domain.Enums;
using Order.API.Infrastructure.Data;

namespace Order.API.Infrastructure
{
    public class DataSeeder
    {
        private readonly MongoDbContext _context;
        private readonly ILogger<DataSeeder> _logger;

        public DataSeeder(MongoDbContext context, ILogger<DataSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                var existingOrders = await _context.Orders.CountDocumentsAsync(Builders<OrderEntity>.Filter.Empty);

                if (existingOrders == 0)
                {
                    _logger.LogInformation("Seeding sample orders...");

                    var sampleOrders = new List<OrderEntity>
                    {
                        new OrderEntity
                        {
                            OrderNumber = "ORD-20240201-SAMPLE01",
                            UserId = "user123",
                            UserName = "John Doe",
                            Status = OrderStatus.Delivered,
                            OrderItems = new List<OrderItem>
                            {
                                new OrderItem
                                {
                                    ProductId = "prod001",
                                    ProductName = "iPhone 15 Pro",
                                    Quantity = 1,
                                    UnitPrice = 999.99m,
                                    TotalPrice = 999.99m
                                }
                            },
                            ShippingAddress = new Address
                            {
                                Street = "123 Main St",
                                City = "New York",
                                State = "NY",
                                Country = "USA",
                                ZipCode = "10001",
                                PhoneNumber = "+1234567890"
                            },
                            PaymentInfo = new Payment
                            {
                                CardName = "John Doe",
                                CardNumber = "****1234",
                                Expiration = "12/26",
                                CVV = "***",
                                PaymentMethod = PaymentMethod.CreditCard
                            },
                            TotalAmount = 999.99m,
                            CreatedAt = DateTime.UtcNow.AddDays(-5),
                            UpdatedAt = DateTime.UtcNow.AddDays(-2)
                        },
                        new OrderEntity
                        {
                            OrderNumber = "ORD-20240205-SAMPLE02",
                            UserId = "user456",
                            UserName = "Jane Smith",
                            Status = OrderStatus.Shipped,
                            OrderItems = new List<OrderItem>
                            {
                                new OrderItem
                                {
                                    ProductId = "prod002",
                                    ProductName = "MacBook Pro",
                                    Quantity = 1,
                                    UnitPrice = 2499.99m,
                                    TotalPrice = 2499.99m
                                },
                                new OrderItem
                                {
                                    ProductId = "prod003",
                                    ProductName = "AirPods Pro",
                                    Quantity = 2,
                                    UnitPrice = 299.99m,
                                    TotalPrice = 599.98m
                                }
                            },
                            ShippingAddress = new Address
                            {
                                Street = "456 Oak Ave",
                                City = "Los Angeles",
                                State = "CA",
                                Country = "USA",
                                ZipCode = "90001",
                                PhoneNumber = "+1987654321"
                            },
                            PaymentInfo = new Payment
                            {
                                CardName = "Jane Smith",
                                CardNumber = "****5678",
                                Expiration = "06/27",
                                CVV = "***",
                                PaymentMethod = PaymentMethod.CreditCard
                            },
                            TotalAmount = 3099.97m,
                            CreatedAt = DateTime.UtcNow.AddDays(-2),
                            UpdatedAt = DateTime.UtcNow.AddDays(-1)
                        },
                        new OrderEntity
                        {
                            OrderNumber = "ORD-20240210-SAMPLE03",
                            UserId = "user123",
                            UserName = "John Doe",
                            Status = OrderStatus.Pending,
                            OrderItems = new List<OrderItem>
                            {
                                new OrderItem
                                {
                                    ProductId = "prod004",
                                    ProductName = "iPad Air",
                                    Quantity = 1,
                                    UnitPrice = 599.99m,
                                    TotalPrice = 599.99m
                                }
                            },
                            ShippingAddress = new Address
                            {
                                Street = "123 Main St",
                                City = "New York",
                                State = "NY",
                                Country = "USA",
                                ZipCode = "10001",
                                PhoneNumber = "+1234567890"
                            },
                            PaymentInfo = new Payment
                            {
                                CardName = "John Doe",
                                CardNumber = "****1234",
                                Expiration = "12/26",
                                CVV = "***",
                                PaymentMethod = PaymentMethod.DebitCard
                            },
                            TotalAmount = 599.99m,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        }
                    };

                    await _context.Orders.InsertManyAsync(sampleOrders);
                    _logger.LogInformation($"Seeded {sampleOrders.Count} sample orders");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding data");
            }
        }
    }
}
