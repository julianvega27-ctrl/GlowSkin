using AlaiaStore.Domain.Entities;
using AlaiaStore.Web.DTOs;
using AutoMapper;

namespace AlaiaStore.Web.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();

        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ReverseMap();

        CreateMap<User, UserDto>().ReverseMap();
    }
}
