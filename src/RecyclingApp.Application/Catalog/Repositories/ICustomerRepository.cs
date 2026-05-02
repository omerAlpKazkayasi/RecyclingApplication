using RecyclingApp.Modules.Catalog.Domain.Entities;

namespace RecyclingApp.Application.Catalog.Repositories;

public interface ICustomerRepository
{
    void Add(Customer customer);
    void Remove(Customer customer);
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
}
