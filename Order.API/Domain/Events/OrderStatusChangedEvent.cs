using Order.API.Domain.Enums;

namespace Order.API.Domain.Events
{
    public class OrderStatusChangedEvent
    {
        public string OrderId { get; set; }
        public OrderStatus OldStatus { get; set; }
        public OrderStatus NewStatus { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
