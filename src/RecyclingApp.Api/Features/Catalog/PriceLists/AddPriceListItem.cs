using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using RecyclingApp.Infrastructure.Context;
using RecyclingApp.Infrastructure.Persistence;
using RecyclingApp.Modules.Catalog.Domain.Entities;

namespace RecyclingApp.Api.Features.Catalog.PriceLists;

public static class AddPriceListItem
{
    public record Request(Guid PriceListPublicId, Guid ProductPublicId, decimal UnitPrice, string CurrencyCode);

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/catalog/pricelists/items", async (
            Request request,
            IRequestContext context,
            AppDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            if (!context.HasTenant)
                return Results.BadRequest("Tenant context is missing.");

            var priceList = await dbContext.PriceLists
                .FirstOrDefaultAsync(p => p.PublicId == request.PriceListPublicId, cancellationToken);
            
            if (priceList == null)
                return Results.NotFound("PriceList not found.");

            var product = await dbContext.Products
                .FirstOrDefaultAsync(p => p.PublicId == request.ProductPublicId, cancellationToken);

            if (product == null)
                return Results.NotFound("Product not found.");

            var exists = await dbContext.PriceListItems.AnyAsync(
                p => p.PriceListId == priceList.Id && p.ProductId == product.Id,
                cancellationToken);

            if (exists)
                return Results.Conflict("This product is already in the price list.");

            var item = PriceListItem.Create(
                context.TenantId,
                priceList.Id,
                product.Id,
                request.UnitPrice,
                request.CurrencyCode);

            dbContext.PriceListItems.Add(item);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok(new { item.Id, item.PublicId });
        })
        .WithName("AddPriceListItem")
        .WithTags("Catalog");
    }
}
