using Xunit;
using Moq;
using Order.API.Application.Services;
using Order.API.Application.Interfaces;
using Order.API.Application.DTOs;
using Order.API.Domain.Entities;
using Order.API.Domain.Enums;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Order.API.Tests.Unit
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockRepository;
        private readonly Mock<IEventPublisher> _mockEventPublisher;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<OrderService>> _mockLogger;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockRepository = new Mock<IOrderRepository>();
            _mockEventPublisher = new Mock<IEventPublisher>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<OrderService>>();

            _orderService = new OrderService(
                _mockRepository.Object,
                _mockEventPublisher.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task CreateOrderAsync_ShouldReturnOrderDto()
        {
            // Arrange
            var createOrderDto = new CreateOrderDto
            {
                UserId = "user123",
                UserName = "John Doe",
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto
                    {
                        ProductId = "prod001",
                        ProductName = "Test Product",
                        Quantity = 1,
                        UnitPrice = 99.99m
                    }
                },
                ShippingAddress = new AddressDto
                {
                    Street = "123 Main St",
                    City = "New York",
                    State = "NY",
                    Country = "USA",
                    ZipCode = "10001",
                    PhoneNumber = "+1234567890"
                },
                PaymentInfo = new PaymentDto
                {
                    CardName = "John Doe",
                    CardNumber = "1234567890123456",
                    Expiration = "12/26",
                    CVV = "123",
                    PaymentMethod = PaymentMethod.CreditCard
                }
            };

            var order = new Domain.Entities.Order
            {
                Id = "order123",
                UserId = "user123",
                UserName = "John Doe",
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = "prod001",
                        ProductName = "Test Product",
                        Quantity = 1,
                        UnitPrice = 99.99m,
                        TotalPrice = 99.99m
                    }
                }
            };

            var orderDto = new OrderDto { Id = "order123" };

            _mockMapper.Setup(m => m.Map<Domain.Entities.Order>(createOrderDto)).Returns(order);
            _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.Order>())).ReturnsAsync(order);
            _mockMapper.Setup(m => m.Map<OrderDto>(order)).Returns(orderDto);

            // Act
            var result = await _orderService.CreateOrderAsync(createOrderDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("order123", result.Id);
            _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Domain.Entities.Order>()), Times.Once);
            _mockEventPublisher.Verify(e => e.PublishAsync(It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnOrderDto_WhenOrderExists()
        {
            // Arrange
            var orderId = "order123";
            var order = new Domain.Entities.Order { Id = orderId };
            var orderDto = new OrderDto { Id = orderId };

            _mockRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
            _mockMapper.Setup(m => m.Map<OrderDto>(order)).Returns(orderDto);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.Id);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldThrowException_WhenOrderNotFound()
        {
            // Arrange
            var orderId = "nonexistent";
            _mockRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Domain.Entities.Order)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _orderService.GetOrderByIdAsync(orderId));
        }

        [Fact]
        public async Task CancelOrderAsync_ShouldReturnTrue_WhenOrderCanBeCancelled()
        {
            // Arrange
            var orderId = "order123";
            var order = new Domain.Entities.Order
            {
                Id = orderId,
                Status = OrderStatus.Pending
            };

            _mockRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Domain.Entities.Order>())).ReturnsAsync(true);

            // Act
            var result = await _orderService.CancelOrderAsync(orderId);

            // Assert
            Assert.True(result);
            _mockEventPublisher.Verify(e => e.PublishAsync(It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task CancelOrderAsync_ShouldThrowException_WhenOrderCannotBeCancelled()
        {
            // Arrange
            var orderId = "order123";
            var order = new Domain.Entities.Order
            {
                Id = orderId,
                Status = OrderStatus.Delivered
            };

            _mockRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _orderService.CancelOrderAsync(orderId));
        }
    }
}
