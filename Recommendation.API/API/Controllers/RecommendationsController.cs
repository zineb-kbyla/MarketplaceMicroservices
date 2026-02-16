using Microsoft.AspNetCore.Mvc;
using Recommendation.API.Application.DTOs;
using Recommendation.API.Application.Interfaces;

namespace Recommendation.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationsController : ControllerBase
    {
        private readonly IRecommendationService _service;
        private readonly ILogger<RecommendationsController> _logger;

        public RecommendationsController(
            IRecommendationService service,
            ILogger<RecommendationsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<List<RecommendedProductDto>>> GetPersonalizedRecommendations(
            string userId, [FromQuery] int limit = 10)
        {
            _logger.LogInformation("GET /api/recommendations/{UserId}?limit={Limit}", userId, limit);

            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return BadRequest(new { message = "UserId cannot be empty" });

                var recommendations = await _service.GetPersonalizedRecommendationsAsync(userId, limit);
                return Ok(recommendations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting personalized recommendations for user {UserId}", userId);
                return StatusCode(500, new { message = "Error retrieving recommendations" });
            }
        }

        [HttpGet("similar/{productId}")]
        public async Task<ActionResult<List<SimilarProductDto>>> GetSimilarProducts(
            string productId, [FromQuery] int limit = 5)
        {
            _logger.LogInformation("GET /api/recommendations/similar/{ProductId}?limit={Limit}", productId, limit);

            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                    return BadRequest(new { message = "ProductId cannot be empty" });

                var similar = await _service.GetSimilarProductsAsync(productId, limit);
                return Ok(similar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting similar products for {ProductId}", productId);
                return StatusCode(500, new { message = "Error retrieving similar products" });
            }
        }

        [HttpGet("trending")]
        public async Task<ActionResult<List<TrendingProductDto>>> GetTrendingProducts(
            [FromQuery] int days = 7, [FromQuery] int limit = 10)
        {
            _logger.LogInformation("GET /api/recommendations/trending?days={Days}&limit={Limit}", days, limit);

            try
            {
                var trending = await _service.GetTrendingProductsAsync(days, limit);
                return Ok(trending);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trending products");
                return StatusCode(500, new { message = "Error retrieving trending products" });
            }
        }

        [HttpGet("history/{userId}")]
        public async Task<ActionResult<List<UserHistoryDto>>> GetUserHistory(
            string userId, [FromQuery] int limit = 20)
        {
            _logger.LogInformation("GET /api/recommendations/history/{UserId}?limit={Limit}", userId, limit);

            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return BadRequest(new { message = "UserId cannot be empty" });

                var history = await _service.GetUserHistoryAsync(userId, limit);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting history for user {UserId}", userId);
                return StatusCode(500, new { message = "Error retrieving user history" });
            }
        }

        [HttpPost("view")]
        public async Task<ActionResult> RecordProductView([FromBody] RecordViewDto dto)
        {
            _logger.LogInformation("POST /api/recommendations/view - User {UserId} viewed product {ProductId}",
                dto.UserId, dto.ProductId);

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _service.RecordViewAsync(dto.UserId, dto.ProductId, dto.Duration, dto.Source);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording product view");
                return StatusCode(500, new { message = "Error recording view" });
            }
        }

        [HttpPost("purchase")]
        public async Task<ActionResult> RecordPurchase([FromBody] RecordPurchaseDto dto)
        {
            _logger.LogInformation("POST /api/recommendations/purchase - Recording purchase for order {OrderId}",
                dto.OrderId);

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _service.RecordPurchaseAsync(dto.UserId, dto.OrderId, dto.Items);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording purchase for order {OrderId}", dto.OrderId);
                return StatusCode(500, new { message = "Error recording purchase" });
            }
        }
    }
}
