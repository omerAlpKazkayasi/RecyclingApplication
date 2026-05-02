using RecyclingApp.Application.Abstractions.Messaging;
using RecyclingApp.Application.Catalog.Repositories;

namespace RecyclingApp.Application.Catalog.Commands;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly ICustomerRepository _customerRepository;

    public DeleteCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<bool> HandleAsync(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (customer == null)
        {
            return false;
        }

        _customerRepository.Remove(customer);
        return true;
    }
}
