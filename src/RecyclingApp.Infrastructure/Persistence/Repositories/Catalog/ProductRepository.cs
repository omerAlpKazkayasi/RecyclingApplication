using Microsoft.EntityFrameworkCore;
using RecyclingApp.Application.Catalog.Repositories;
using RecyclingApp.Modules.Catalog.Domain.Entities;

namespace RecyclingApp.Infrastructure.Persistence.Repositories.Catalog;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _dbContext;

    public ProductRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Product product)
    {
        _dbContext.Products.Add(product);
    }

    public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products.ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public void Remove(Product product)
    {
        _dbContext.Products.Remove(product);
    }
}
