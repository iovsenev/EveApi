using Eve.Application.DTOs;

namespace Eve.Application.QueryServices.Category.GetGroupsForCategoryId;

public record GetGroupsForCategoryIdResponse(ICollection<GroupDto> groups);