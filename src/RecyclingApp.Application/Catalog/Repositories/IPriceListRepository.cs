using RecyclingApp.Modules.Catalog.Domain.Entities;

namespace RecyclingApp.Application.Catalog.Repositories;

public interface IPriceListRepository
{
    void Add(PriceList priceList);
    void Remove(PriceList priceList);
    Task<PriceList?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<PriceList>> GetAllAsync(CancellationToken cancellationToken = default);
}
