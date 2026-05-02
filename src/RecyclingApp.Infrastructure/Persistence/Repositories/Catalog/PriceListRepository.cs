using Microsoft.EntityFrameworkCore;
using RecyclingApp.Application.Catalog.Repositories;
using RecyclingApp.Modules.Catalog.Domain.Entities;

namespace RecyclingApp.Infrastructure.Persistence.Repositories.Catalog;

public class PriceListRepository : IPriceListRepository
{
    private readonly AppDbContext _dbContext;

    public PriceListRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(PriceList priceList)
    {
        _dbContext.PriceLists.Add(priceList);
    }

    public void Remove(PriceList priceList)
    {
        _dbContext.PriceLists.Remove(priceList);
    }

    public async Task<PriceList?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PriceLists
            .Include(pl => pl.Items)
            .FirstOrDefaultAsync(pl => pl.Id == id, cancellationToken);
    }

    public async Task<List<PriceList>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.PriceLists
            .Include(pl => pl.Items)
            .ToListAsync(cancellationToken);
    }
}
