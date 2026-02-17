using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Order.API.Application.DTOs;
using Order.API.Domain.Enums;
using Order.API.Infrastructure.Data;
using Xunit;

namespace Order.API.Tests.Integration
{
    public class OrdersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly JsonSerializerOptions _jsonOptions;

        public OrdersControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            
            // Configure JSON options to match API configuration
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        [Fact]
        public async Task CreateOrder_ReturnsCreatedOrder()
        {
            // Arrange
            var createOrderDto = new CreateOrderDto
            {
                UserId = "integration-user-001",
                UserName = "Integration Test User",
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto
                    {
                        ProductId = "prod-int-001",
                        ProductName = "Integration Test Product",
                        Quantity = 2,
                        UnitPrice = 49.99m
                    }
                },
                ShippingAddress = new AddressDto
                {
                    Street = "456 Test Avenue",
                    City = "Test City",
                    State = "TC",
                    Country = "TestLand",
                    ZipCode = "12345",
                    PhoneNumber = "+1555000111"
                },
                PaymentInfo = new PaymentDto
                {
                    CardName = "Test User",
                    CardNumber = "4111111111111111",
                    Expiration = "12/27",
                    CVV = "456",
                    PaymentMethod = PaymentMethod.CreditCard
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/orders", createOrderDto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            var createdOrder = await response.Content.ReadFromJsonAsync<OrderDto>(_jsonOptions);
            Assert.NotNull(createdOrder);
            Assert.NotNull(createdOrder.Id);
            Assert.Equal("integration-user-001", createdOrder.UserId);
            Assert.Equal(OrderStatus.Pending, createdOrder.Status);
            Assert.Single(createdOrder.OrderItems);
            Assert.Equal(99.98m, createdOrder.TotalAmount); // 2 * 49.99
        }

        [Fact]
        public async Task GetOrderById_ReturnsOrder_WhenOrderExists()
        {
            // Arrange - First create an order
            var createOrderDto = new CreateOrderDto
            {
                UserId = "integration-user-002",
                UserName = "Test User 2",
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto
                    {
                        ProductId = "prod-int-002",
                        ProductName = "Test Product 2",
                        Quantity = 1,
                        UnitPrice = 29.99m
                    }
                },
                ShippingAddress = new AddressDto
                {
                    Street = "789 Test Street",
                    City = "Test Town",
                    State = "TT",
                    Country = "TestCountry",
                    ZipCode = "54321",
                    PhoneNumber = "+1555000222"
                },
                PaymentInfo = new PaymentDto
                {
                    CardName = "Test User 2",
                    CardNumber = "5555555555554444",
                    Expiration = "06/28",
                    CVV = "789",
                    PaymentMethod = PaymentMethod.DebitCard
                }
            };

            var createResponse = await _client.PostAsJsonAsync("/api/orders", createOrderDto);
            var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderDto>(_jsonOptions);

            // Act
            var getResponse = await _client.GetAsync($"/api/orders/{createdOrder!.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            
            var retrievedOrder = await getResponse.Content.ReadFromJsonAsync<OrderDto>(_jsonOptions);
            Assert.NotNull(retrievedOrder);
            Assert.Equal(createdOrder.Id, retrievedOrder.Id);
            Assert.Equal("integration-user-002", retrievedOrder.UserId);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            // Arrange
            var nonExistentOrderId = "507f1f77bcf86cd799439011";

            // Act
            var response = await _client.GetAsync($"/api/orders/{nonExistentOrderId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetOrdersByUserId_ReturnsUserOrders()
        {
            // Arrange - Create multiple orders for same user
            var userId = "integration-user-003";
            
            for (int i = 1; i <= 3; i++)
            {
                var createOrderDto = new CreateOrderDto
                {
                    UserId = userId,
                    UserName = "Test User 3",
                    Items = new List<OrderItemDto>
                    {
                        new OrderItemDto
                        {
                            ProductId = $"prod-int-00{i}",
                            ProductName = $"Test Product {i}",
                            Quantity = i,
                            UnitPrice = 10.00m * i
                        }
                    },
                    ShippingAddress = new AddressDto
                    {
                        Street = "123 Multi Order St",
                        City = "Order City",
                        State = "OC",
                        Country = "OrderLand",
                        ZipCode = "99999",
                        PhoneNumber = "+1555000333"
                    },
                    PaymentInfo = new PaymentDto
                    {
                        CardName = "Test User 3",
                        CardNumber = "378282246310005",
                        Expiration = "09/29",
                        CVV = "123",
                        PaymentMethod = PaymentMethod.CreditCard
                    }
                };

                await _client.PostAsJsonAsync("/api/orders", createOrderDto);
            }

            // Act
            var response = await _client.GetAsync($"/api/orders/user/{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>(_jsonOptions);
            Assert.NotNull(orders);
            Assert.True(orders.Count >= 3); // At least 3 orders
            Assert.All(orders, o => Assert.Equal(userId, o.UserId));
        }

        [Fact]
        public async Task UpdateOrderStatus_UpdatesSuccessfully()
        {
            // Arrange - Create an order first
            var createOrderDto = new CreateOrderDto
            {
                UserId = "integration-user-004",
                UserName = "Test User 4",
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto
                    {
                        ProductId = "prod-int-004",
                        ProductName = "Status Test Product",
                        Quantity = 1,
                        UnitPrice = 99.99m
                    }
                },
                ShippingAddress = new AddressDto
                {
                    Street = "Status Update Avenue",
                    City = "Update City",
                    State = "UC",
                    Country = "UpdateLand",
                    ZipCode = "88888",
                    PhoneNumber = "+1555000444"
                },
                PaymentInfo = new PaymentDto
                {
                    CardName = "Test User 4",
                    CardNumber = "6011111111111117",
                    Expiration = "03/30",
                    CVV = "321",
                    PaymentMethod = PaymentMethod.CreditCard
                }
            };

            var createResponse = await _client.PostAsJsonAsync("/api/orders", createOrderDto);
            var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderDto>(_jsonOptions);

            var updateStatusDto = new UpdateOrderStatusDto
            {
                Status = OrderStatus.Processing
            };

            // Act
            var updateResponse = await _client.PutAsJsonAsync(
                $"/api/orders/{createdOrder!.Id}/status", 
                updateStatusDto
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            
            // Verify the status was updated
            var getResponse = await _client.GetAsync($"/api/orders/{createdOrder.Id}");
            var updatedOrder = await getResponse.Content.ReadFromJsonAsync<OrderDto>(_jsonOptions);
            Assert.Equal(OrderStatus.Processing, updatedOrder!.Status);
        }

        [Fact]
        public async Task CancelOrder_CancelsSuccessfully()
        {
            // Arrange - Create an order first
            var createOrderDto = new CreateOrderDto
            {
                UserId = "integration-user-005",
                UserName = "Test User 5",
                Items = new List<OrderItemDto>
                {
                    new OrderItemDto
                    {
                        ProductId = "prod-int-005",
                        ProductName = "Cancel Test Product",
                        Quantity = 1,
                        UnitPrice = 59.99m
                    }
                },
                ShippingAddress = new AddressDto
                {
                    Street = "Cancel Street",
                    City = "Cancel City",
                    State = "CC",
                    Country = "CancelLand",
                    ZipCode = "77777",
                    PhoneNumber = "+1555000555"
                },
                PaymentInfo = new PaymentDto
                {
                    CardName = "Test User 5",
                    CardNumber = "3530111333300000",
                    Expiration = "11/31",
                    CVV = "987",
                    PaymentMethod = PaymentMethod.CreditCard
                }
            };

            var createResponse = await _client.PostAsJsonAsync("/api/orders", createOrderDto);
            var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderDto>(_jsonOptions);

            // Act
            var cancelResponse = await _client.DeleteAsync($"/api/orders/{createdOrder!.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, cancelResponse.StatusCode);
            
            // Verify the order was cancelled
            var getResponse = await _client.GetAsync($"/api/orders/{createdOrder.Id}");
            var cancelledOrder = await getResponse.Content.ReadFromJsonAsync<OrderDto>(_jsonOptions);
            Assert.Equal(OrderStatus.Cancelled, cancelledOrder!.Status);
        }

        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenInputIsInvalid()
        {
            // Arrange - Missing required fields
            var invalidOrderDto = new CreateOrderDto
            {
                UserId = "", // Invalid: empty
                UserName = "Test",
                Items = new List<OrderItemDto>() // Invalid: empty list
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/orders", invalidOrderDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
