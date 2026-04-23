using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

        return services;
    }
}
