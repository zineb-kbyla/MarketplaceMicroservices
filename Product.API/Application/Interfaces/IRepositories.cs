using Product.API.Application.DTOs;
using ProductEntity = Product.API.Domain.Entities.Product;
using CategoryEntity = Product.API.Domain.Entities.Category;

namespace Product.API.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<List<ProductEntity>> GetAllAsync();
        Task<ProductEntity> GetByIdAsync(string id);
        Task<List<ProductEntity>> GetByCategoryAsync(string category);
        Task<ProductEntity> CreateAsync(ProductEntity product);
        Task<bool> UpdateAsync(ProductEntity product);
        Task<bool> DeleteAsync(string id);
        Task<List<ProductEntity>> SearchAsync(string query);
    }

    public interface IProductService
    {
        Task<List<ProductDto>> GetProductsAsync();
        Task<ProductDto> GetProductByIdAsync(string id);
        Task<List<ProductDto>> GetProductsByCategoryAsync(string category);
        Task<ProductDto> CreateProductAsync(CreateProductDto dto);
        Task<bool> UpdateProductAsync(string id, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(string id);
        Task<List<ProductDto>> SearchProductsAsync(string query);
        Task<bool> DecrementStockAsync(string id, int quantity);
    }

    public interface ICategoryRepository
    {
        Task<List<CategoryEntity>> GetAllAsync();
        Task<CategoryEntity> GetByIdAsync(string id);
        Task<CategoryEntity> CreateAsync(CategoryEntity category);
        Task<bool> UpdateAsync(CategoryEntity category);
        Task<bool> DeleteAsync(string id);
    }

    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(string id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<bool> UpdateCategoryAsync(string id, CreateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(string id);
    }

    public interface IEventPublisher
    {
        Task PublishAsync<T>(T @event) where T : class;
    }
}
