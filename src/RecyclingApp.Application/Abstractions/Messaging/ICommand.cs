namespace RecyclingApp.Application.Abstractions.Messaging;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

public interface ICommand : IRequest<Unit>
{
}

public readonly struct Unit
{
    public static readonly Unit Value = new();
}
