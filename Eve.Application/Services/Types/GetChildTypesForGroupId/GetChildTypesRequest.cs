using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.Services.Types.GetChildTypesForGroupId;
public record GetChildTypesRequest(
    int GroupId) : IRequest;
