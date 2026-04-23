using Microsoft.AspNetCore.Http;
using RecyclingApp.Infrastructure.Context;

namespace RecyclingApp.Infrastructure.Middleware;

/// <summary>
/// Reads X-Tenant-Id, X-Facility-Id, X-User-Id headers and populates the RequestContext.
/// This is a development stub — Phase 10 replaces this with real JWT-based authentication.
/// </summary>
public class RequestContextMiddleware
{
    private readonly RequestDelegate _next;

    public RequestContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, RequestContext requestContext)
    {
        if (httpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader)
            && Guid.TryParse(tenantHeader, out var tenantId))
        {
            requestContext.SetTenant(tenantId);
        }

        if (httpContext.Request.Headers.TryGetValue("X-Facility-Id", out var facilityHeader)
            && Guid.TryParse(facilityHeader, out var facilityId))
        {
            requestContext.SetFacility(facilityId);
        }

        if (httpContext.Request.Headers.TryGetValue("X-User-Id", out var userHeader)
            && Guid.TryParse(userHeader, out var userId))
        {
            requestContext.SetUser(userId);
        }

        await _next(httpContext);
    }
}
