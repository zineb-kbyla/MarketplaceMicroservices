using AutoMapper;
using Product.API.Application.DTOs;
using ProductEntity = Product.API.Domain.Entities.Product;
using CategoryEntity = Product.API.Domain.Entities.Category;

namespace Product.API.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product mappings
            CreateMap<ProductEntity, ProductDto>().ReverseMap();
            CreateMap<CreateProductDto, ProductEntity>();
            CreateMap<UpdateProductDto, ProductEntity>();

            // Category mappings
            CreateMap<CategoryEntity, CategoryDto>().ReverseMap();
            CreateMap<CreateCategoryDto, CategoryEntity>();
        }
    }
}
