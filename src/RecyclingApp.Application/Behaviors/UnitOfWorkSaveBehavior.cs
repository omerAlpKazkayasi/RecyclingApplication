using RecyclingApp.Application.Abstractions.Messaging;
using RecyclingApp.Application.Abstractions.Persistence;

namespace RecyclingApp.Application.Behaviors;

public class UnitOfWorkSaveBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UnitOfWorkSaveBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Only apply transaction if it's a command
        var isCommand = request.GetType().GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>) || i == typeof(ICommand));

        if (!isCommand)
        {
            return await next();
        }

        var response = await next();
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
    }
}
