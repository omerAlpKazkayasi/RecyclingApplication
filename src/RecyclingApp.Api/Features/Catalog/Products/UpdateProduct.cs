using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using RecyclingApp.Infrastructure.Context;
using RecyclingApp.Infrastructure.Persistence;

namespace RecyclingApp.Api.Features.Catalog.Products;

public static class UpdateProduct
{
    public record Request(string Name, string Category, string QualityGrade, string UnitName);

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/catalog/products/{publicId:guid}", async (
            Guid publicId,
            Request request,
            IRequestContext context,
            AppDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            if (!context.HasTenant)
                return Results.BadRequest("Tenant context is missing.");

            var product = await dbContext.Products
                .FirstOrDefaultAsync(p => p.PublicId == publicId, cancellationToken);

            if (product == null)
                return Results.NotFound($"Product '{publicId}' not found.");

            product.Update(request.Name, request.Category, request.QualityGrade, request.UnitName);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok(new { product.PublicId, product.Code, product.Name, product.IsActive });
        })
        .WithName("UpdateProduct")
        .WithTags("Catalog");
    }
}
