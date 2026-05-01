using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using RecyclingApp.Infrastructure.Context;
using RecyclingApp.Infrastructure.Persistence;
using RecyclingApp.Modules.Catalog.Domain.Entities;

namespace RecyclingApp.Api.Features.Catalog.Products;

public static class CreateProduct
{
    public record Request(string Code, string Name, string Category, string QualityGrade, string UnitName);

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/catalog/products", async (
            Request request,
            IRequestContext context,
            AppDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            if (!context.HasTenant)
                return Results.BadRequest("Tenant context is missing.");

            // Check uniqueness
            var exists = await dbContext.Products.AnyAsync(
                p => p.TenantId == context.TenantId && p.Code == request.Code,
                cancellationToken);

            if (exists)
                return Results.Conflict($"Product with code '{request.Code}' already exists for this tenant.");

            var product = Product.Create(
                context.TenantId,
                request.Code,
                request.Name,
                request.Category,
                request.QualityGrade,
                request.UnitName);

            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok(new { product.Id, product.PublicId, product.Code });
        })
        .WithName("CreateProduct")
        .WithTags("Catalog");
    }
}
