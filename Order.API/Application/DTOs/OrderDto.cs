using System.ComponentModel.DataAnnotations;
using Order.API.Domain.Enums;

namespace Order.API.Application.DTOs
{
    public class OrderDto
    {
        public string Id { get; set; }
        public string OrderNumber { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
        public AddressDto ShippingAddress { get; set; }
        public PaymentDto PaymentInfo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class OrderItemDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class AddressDto
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class PaymentDto
    {
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string CVV { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }

    public class CreateOrderDto
    {
        [Required(ErrorMessage = "UserId is required")]
        [MinLength(1, ErrorMessage = "UserId cannot be empty")]
        public string UserId { get; set; }
        
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }
        
        [Required(ErrorMessage = "Items are required")]
        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<OrderItemDto> Items { get; set; }
        
        [Required(ErrorMessage = "Shipping address is required")]
        public AddressDto ShippingAddress { get; set; }
        
        [Required(ErrorMessage = "Payment info is required")]
        public PaymentDto PaymentInfo { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
    }
}
