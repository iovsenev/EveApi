using AutoMapper;
using Eve.Application.DTOs;
using Eve.Domain.Entities.Products;

namespace Eve.Application.Mapping;
public class ProductionProfile : Profile
{
    public ProductionProfile()
    {
        CreateMap<ProductEntity, ProductDto>()
            .ForSourceMember(t => t.Materials, config => config.DoNotValidate());

        CreateMap<ProductMaterialEntity, MaterialDto>()
            .ForMember(m => m.Name, src => src.MapFrom(me => me.Type.Name))
            .ForMember(m => m.TypeId, src => src.MapFrom(me => me.TypeId));
    }
}
