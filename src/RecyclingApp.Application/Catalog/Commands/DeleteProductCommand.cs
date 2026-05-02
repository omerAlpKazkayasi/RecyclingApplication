using RecyclingApp.Application.Abstractions.Messaging;

namespace RecyclingApp.Application.Catalog.Commands;

public record DeleteProductCommand(Guid Id) : ICommand<bool>;
