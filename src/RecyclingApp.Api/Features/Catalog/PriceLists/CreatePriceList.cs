using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using RecyclingApp.Infrastructure.Context;
using RecyclingApp.Infrastructure.Persistence;
using RecyclingApp.Modules.Catalog.Domain.Entities;
using RecyclingApp.Modules.Catalog.Domain.Enums;

namespace RecyclingApp.Api.Features.Catalog.PriceLists;

public static class CreatePriceList
{
    public record Request(PriceListType Type, string Name, DateTime ValidFrom, DateTime? ValidTo);

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/catalog/pricelists", async (
            Request request,
            IRequestContext context,
            AppDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            if (!context.HasTenant)
                return Results.BadRequest("Tenant context is missing.");

            var priceList = PriceList.Create(
                context.TenantId,
                request.Type,
                request.Name,
                request.ValidFrom,
                request.ValidTo);

            dbContext.PriceLists.Add(priceList);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok(new { priceList.Id, priceList.PublicId, priceList.Name });
        })
        .WithName("CreatePriceList")
        .WithTags("Catalog");
    }
}
