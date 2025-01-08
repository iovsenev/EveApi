using AutoMapper;
using Eve.Application.DTOs;
using Eve.Domain.Entities;
using Eve.Domain.Entities.Products;
using Eve.Domain.ExternalTypes;

namespace Eve.Application.Mapping;
public class GroupProfile : Profile
{
    public GroupProfile()
    {
        CreateMap<MarketGroupEntity, MarketGroupDto>()
            .ForMember(g => g.IconFileName, src => src.MapFrom(gr => gr.Icon.FileName));

        CreateMap<TypeEntity, ShortTypeDto>()
            .ForMember(t=>t.IconFileName, src => src.MapFrom(st => st.Icon.FileName)); 

        CreateMap<TypeEntity, TypeInfoDto>()
            .ForSourceMember(t => t.ReprocessComponents, config => config.DoNotValidate())
            .ForMember(t=> t.IconFileName, src => src.MapFrom(st => st.Icon.FileName));

        CreateMap<ProductEntity, ProductDto>()
            .ForSourceMember(t => t.Materials, config => config.DoNotValidate());

        CreateMap<ReprocessMaterialEntity, MaterialDto>()
            .ForMember(m => m.Name, src => src.MapFrom(me => me.Material.Name))
            .ForMember(m => m.TypeId, src => src.MapFrom(me => me.MaterialId));

        CreateMap<ProductMaterialEntity, MaterialDto>()
            .ForMember(m => m.Name, src => src.MapFrom(me => me.Type.Name))
            .ForMember(m => m.TypeId, src => src.MapFrom(me => me.TypeId));

        CreateMap<TypeOrdersInfo, TypeOrderDto>();

        CreateMap<StationEntity, StationNameDto>()
            .ForMember(dto => dto.Id, src => src.MapFrom(entity => entity.Id))
            .ForMember(dto => dto.Name, src => src.MapFrom(entity => entity.Name));
    }
}
