using Eve.Domain.Common;
using Eve.Domain.Interfaces.ApiServices;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Windows.Input;

namespace Eve.Application.Services;
public class QueryHandler : IQueryHandler
{
    private readonly IServiceProvider _provider;
    private static readonly ConcurrentDictionary<Type, RequestHandlerBase> _requestHandlers = new();

    public QueryHandler(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<Result<TResponse>> Send<TResponse>(IRequest request, CancellationToken token = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var handler = (RequestHandlerWrapper<TResponse>)_requestHandlers.GetOrAdd(request.GetType(), requestType =>
        {
            var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResponse));
            var wrapper = Activator.CreateInstance(wrapperType)
                ?? throw new InvalidOperationException($"Could not create wrapper type for {requestType}");
            return (RequestHandlerBase)wrapper;
        });

        return await handler.HandleAsync(request, _provider, token);
    }
}

public abstract class RequestHandlerBase
{
    public abstract Task<Result<object?>> HandleAsync(object request, IServiceProvider provider, CancellationToken token);
}

public abstract class RequestHandlerWrapper<TResponse> : RequestHandlerBase
{
    public abstract Task<Result<TResponse>> HandleAsync(IRequest request, IServiceProvider provider, CancellationToken token);
}

public class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse> where TRequest : IRequest
{
    public override async Task<Result<object?>> HandleAsync(object request, IServiceProvider provider, CancellationToken token) =>
        await HandleAsync((TRequest)request, provider, token).ConfigureAwait(false);
    public override Task<Result<TResponse>> HandleAsync(IRequest request, IServiceProvider provider, CancellationToken token)
    {
        var service = provider.GetRequiredService<IRequestHandler<TResponse, TRequest>>();
        Task<Result<TResponse>> Handler() => service
            .Handle((TRequest)request, token);
        var result = Handler();
        return result;
    }
}
