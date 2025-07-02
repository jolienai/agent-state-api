using AgentState.Application;

namespace AgentState.Infrastructure.Shared;

// this can be abstract to a new project to be reused as a package
public class Mediator(IServiceProvider provider) : IMediator
{
    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        dynamic handler = provider.GetService(handlerType)!;

        if (handler == null)
            throw new InvalidOperationException($"Handler for {request.GetType().Name} not found");

        return await handler.HandleAsync((dynamic)request, cancellationToken);
    }
}