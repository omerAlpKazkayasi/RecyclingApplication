using RecyclingApp.Application.Abstractions.Messaging;
using RecyclingApp.Application.Catalog.DTOs;
using RecyclingApp.Application.Catalog.Repositories;
using RecyclingApp.Application.Abstractions.Security;
using RecyclingApp.Modules.Catalog.Domain.Entities;

namespace RecyclingApp.Application.Catalog.Commands;

public class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IRequestContext _requestContext;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository, IRequestContext requestContext)
    {
        _customerRepository = customerRepository;
        _requestContext = requestContext;
    }

    public Task<CustomerDto> HandleAsync(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        if (!_requestContext.HasTenant)
        {
            throw new InvalidOperationException("Tenant context is required.");
        }

        var customer = Customer.Create(
            _requestContext.TenantId,
            request.Name,
            request.TaxNumber ?? "",
            request.IdentityNumber ?? "",
            request.Phone ?? "",
            request.Address ?? "",
            request.Notes ?? "");

        _customerRepository.Add(customer);

        // SaveChanges is handled by TransactionBehavior

        return Task.FromResult(new CustomerDto(customer.Id, customer.Name));
    }
}
