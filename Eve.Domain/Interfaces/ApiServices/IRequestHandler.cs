using Eve.Domain.Common;

namespace Eve.Domain.Interfaces.ApiServices;
public interface IRequestHandler<TResponse, TRequest> where TRequest : IRequest
{
    Task<Result<TResponse>> Handle(TRequest request, CancellationToken token);
}