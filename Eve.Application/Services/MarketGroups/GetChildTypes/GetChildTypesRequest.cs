using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.Services.MarketGroups.GetChildTypes;
public record GetChildTypesRequest(
    int GroupId) : IRequest;
