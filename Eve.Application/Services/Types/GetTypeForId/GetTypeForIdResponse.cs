using Eve.Application.DTOs;
using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.Services.Types.GetTypeForId;

public record GetTypeForIdResponse(
    TypeInfoDto Type, 
    double BestSumPriceMaterials);