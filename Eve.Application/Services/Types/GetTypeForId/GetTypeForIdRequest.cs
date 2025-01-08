using Eve.Domain.Interfaces.ApiServices;

namespace Eve.Application.Services.Types.GetTypeForId;
public record GetTypeForIdRequest( int TypeId) : IRequest;