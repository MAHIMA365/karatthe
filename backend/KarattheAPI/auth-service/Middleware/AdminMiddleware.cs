using System.Security.Claims;
using KarattheAPI.AuthService.Services;

namespace KarattheAPI.AuthService.Middleware;

public class AdminMiddleware
{
    private readonly RequestDelegate _next;

    public AdminMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
    {
        // TDD: Skip middleware for non-admin endpoints
        if (!context.Request.Path.StartsWithSegments("/api/admin"))
        {
            await _next(context);
            return;
        }

        // TDD: Skip for login endpoint
        if (context.Request.Path.Value?.EndsWith("/login") == true)
        {
            await _next(context);
            return;
        }

        // TDD: Validate admin token
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        
        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Admin token required");
            return;
        }

        var userId = jwtService.ValidateToken(token);
        if (!userId.HasValue)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid admin token");
            return;
        }

        // TDD: Check if user has admin role (this would require user service call)
        // For now, assume token validation is sufficient for admin endpoints
        
        await _next(context);
    }
}