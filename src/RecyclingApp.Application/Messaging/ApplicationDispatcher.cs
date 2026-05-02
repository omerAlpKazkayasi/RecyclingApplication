using Microsoft.Extensions.DependencyInjection;

namespace RecyclingApp.Application.Abstractions.Messaging;

public class ApplicationDispatcher : IApplicationDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public ApplicationDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        // Resolve Handler
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        var handler = _serviceProvider.GetService(handlerType);

        if (handler == null)
        {
            throw new InvalidOperationException($"No handler registered for {requestType.Name}");
        }

        // Resolve Behaviors
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var behaviors = _serviceProvider.GetServices(behaviorType)
            .Cast<object>()
            .ToList();

        // Create the handler delegate
        var handleMethod = handlerType.GetMethod("HandleAsync");
        if (handleMethod == null) throw new InvalidOperationException($"HandleAsync method not found on {handlerType.Name}");

        RequestHandlerDelegate<TResponse> handlerDelegate = () => 
            (Task<TResponse>)handleMethod.Invoke(handler, new object[] { request, cancellationToken })!;

        // Build the pipeline backwards
        foreach (var behavior in behaviors.AsEnumerable().Reverse())
        {
            var next = handlerDelegate;
            var behaviorMethod = behaviorType.GetMethod("HandleAsync");
            
            if (behaviorMethod == null) continue;

            handlerDelegate = () => 
                (Task<TResponse>)behaviorMethod.Invoke(behavior, new object[] { request, next, cancellationToken })!;
        }

        return await handlerDelegate();
    }
}
