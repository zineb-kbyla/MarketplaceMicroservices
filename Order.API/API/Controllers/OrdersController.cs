using Microsoft.AspNetCore.Mvc;
using Order.API.Application.DTOs;
using Order.API.Application.Interfaces;
using Order.API.Domain.Enums;

namespace Order.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Get all orders or filter by userId
        /// </summary>
        /// <param name="userId">Optional user ID to filter orders</param>
        /// <returns>List of orders</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<OrderDto>>> GetOrders([FromQuery] string userId = null)
        {
            try
            {
                var orders = await _orderService.GetOrdersAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                return StatusCode(500, new { message = "An error occurred while retrieving orders" });
            }
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> GetOrder(string id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                return Ok(order);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Order with ID {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order {id}");
                return StatusCode(500, new { message = "An error occurred while retrieving the order" });
            }
        }

        /// <summary>
        /// Get orders for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of user's orders</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<OrderDto>>> GetUserOrders(string userId)
        {
            try
            {
                var orders = await _orderService.GetOrdersAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving orders for user {userId}");
                return StatusCode(500, new { message = "An error occurred while retrieving user orders" });
            }
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="dto">Order creation data</param>
        /// <returns>Created order</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { message = "Order data is required" });
                }

                if (dto.Items == null || !dto.Items.Any())
                {
                    return BadRequest(new { message = "Order must contain at least one item" });
                }

                var createdOrder = await _orderService.CreateOrderAsync(dto);
                return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, new { message = "An error occurred while creating the order" });
            }
        }

        /// <summary>
        /// Update order status
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="dto">Status update data</param>
        /// <returns>Success status</returns>
        [HttpPut("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateOrderStatus(string id, [FromBody] UpdateOrderStatusDto dto)
        {
            try
            {
                var success = await _orderService.UpdateOrderStatusAsync(id, dto.Status);

                if (!success)
                {
                    return NotFound(new { message = $"Order with ID {id} not found" });
                }

                return Ok(new { message = "Order status updated successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Order with ID {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order status {id}");
                return StatusCode(500, new { message = "An error occurred while updating order status" });
            }
        }

        /// <summary>
        /// Cancel an order
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CancelOrder(string id)
        {
            try
            {
                var success = await _orderService.CancelOrderAsync(id);

                if (!success)
                {
                    return NotFound(new { message = $"Order with ID {id} not found" });
                }

                return Ok(new { message = "Order cancelled successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Order with ID {id} not found" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling order {id}");
                return StatusCode(500, new { message = "An error occurred while cancelling the order" });
            }
        }

        /// <summary>
        /// Get order tracking information
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order tracking details</returns>
        [HttpGet("{id}/tracking")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetOrderTracking(string id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);

                var tracking = new
                {
                    orderId = order.Id,
                    orderNumber = order.OrderNumber,
                    status = order.Status.ToString(),
                    createdAt = order.CreatedAt,
                    updatedAt = order.UpdatedAt,
                    estimatedDelivery = CalculateEstimatedDelivery(order.Status, order.CreatedAt)
                };

                return Ok(tracking);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Order with ID {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order tracking {id}");
                return StatusCode(500, new { message = "An error occurred while retrieving order tracking" });
            }
        }

        private DateTime? CalculateEstimatedDelivery(OrderStatus status, DateTime createdAt)
        {
            return status switch
            {
                OrderStatus.Pending => createdAt.AddDays(7),
                OrderStatus.Confirmed => createdAt.AddDays(6),
                OrderStatus.Processing => createdAt.AddDays(5),
                OrderStatus.Shipped => createdAt.AddDays(3),
                OrderStatus.Delivered => null,
                OrderStatus.Cancelled => null,
                OrderStatus.Refunded => null,
                _ => createdAt.AddDays(7)
            };
        }
    }
}
