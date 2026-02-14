using Microsoft.AspNetCore.Mvc;
using Product.API.Application.DTOs;
using Product.API.Application.Interfaces;

namespace Product.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            _logger.LogInformation("GET /api/categories");
            try
            {
                var categories = await _categoryService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                return StatusCode(500, new { message = "Error retrieving categories" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(string id)
        {
            _logger.LogInformation($"GET /api/categories/{id}");
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                return Ok(category);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Category {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting category {id}");
                return StatusCode(500, new { message = "Error retrieving category" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            _logger.LogInformation($"POST /api/categories - Creating category {dto.Name}");
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var category = await _categoryService.CreateCategoryAsync(dto);
                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, new { message = "Error creating category" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCategory(string id, [FromBody] CreateCategoryDto dto)
        {
            _logger.LogInformation($"PUT /api/categories/{id}");
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _categoryService.UpdateCategoryAsync(id, dto);
                if (!result)
                    return NotFound(new { message = $"Category {id} not found" });

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Category {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating category {id}");
                return StatusCode(500, new { message = "Error updating category" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(string id)
        {
            _logger.LogInformation($"DELETE /api/categories/{id}");
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (!result)
                    return NotFound(new { message = $"Category {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting category {id}");
                return StatusCode(500, new { message = "Error deleting category" });
            }
        }
    }
}
