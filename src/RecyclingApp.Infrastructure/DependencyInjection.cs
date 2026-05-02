using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecyclingApp.Application.Abstractions.Security;
using RecyclingApp.Infrastructure.Context;
using RecyclingApp.Infrastructure.Persistence;

namespace RecyclingApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        // PostgreSQL + EF Core
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
            });
        });

        // Request context (scoped per request)
        services.AddScoped<RequestContext>();
        services.AddScoped<IRequestContext>(sp => sp.GetRequiredService<RequestContext>());

        // Repositories
        services.AddScoped<RecyclingApp.Application.Catalog.Repositories.ICustomerRepository, RecyclingApp.Infrastructure.Persistence.Repositories.Catalog.CustomerRepository>();
        services.AddScoped<RecyclingApp.Application.Catalog.Repositories.IProductRepository, RecyclingApp.Infrastructure.Persistence.Repositories.Catalog.ProductRepository>();
        services.AddScoped<RecyclingApp.Application.Catalog.Repositories.IVehicleRepository, RecyclingApp.Infrastructure.Persistence.Repositories.Catalog.VehicleRepository>();
        services.AddScoped<RecyclingApp.Application.Catalog.Repositories.IPriceListRepository, RecyclingApp.Infrastructure.Persistence.Repositories.Catalog.PriceListRepository>();

        // Unit of Work
        services.AddScoped<RecyclingApp.Application.Abstractions.Persistence.IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        return services;
    }
}
