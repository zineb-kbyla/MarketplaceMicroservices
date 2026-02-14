using Microsoft.AspNetCore.Mvc;
using Product.API.Application.DTOs;
using Product.API.Application.Interfaces;

namespace Product.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> GetProducts()
        {
            _logger.LogInformation("GET /api/products");
            try
            {
                var products = await _productService.GetProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return StatusCode(500, new { message = "Error retrieving products" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(string id)
        {
            _logger.LogInformation($"GET /api/products/{id}");
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Product {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product {id}");
                return StatusCode(500, new { message = "Error retrieving product" });
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<List<ProductDto>>> GetProductsByCategory(string category)
        {
            _logger.LogInformation($"GET /api/products/category/{category}");
            try
            {
                var products = await _productService.GetProductsByCategoryAsync(category);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting products for category {category}");
                return StatusCode(500, new { message = "Error retrieving products" });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<ProductDto>>> SearchProducts([FromQuery] string q)
        {
            _logger.LogInformation($"GET /api/products/search?q={q}");
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                    return BadRequest(new { message = "Search query cannot be empty" });

                var products = await _productService.SearchProductsAsync(q);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching products");
                return StatusCode(500, new { message = "Error searching products" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto dto)
        {
            _logger.LogInformation($"POST /api/products - Creating product {dto.Name}");
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var product = await _productService.CreateProductAsync(dto);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, new { message = "Error creating product" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(string id, [FromBody] UpdateProductDto dto)
        {
            _logger.LogInformation($"PUT /api/products/{id}");
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _productService.UpdateProductAsync(id, dto);
                if (!result)
                    return NotFound(new { message = $"Product {id} not found" });

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Product {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product {id}");
                return StatusCode(500, new { message = "Error updating product" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(string id)
        {
            _logger.LogInformation($"DELETE /api/products/{id}");
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (!result)
                    return NotFound(new { message = $"Product {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product {id}");
                return StatusCode(500, new { message = "Error deleting product" });
            }
        }

        [HttpPost("{id}/decrement-stock")]
        public async Task<ActionResult> DecrementStock(string id, [FromBody] DecrementStockDto dto)
        {
            _logger.LogInformation($"POST /api/products/{id}/decrement-stock");
            try
            {
                if (dto == null)
                    return BadRequest(new { message = "Request body is required" });

                if (dto.Quantity <= 0)
                    return BadRequest(new { message = "Quantity must be greater than 0" });

                var result = await _productService.DecrementStockAsync(id, dto.Quantity);
                if (!result)
                    return NotFound(new { message = $"Product {id} not found" });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Product {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error decrementing stock for product {id}");
                return StatusCode(500, new { message = "Error updating stock" });
            }
        }
    }
}
