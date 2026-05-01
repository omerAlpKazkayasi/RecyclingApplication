using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using RecyclingApp.Infrastructure.Context;
using RecyclingApp.Infrastructure.Persistence;

namespace RecyclingApp.Api.Features.Catalog.Products;

public static class GetProducts
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalog/products", async (
            IRequestContext context,
            AppDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            if (!context.HasTenant)
                return Results.BadRequest("Tenant context is missing.");

            var products = await dbContext.Products
                .AsNoTracking()
                .Select(p => new
                {
                    p.PublicId,
                    p.Code,
                    p.Name,
                    p.Category,
                    p.QualityGrade,
                    p.UnitName,
                    p.IsActive
                })
                .ToListAsync(cancellationToken);

            return Results.Ok(products);
        })
        .WithName("GetProducts")
        .WithTags("Catalog");
    }
}
