using Eve.Application.DTOs;
using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.QueryServices.Types.GetTypeForId;

public record GetTypeForIdResponse(
    TypeInfoDto Type,
    double BestSumPriceMaterials);