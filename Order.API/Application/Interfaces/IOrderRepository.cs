using OrderEntity = Order.API.Domain.Entities.Order;
using Order.API.Domain.Enums;

namespace Order.API.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<OrderEntity>> GetAllAsync();
        Task<OrderEntity> GetByIdAsync(string id);
        Task<List<OrderEntity>> GetByUserIdAsync(string userId);
        Task<OrderEntity> CreateAsync(OrderEntity order);
        Task<bool> UpdateAsync(OrderEntity order);
        Task<bool> UpdateStatusAsync(string id, OrderStatus status);
        Task<bool> DeleteAsync(string id);
    }
}
