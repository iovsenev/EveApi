using Eve.Application.DTOs;

namespace Eve.Application.Services.Types.GetChildTypesForGroupId;

public record GetChildTypesProductResponse(ICollection<ShortTypeDto> Types);