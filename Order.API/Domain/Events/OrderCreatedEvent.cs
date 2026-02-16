using Order.API.Application.DTOs;

namespace Order.API.Domain.Events
{
    public class OrderCreatedEvent
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }
        public List<OrderItemDto> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
