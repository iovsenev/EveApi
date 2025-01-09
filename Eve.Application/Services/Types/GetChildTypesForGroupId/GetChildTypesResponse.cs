using Eve.Application.DTOs;
using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.Services.Types.GetChildTypesForGroupId;

public record GetChildTypesResponse(ICollection<ShortTypeDto> Types);
