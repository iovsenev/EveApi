using AutoMapper;
using Eve.Application.DTOs;
using Eve.Domain.Entities;

namespace Eve.Application.Mapping;
public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<CategoryEntity, CategoryDto>();
        CreateMap<GroupEntity, GroupDto>();
    }
}
