using RecyclingApp.Application.Abstractions.Messaging;
using RecyclingApp.Application.Catalog.DTOs;
using RecyclingApp.Application.Catalog.Repositories;

namespace RecyclingApp.Application.Catalog.Queries;

public class ListCustomersQueryHandler : IQueryHandler<ListCustomersQuery, List<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;

    public ListCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<List<CustomerDto>> HandleAsync(ListCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);
        
        return customers.Select(c => new CustomerDto(c.Id, c.Name)).ToList();
    }
}
