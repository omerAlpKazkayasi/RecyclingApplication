using RecyclingApp.Application.Abstractions.Messaging;

namespace RecyclingApp.Application.Catalog.Commands;

public record DeleteCustomerCommand(Guid Id) : ICommand<bool>;
