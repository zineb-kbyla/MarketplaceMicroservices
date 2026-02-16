using Order.API.Domain.Enums;

namespace Order.API.Application.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommand
    {
        public string OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
    }
}
