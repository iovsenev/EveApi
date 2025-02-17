using Eve.Domain.Common;

namespace Eve.Application.InternalServices.TokenService;
public interface IEsiTokenService
{
    Task<Result<ResponseTokenData>> GetAccessTokenAsync(RequestData data, CancellationToken token);
}