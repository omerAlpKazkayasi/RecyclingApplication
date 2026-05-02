namespace RecyclingApp.Application.Abstractions.Messaging;

public interface IApplicationDispatcher
{
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}
