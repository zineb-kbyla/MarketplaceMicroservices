using Xunit;
using Moq;
using AutoMapper;
using Product.API.Application.DTOs;
using Product.API.Application.Interfaces;
using Product.API.Application.Services;
using ProductEntity = Product.API.Domain.Entities.Product;

namespace Product.API.Tests.Unit
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly Mock<IEventPublisher> _mockEventPublisher;
        private readonly IMapper _mapper;
        private readonly ProductService _productService;
        private readonly Mock<ILogger<ProductService>> _mockLogger;

        public ProductServiceTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _mockEventPublisher = new Mock<IEventPublisher>();
            _mockLogger = new Mock<ILogger<ProductService>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProductEntity, ProductDto>().ReverseMap();
                cfg.CreateMap<CreateProductDto, ProductEntity>();
            });
            _mapper = config.CreateMapper();

            _productService = new ProductService(
                _mockRepository.Object,
                _mockEventPublisher.Object,
                _mapper,
                _mockLogger.Object);
        }

        [Fact]
        public async Task GetProductsAsync_ShouldReturnListOfProducts()
        {
            // Arrange
            var products = new List<ProductEntity>
            {
                new ProductEntity { Id = "1", Name = "Product 1", Price = 100 },
                new ProductEntity { Id = "2", Name = "Product 2", Price = 200 }
            };
            _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _productService.GetProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithValidId_ShouldReturnProduct()
        {
            // Arrange
            var productId = "1";
            var product = new ProductEntity { Id = productId, Name = "Test Product", Price = 100 };
            _mockRepository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            Assert.Equal("Test Product", result.Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithInvalidId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var productId = "invalid";
            _mockRepository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync((ProductEntity)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _productService.GetProductByIdAsync(productId));
        }

        [Fact]
        public async Task CreateProductAsync_ShouldCreateProduct()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "New Product",
                Description = "Test",
                Category = "Electronics",
                Price = 150,
                Stock = 10,
                ImageUrl = "url"
            };

            var product = new ProductEntity
            {
                Id = "1",
                Name = createDto.Name,
                Price = createDto.Price,
                Stock = createDto.Stock
            };

            _mockRepository.Setup(x => x.CreateAsync(It.IsAny<ProductEntity>())).ReturnsAsync(product);
            _mockEventPublisher.Setup(x => x.PublishAsync(It.IsAny<object>())).Returns(Task.CompletedTask);

            // Act
            var result = await _productService.CreateProductAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Product", result.Name);
            _mockRepository.Verify(x => x.CreateAsync(It.IsAny<ProductEntity>()), Times.Once);
            _mockEventPublisher.Verify(x => x.PublishAsync(It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task DecrementStockAsync_WithSufficientStock_ShouldDecrementStock()
        {
            // Arrange
            var productId = "1";
            var product = new ProductEntity
            {
                Id = productId,
                Name = "Product",
                Stock = 10,
                Status = "Available"
            };

            _mockRepository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<ProductEntity>())).ReturnsAsync(true);
            _mockEventPublisher.Setup(x => x.PublishAsync(It.IsAny<object>())).Returns(Task.CompletedTask);

            // Act
            var result = await _productService.DecrementStockAsync(productId, 5);

            // Assert
            Assert.True(result);
            Assert.Equal(5, product.Stock);
            _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<ProductEntity>()), Times.Once);
        }

        [Fact]
        public async Task DecrementStockAsync_WithInsufficientStock_ShouldThrowException()
        {
            // Arrange
            var productId = "1";
            var product = new ProductEntity
            {
                Id = productId,
                Name = "Product",
                Stock = 5
            };

            _mockRepository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _productService.DecrementStockAsync(productId, 10));
        }
    }
}
