using Eve.Application.DTOs;
using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.Services.MarketGroups.GetChildTypes;

public record GetChildTypesResponse(ICollection<ShortTypeDto> Types);