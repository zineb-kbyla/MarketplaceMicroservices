using AutoMapper;
using Order.API.Application.DTOs;
using Order.API.Domain.Entities;
using OrderEntity = Order.API.Domain.Entities.Order;

namespace Order.API.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Order mappings
            CreateMap<OrderEntity, OrderDto>().ReverseMap();
            CreateMap<CreateOrderDto, OrderEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.OrderNumber, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.OrderStatus.Pending))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // OrderItem mappings
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();

            // Address mappings
            CreateMap<Address, AddressDto>().ReverseMap();

            // Payment mappings
            CreateMap<Payment, PaymentDto>().ReverseMap();
        }
    }
}
