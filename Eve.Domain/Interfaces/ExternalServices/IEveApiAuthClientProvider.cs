using Eve.Domain.Common;
using Eve.Domain.ExternalTypes;

namespace Eve.Domain.Interfaces.ExternalServices;

public interface IEveApiAuthClientProvider
{
    Task<Result<TokenResponse>> ExchangeCodeForTokenAsync(string code, CancellationToken token);
    Task<Result<JwksMetadata>> FetchJwksMetadataAsync(CancellationToken token);
}