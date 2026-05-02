using RecyclingApp.Modules.Catalog.Domain.Entities;

namespace RecyclingApp.Application.Catalog.Repositories;

public interface IProductRepository
{
    void Add(Product product);
    void Remove(Product product);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default);
}
