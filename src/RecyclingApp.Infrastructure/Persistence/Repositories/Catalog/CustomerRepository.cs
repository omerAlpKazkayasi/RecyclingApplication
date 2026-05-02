using Microsoft.EntityFrameworkCore;
using RecyclingApp.Application.Catalog.Repositories;
using RecyclingApp.Modules.Catalog.Domain.Entities;

namespace RecyclingApp.Infrastructure.Persistence.Repositories.Catalog;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _dbContext;

    public CustomerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Customer customer)
    {
        _dbContext.Customers.Add(customer);
    }

    public async Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers.ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public void Remove(Customer customer)
    {
        _dbContext.Customers.Remove(customer);
    }
}
