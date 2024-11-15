using App.Core;
using AutoMapper;

namespace App.Infrastructure;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductEntity, Product>()
            .ForMember(
                dest => dest.Name, 
                opt => opt.MapFrom(src => src.ProductName));

        CreateMap<Product, ProductEntity>()
            .ForMember(
                dest => dest.ProductName, 
                opt => opt.MapFrom(src => src.Name));
    }
}