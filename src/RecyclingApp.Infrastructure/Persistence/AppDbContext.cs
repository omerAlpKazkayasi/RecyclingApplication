using Microsoft.EntityFrameworkCore;
using RecyclingApp.Application.Abstractions.Persistence;
using RecyclingApp.Application.Abstractions.Security;
using RecyclingApp.Infrastructure.Context;
using RecyclingApp.Modules.Identity.Domain.Entities;
using RecyclingApp.Modules.Tenancy.Domain.Entities;
using RecyclingApp.Modules.Catalog.Domain.Entities;
using RecyclingApp.Modules.Operations.Domain.Entities;
using RecyclingApp.SharedKernel.Domain;

namespace RecyclingApp.Infrastructure.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    private readonly IRequestContext? _requestContext;

    // Tenancy
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<SystemParameter> SystemParameters => Set<SystemParameter>();

    // Identity
    public DbSet<User> Users => Set<User>();

    // Catalog
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<PriceList> PriceLists => Set<PriceList>();
    public DbSet<PriceListItem> PriceListItems => Set<PriceListItem>();

    // Operations
    public DbSet<Transaction> Transactions => Set<Transaction>();

    public AppDbContext(DbContextOptions<AppDbContext> options, IRequestContext? requestContext = null)
        : base(options)
    {
        _requestContext = requestContext;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from module assemblies
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Modules.Tenancy.Infrastructure.Persistence.TenantConfiguration).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Modules.Identity.Infrastructure.Persistence.UserConfiguration).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Modules.Catalog.Infrastructure.Persistence.ProductConfiguration).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Modules.Operations.Domain.Entities.Transaction).Assembly);

        // Global query filter for tenant isolation on TenantEntity-derived types
        ApplyTenantQueryFilters(modelBuilder);
    }

    private void ApplyTenantQueryFilters(ModelBuilder modelBuilder)
    {
        // Facility is tenant-scoped
        modelBuilder.Entity<Facility>().HasQueryFilter(f =>
            _requestContext == null || !_requestContext.HasTenant || f.TenantId == _requestContext.TenantId);

        // SystemParameter is tenant-scoped
        modelBuilder.Entity<SystemParameter>().HasQueryFilter(sp =>
            _requestContext == null || !_requestContext.HasTenant || sp.TenantId == _requestContext.TenantId);

        // Catalog entities are tenant-scoped
        modelBuilder.Entity<Product>().HasQueryFilter(x =>
            _requestContext == null || !_requestContext.HasTenant || x.TenantId == _requestContext.TenantId);

        modelBuilder.Entity<Customer>().HasQueryFilter(x =>
            _requestContext == null || !_requestContext.HasTenant || x.TenantId == _requestContext.TenantId);

        modelBuilder.Entity<Vehicle>().HasQueryFilter(x =>
            _requestContext == null || !_requestContext.HasTenant || x.TenantId == _requestContext.TenantId);

        modelBuilder.Entity<PriceList>().HasQueryFilter(x =>
            _requestContext == null || !_requestContext.HasTenant || x.TenantId == _requestContext.TenantId);

        modelBuilder.Entity<PriceListItem>().HasQueryFilter(x =>
            _requestContext == null || !_requestContext.HasTenant || x.TenantId == _requestContext.TenantId);

        // Operations entities are tenant/facility-scoped
        modelBuilder.Entity<Transaction>().HasQueryFilter(x =>
            _requestContext == null || (!_requestContext.HasTenant && !_requestContext.HasFacility) ||
            ((!_requestContext.HasTenant || x.TenantId == _requestContext.TenantId) &&
             (!_requestContext.HasFacility || x.FacilityId == _requestContext.FacilityId)));
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void SetAuditFields()
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.GetType().GetProperty("CreatedAt")?.SetValue(entry.Entity, DateTime.UtcNow);
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.MarkUpdated();
            }
        }
    }
}
