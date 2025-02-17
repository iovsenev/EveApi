using Eve.Application.DTOs;

namespace Eve.Application.QueryServices.Types.GetChildTypesForGroup;

public record GetChildTypesProductResponse(ICollection<ShortTypeDto> Types);