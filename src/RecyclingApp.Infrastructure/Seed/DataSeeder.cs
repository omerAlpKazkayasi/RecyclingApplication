using Microsoft.EntityFrameworkCore;
using RecyclingApp.Infrastructure.Persistence;
using RecyclingApp.Modules.Identity.Domain.Entities;
using RecyclingApp.Modules.Tenancy.Domain.Entities;
using RecyclingApp.Modules.Catalog.Domain.Entities;
using RecyclingApp.SharedKernel.Domain;

namespace RecyclingApp.Infrastructure.Seed;

/// <summary>
/// Seeds minimal reference data for development.
/// Creates one tenant, one facility, one user, and base system parameters.
/// </summary>
public static class DataSeeder
{
    // Well-known seed IDs for development
    public static readonly Guid DefaultTenantId = Guid.Parse("01966f00-0000-7000-8000-000000000001");
    public static readonly Guid DefaultFacilityId = Guid.Parse("01966f00-0000-7000-8000-000000000002");
    public static readonly Guid DefaultUserId = Guid.Parse("01966f00-0000-7000-8000-000000000003");

    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Tenants.AnyAsync())
            return; // Already seeded

        // Create default tenant
        var tenant = Tenant.Create("DEFAULT", "Default Tenant");
        SetEntityId(tenant, DefaultTenantId);
        context.Tenants.Add(tenant);

        // Create default facility
        var facility = Facility.Create(DefaultTenantId, "MAIN", "Main Yard", "Default Address");
        SetEntityId(facility, DefaultFacilityId);
        context.Facilities.Add(facility);

        // Create default user
        var user = User.Create("admin", "System Administrator");
        SetEntityId(user, DefaultUserId);
        context.Users.Add(user);

        // Create default system parameters
        var parameters = new[]
        {
            SystemParameter.Create(DefaultTenantId, null, "WEIGHT_TOLERANCE_PERCENT", "2.0",
                "Allowed weight mismatch tolerance percentage for vehicle transactions"),
            SystemParameter.Create(DefaultTenantId, null, "DEFAULT_CURRENCY", "TRY",
                "Default currency code for transactions"),
            SystemParameter.Create(DefaultTenantId, null, "TRANSACTION_NO_FORMAT", "AUTO_INCREMENT",
                "Transaction numbering format: AUTO_INCREMENT or FORMATTED"),
        };

        context.SystemParameters.AddRange(parameters);

        // Seed basic products
        if (!await context.Products.AnyAsync())
        {
            var products = new[]
            {
                Product.Create(DefaultTenantId, "PRD-CU", "Copper Mixed", "Non-Ferrous", "Grade A", "kg"),
                Product.Create(DefaultTenantId, "PRD-AL", "Aluminum Cans", "Non-Ferrous", "UBC", "kg"),
                Product.Create(DefaultTenantId, "PRD-FE", "Iron Scrap", "Ferrous", "Heavy Melting", "ton")
            };
            
            // Assign well known IDs for predictability if needed, or let them generate
            SetEntityId(products[0], Guid.Parse("01966f00-0000-7000-8000-000000000010"));
            SetEntityId(products[1], Guid.Parse("01966f00-0000-7000-8000-000000000011"));
            SetEntityId(products[2], Guid.Parse("01966f00-0000-7000-8000-000000000012"));

            context.Products.AddRange(products);
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Sets a well-known ID on an entity for seed data determinism.
    /// Uses reflection to bypass the protected setter.
    /// </summary>
    private static void SetEntityId(EntityBase entity, Guid id)
    {
        typeof(EntityBase).GetProperty(nameof(EntityBase.Id))!.SetValue(entity, id);
    }
}
