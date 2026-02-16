using MongoDB.Driver;
using Order.API.Application.Interfaces;
using OrderEntity = Order.API.Domain.Entities.Order;
using Order.API.Domain.Enums;
using Order.API.Infrastructure.Data;

namespace Order.API.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MongoDbContext _context;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(MongoDbContext context, ILogger<OrderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<OrderEntity>> GetAllAsync()
        {
            try
            {
                return await _context.Orders
                    .Find(_ => true)
                    .SortByDescending(o => o.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all orders");
                throw;
            }
        }

        public async Task<OrderEntity> GetByIdAsync(string id)
        {
            try
            {
                return await _context.Orders
                    .Find(o => o.Id == id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order {id}");
                throw;
            }
        }

        public async Task<List<OrderEntity>> GetByUserIdAsync(string userId)
        {
            try
            {
                return await _context.Orders
                    .Find(o => o.UserId == userId)
                    .SortByDescending(o => o.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving orders for user {userId}");
                throw;
            }
        }

        public async Task<OrderEntity> CreateAsync(OrderEntity order)
        {
            try
            {
                await _context.Orders.InsertOneAsync(order);
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(OrderEntity order)
        {
            try
            {
                order.UpdatedAt = DateTime.UtcNow;

                var result = await _context.Orders.ReplaceOneAsync(
                    o => o.Id == order.Id,
                    order
                );

                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order {order.Id}");
                throw;
            }
        }

        public async Task<bool> UpdateStatusAsync(string id, OrderStatus status)
        {
            try
            {
                var update = Builders<OrderEntity>.Update
                    .Set(o => o.Status, status)
                    .Set(o => o.UpdatedAt, DateTime.UtcNow);

                var result = await _context.Orders.UpdateOneAsync(
                    o => o.Id == id,
                    update
                );

                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order status {id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var result = await _context.Orders.DeleteOneAsync(o => o.Id == id);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order {id}");
                throw;
            }
        }
    }
}
