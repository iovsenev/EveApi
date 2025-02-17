using Eve.Application.DTOs;
using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.QueryServices.Types.GetChildTypesForMarketGroup;

public record GetChildTypesResponse(ICollection<ShortTypeDto> Types);
