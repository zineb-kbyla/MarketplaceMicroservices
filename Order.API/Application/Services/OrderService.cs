using AutoMapper;
using Order.API.Application.DTOs;
using Order.API.Application.Interfaces;
using Order.API.Domain.Entities;
using Order.API.Domain.Enums;
using Order.API.Domain.Events;
using OrderEntity = Order.API.Domain.Entities.Order;

namespace Order.API.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IEventPublisher eventPublisher,
            IMapper mapper,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _eventPublisher = eventPublisher;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
        {
            try
            {
                // Map DTO to entity
                var order = _mapper.Map<OrderEntity>(dto);

                // Generate order number
                order.OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

                // Calculate totals for items
                foreach (var item in order.OrderItems)
                {
                    item.TotalPrice = item.CalculateItemTotal();
                }

                // Calculate total amount
                order.TotalAmount = order.CalculateTotal();

                // Set initial status
                order.Status = OrderStatus.Pending;

                // Mask sensitive payment info
                if (order.PaymentInfo != null && !string.IsNullOrEmpty(order.PaymentInfo.CardNumber))
                {
                    order.PaymentInfo.CardNumber = MaskCardNumber(order.PaymentInfo.CardNumber);
                    order.PaymentInfo.CVV = "***";
                }

                // Create order in database
                var createdOrder = await _orderRepository.CreateAsync(order);

                // Publish OrderCreatedEvent
                var orderCreatedEvent = new OrderCreatedEvent
                {
                    OrderId = createdOrder.Id,
                    UserId = createdOrder.UserId,
                    Items = _mapper.Map<List<OrderItemDto>>(createdOrder.OrderItems),
                    TotalAmount = createdOrder.TotalAmount,
                    CreatedAt = createdOrder.CreatedAt
                };

                await _eventPublisher.PublishAsync(orderCreatedEvent);

                _logger.LogInformation($"Order created: {createdOrder.OrderNumber}");

                return _mapper.Map<OrderDto>(createdOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                throw;
            }
        }

        public async Task<List<OrderDto>> GetOrdersAsync(string userId = null)
        {
            try
            {
                List<OrderEntity> orders;

                if (!string.IsNullOrEmpty(userId))
                {
                    orders = await _orderRepository.GetByUserIdAsync(userId);
                }
                else
                {
                    orders = await _orderRepository.GetAllAsync();
                }

                return _mapper.Map<List<OrderDto>>(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                throw;
            }
        }

        public async Task<OrderDto> GetOrderByIdAsync(string id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);

                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {id} not found");
                }

                return _mapper.Map<OrderDto>(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order {id}");
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(string id, OrderStatus newStatus)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);

                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {id} not found");
                }

                var oldStatus = order.Status;
                order.UpdateStatus(newStatus);

                var result = await _orderRepository.UpdateAsync(order);

                if (result)
                {
                    // Publish OrderStatusChangedEvent
                    var statusChangedEvent = new OrderStatusChangedEvent
                    {
                        OrderId = order.Id,
                        OldStatus = oldStatus,
                        NewStatus = newStatus,
                        ChangedAt = DateTime.UtcNow
                    };

                    await _eventPublisher.PublishAsync(statusChangedEvent);

                    _logger.LogInformation($"Order {order.OrderNumber} status changed from {oldStatus} to {newStatus}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order status for {id}");
                throw;
            }
        }

        public async Task<bool> CancelOrderAsync(string id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);

                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {id} not found");
                }

                if (!order.CanBeCancelled())
                {
                    throw new InvalidOperationException($"Order {order.OrderNumber} cannot be cancelled in status {order.Status}");
                }

                order.UpdateStatus(OrderStatus.Cancelled);
                var result = await _orderRepository.UpdateAsync(order);

                if (result)
                {
                    // Publish OrderCancelledEvent
                    var cancelledEvent = new OrderCancelledEvent
                    {
                        OrderId = order.Id,
                        Reason = "Cancelled by user",
                        CancelledAt = DateTime.UtcNow
                    };

                    await _eventPublisher.PublishAsync(cancelledEvent);

                    _logger.LogInformation($"Order {order.OrderNumber} cancelled");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling order {id}");
                throw;
            }
        }

        private string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 4)
                return "****";

            return "****" + cardNumber.Substring(cardNumber.Length - 4);
        }
    }
}
