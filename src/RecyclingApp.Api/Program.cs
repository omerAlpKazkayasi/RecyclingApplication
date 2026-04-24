using RecyclingApp.Infrastructure;
using RecyclingApp.Infrastructure.Middleware;
using RecyclingApp.Infrastructure.Persistence;
using RecyclingApp.Infrastructure.Seed;
using RecyclingApp.ServiceDefaults;
using Microsoft.EntityFrameworkCore;
using RecyclingApp.Api.Features.Catalog;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (OpenTelemetry, health checks - minimal for Phase 1)
builder.AddServiceDefaults();

// Infrastructure (PostgreSQL + request context)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

// Request context middleware (reads X-Tenant-Id, X-Facility-Id, X-User-Id headers)
app.UseMiddleware<RequestContextMiddleware>();

// Health check
app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0-phase1"
})).WithName("HealthCheck").WithTags("health");

// Basic info endpoint
app.MapGet("/", () => Results.Ok(new
{
    Application = "RecyclingApp - Scrap Yard Management System",
    Phase = "Phase 1 - Core Skeleton",
    Endpoints = new[] { "/health", "/api/tenants/current" }
}));

// Tenant context verification endpoint
app.MapGet("/api/tenants/current", (RecyclingApp.Infrastructure.Context.IRequestContext ctx) => Results.Ok(new
{
    TenantId = ctx.HasTenant ? ctx.TenantId.ToString() : null,
    FacilityId = ctx.HasFacility ? ctx.FacilityId?.ToString() : null,
    UserId = ctx.HasUser ? ctx.UserId?.ToString() : null,
    Message = ctx.HasTenant ? "Context resolved" : "No tenant context. Send X-Tenant-Id header."
})).WithName("GetCurrentTenant").WithTags("tenancy");

// Map default endpoints from ServiceDefaults (health checks)
app.MapDefaultEndpoints();

// Map Catalog endpoints
app.MapCatalogEndpoints();

// Apply migrations and seed on startup (development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
    await DataSeeder.SeedAsync(dbContext);
}

app.Run();
