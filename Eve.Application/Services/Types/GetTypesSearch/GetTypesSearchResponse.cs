using Eve.Application.DTOs;
using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.Services.Types.GetTypesSearch;

public record GetTypesSearchResponse(
    ICollection<ShortTypeDto> Types);