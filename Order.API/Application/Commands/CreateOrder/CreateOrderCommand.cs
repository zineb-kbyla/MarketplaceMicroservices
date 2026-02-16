using Order.API.Application.DTOs;

namespace Order.API.Application.Commands.CreateOrder
{
    public class CreateOrderCommand
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<OrderItemDto> Items { get; set; }
        public AddressDto ShippingAddress { get; set; }
        public PaymentDto PaymentInfo { get; set; }
    }
}
