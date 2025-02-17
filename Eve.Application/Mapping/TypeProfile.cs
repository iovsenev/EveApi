using AutoMapper;
using Eve.Application.DTOs;
using Eve.Domain.Entities;

namespace Eve.Application.Mapping;
public class TypeProfile : Profile
{
    public TypeProfile()
    {
        CreateMap<TypeEntity, ShortTypeDto>()
            .ForMember(t => t.IconFileName, src => src.MapFrom(st => st.Icon.FileName));

        CreateMap<TypeEntity, TypeInfoDto>()
            .ForSourceMember(t => t.ReprocessComponents, config => config.DoNotValidate())
            .ForMember(t => t.IconFileName, src => src.MapFrom(st => st.Icon.FileName));

        CreateMap<ReprocessMaterialEntity, MaterialDto>()
            .ForMember(m => m.Name, src => src.MapFrom(me => me.Material.Name))
            .ForMember(m => m.TypeId, src => src.MapFrom(me => me.MaterialId));
    }
}
