using Eve.Application.DTOs;

namespace Eve.Application.QueryServices.Category.GetCategories;

public record GetCategoriesResponse(ICollection<CategoryDto> categories);