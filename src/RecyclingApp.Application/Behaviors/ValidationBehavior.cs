using RecyclingApp.Application.Abstractions.Messaging;

namespace RecyclingApp.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    // In the future, inject IEnumerable<IValidator<TRequest>> here
    
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // For now, this is just a placeholder to show the pipeline works.
        // If validators were injected, we would run them here and throw a ValidationException if any fail.
        
        return await next();
    }
}
