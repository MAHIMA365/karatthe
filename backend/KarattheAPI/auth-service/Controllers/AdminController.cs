using Microsoft.AspNetCore.Mvc;
using KarattheAPI.AuthService.Models;
using KarattheAPI.AuthService.Services;

namespace KarattheAPI.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAuthService _authService;

    public AdminController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> AdminLogin(AdminLoginRequest request)
    {
        // Test: Should validate all required fields
        if (string.IsNullOrEmpty(request.Email) || 
            string.IsNullOrEmpty(request.Password) || 
            string.IsNullOrEmpty(request.AdminKey))
            return BadRequest(new { Message = "Email, password, and admin key are required" });

        // Test: Should return 401 for invalid admin credentials
        var result = await _authService.AdminLoginAsync(request);
        if (result == null) 
            return Unauthorized(new { Message = "Invalid admin credentials or admin key" });

        // Test: Should return admin token
        return Ok(result);
    }

    [HttpGet("users")]
    public IActionResult GetAllUsers()
    {
        // Test: This endpoint requires admin authorization
        // For now, return placeholder - will be implemented with admin middleware
        return Ok(new { Message = "Admin endpoint - requires admin authorization middleware" });
    }

    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        // Test: Should return service health status
        return Ok(new { 
            Status = "OK", 
            Service = "AuthService-Admin", 
            Timestamp = DateTime.UtcNow 
        });
    }
}