using RecyclingApp.Application.Abstractions.Messaging;
using RecyclingApp.Application.Catalog.DTOs;

namespace RecyclingApp.Application.Catalog.Commands;

public record CreateCustomerCommand(string Name, string TaxNumber, string IdentityNumber, string Phone, string Address, string Notes) : ICommand<CustomerDto>;
