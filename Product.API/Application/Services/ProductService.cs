using AutoMapper;
using Product.API.Application.DTOs;
using Product.API.Application.Interfaces;
using ProductEntity = Product.API.Domain.Entities.Product;
using CategoryEntity = Product.API.Domain.Entities.Category;
using Product.API.Domain.Events;

namespace Product.API.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository repository,
            IEventPublisher eventPublisher,
            IMapper mapper,
            ILogger<ProductService> logger)
        {
            _repository = repository;
            _eventPublisher = eventPublisher;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            _logger.LogInformation("Getting all products");
            var products = await _repository.GetAllAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<ProductDto> GetProductByIdAsync(string id)
        {
            _logger.LogInformation($"Getting product {id}");
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Product {id} not found");

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<List<ProductDto>> GetProductsByCategoryAsync(string category)
        {
            _logger.LogInformation($"Getting products for category {category}");
            var products = await _repository.GetByCategoryAsync(category);
            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            _logger.LogInformation($"Creating product {dto.Name}");

            var product = new ProductEntity
            {
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = dto.ImageUrl,
                Rating = 0,
                ReviewCount = 0,
                Status = "Available"
            };

            var createdProduct = await _repository.CreateAsync(product);

            // Publish event
            var @event = new ProductCreatedEvent
            {
                ProductId = createdProduct.Id,
                Name = createdProduct.Name,
                Category = createdProduct.Category,
                Price = createdProduct.Price,
                CreatedAt = createdProduct.CreatedAt
            };
            await _eventPublisher.PublishAsync(@event);

            return _mapper.Map<ProductDto>(createdProduct);
        }

        public async Task<bool> UpdateProductAsync(string id, UpdateProductDto dto)
        {
            _logger.LogInformation($"Updating product {id}");

            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Product {id} not found");

            product.Name = dto.Name ?? product.Name;
            product.Description = dto.Description ?? product.Description;
            product.Category = dto.Category ?? product.Category;
            product.Price = dto.Price > 0 ? dto.Price : product.Price;
            product.Stock = dto.Stock > 0 ? dto.Stock : product.Stock;
            product.ImageUrl = dto.ImageUrl ?? product.ImageUrl;

            var result = await _repository.UpdateAsync(product);

            // Publish event
            if (result)
            {
                var @event = new ProductUpdatedEvent
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    UpdatedAt = product.UpdatedAt
                };
                await _eventPublisher.PublishAsync(@event);
            }

            return result;
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            _logger.LogInformation($"Deleting product {id}");
            return await _repository.DeleteAsync(id);
        }

        public async Task<List<ProductDto>> SearchProductsAsync(string query)
        {
            _logger.LogInformation($"Searching products with query: {query}");
            var products = await _repository.SearchAsync(query);
            return _mapper.Map<List<ProductDto>>(products);
        }

        public async Task<bool> DecrementStockAsync(string id, int quantity)
        {
            _logger.LogInformation($"Decrementing stock for product {id} by {quantity}");

            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Product {id} not found");

            var oldStock = product.Stock;
            product.DecrementStock(quantity);

            var result = await _repository.UpdateAsync(product);

            // Publish event
            if (result)
            {
                var @event = new StockUpdatedEvent
                {
                    ProductId = product.Id,
                    OldStock = oldStock,
                    NewStock = product.Stock,
                    UpdatedAt = product.UpdatedAt
                };
                await _eventPublisher.PublishAsync(@event);
            }

            return result;
        }
    }

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(
            ICategoryRepository repository,
            IMapper mapper,
            ILogger<CategoryService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            _logger.LogInformation("Getting all categories");
            var categories = await _repository.GetAllAsync();
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(string id)
        {
            _logger.LogInformation($"Getting category {id}");
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException($"Category {id} not found");

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            _logger.LogInformation($"Creating category {dto.Name}");

            var category = new CategoryEntity
            {
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                ProductCount = 0
            };

            var createdCategory = await _repository.CreateAsync(category);
            return _mapper.Map<CategoryDto>(createdCategory);
        }

        public async Task<bool> UpdateCategoryAsync(string id, CreateCategoryDto dto)
        {
            _logger.LogInformation($"Updating category {id}");

            var category = await _repository.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException($"Category {id} not found");

            category.Name = dto.Name;
            category.Description = dto.Description;
            category.ImageUrl = dto.ImageUrl;

            return await _repository.UpdateAsync(category);
        }

        public async Task<bool> DeleteCategoryAsync(string id)
        {
            _logger.LogInformation($"Deleting category {id}");
            return await _repository.DeleteAsync(id);
        }
    }
}
