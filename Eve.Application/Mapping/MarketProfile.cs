using AutoMapper;
using Eve.Application.DTOs;
using Eve.Domain.Entities;
using Eve.Domain.Entities.Products;
using Eve.Domain.ExternalTypes;

namespace Eve.Application.Mapping;
public class MarketProfile : Profile
{
    public MarketProfile()
    {
        CreateMap<MarketGroupEntity, MarketGroupDto>()
            .ForMember(g => g.IconFileName, src => src.MapFrom(gr => gr.Icon.FileName));

        CreateMap<TypeOrdersInfo, TypeOrderDto>();

        CreateMap<StationEntity, StationNameDto>()
            .ForMember(dto => dto.Id, src => src.MapFrom(entity => entity.Id))
            .ForMember(dto => dto.Name, src => src.MapFrom(entity => entity.Name));
    }
}
