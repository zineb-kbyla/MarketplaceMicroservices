using Order.API.Application.DTOs;
using Order.API.Domain.Enums;

namespace Order.API.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);
        Task<List<OrderDto>> GetOrdersAsync(string userId = null);
        Task<OrderDto> GetOrderByIdAsync(string id);
        Task<bool> UpdateOrderStatusAsync(string id, OrderStatus status);
        Task<bool> CancelOrderAsync(string id);
    }
}
