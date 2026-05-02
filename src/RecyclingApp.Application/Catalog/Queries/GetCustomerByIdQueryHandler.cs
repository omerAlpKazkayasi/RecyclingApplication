using RecyclingApp.Application.Abstractions.Messaging;
using RecyclingApp.Application.Catalog.DTOs;
using RecyclingApp.Application.Catalog.Repositories;

namespace RecyclingApp.Application.Catalog.Queries;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDto> HandleAsync(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID '{request.Id}' not found.");
        }

        return new CustomerDto(customer.Id, customer.Name);
    }
}
