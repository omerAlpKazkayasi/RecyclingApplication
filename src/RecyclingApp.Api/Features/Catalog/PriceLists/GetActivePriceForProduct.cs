using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using RecyclingApp.Infrastructure.Context;
using RecyclingApp.Infrastructure.Persistence;
using RecyclingApp.Modules.Catalog.Domain.Enums;

namespace RecyclingApp.Api.Features.Catalog.PriceLists;

public static class GetActivePriceForProduct
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalog/products/{publicId:guid}/price", async (
            Guid publicId,
            PriceListType type,
            IRequestContext context,
            AppDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            if (!context.HasTenant)
                return Results.BadRequest("Tenant context is missing.");

            var product = await dbContext.Products
                .FirstOrDefaultAsync(p => p.PublicId == publicId, cancellationToken);
            
            if (product == null)
                return Results.NotFound("Product not found.");

            var now = DateTime.UtcNow;

            // Find active price list of given type that is valid right now
            var activeItem = await dbContext.PriceListItems
                .Include(i => i.PriceList)
                .Where(i => i.ProductId == product.Id && 
                            i.PriceList.Type == type &&
                            i.PriceList.IsActive &&
                            i.PriceList.ValidFrom <= now &&
                            (i.PriceList.ValidTo == null || i.PriceList.ValidTo >= now))
                .OrderByDescending(i => i.PriceList.ValidFrom) // most recent valid price list
                .FirstOrDefaultAsync(cancellationToken);

            if (activeItem == null)
                return Results.NotFound("No active price found for this product.");

            return Results.Ok(new 
            {
                PriceListPublicId = activeItem.PriceList.PublicId,
                UnitPrice = activeItem.UnitPrice,
                CurrencyCode = activeItem.CurrencyCode
            });
        })
        .WithName("GetActivePriceForProduct")
        .WithTags("Catalog");
    }
}
