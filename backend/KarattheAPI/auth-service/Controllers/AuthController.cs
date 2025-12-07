using Microsoft.AspNetCore.Mvc;
using KarattheAPI.AuthService.Models;
using KarattheAPI.AuthService.Services;

namespace KarattheAPI.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Register([FromBody] RegisterRequest? request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { Message = "Email and password are required" });

            var result = await _authService.RegisterAsync(request);
            if (result == null) 
                return BadRequest(new { Message = "Email already exists" });

            return CreatedAtAction(nameof(Register), new { id = result.UserId }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration: {Message}", ex.Message);
            return StatusCode(500, new { Message = "An error occurred during registration", Error = ex.Message });
        }
    }

    [HttpPost("login")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { Message = "Email and password are required" });

            var result = await _authService.LoginAsync(request);
            if (result == null) 
                return Unauthorized(new { Message = "Invalid credentials" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user login");
            return StatusCode(500, new { Message = "An error occurred during login" });
        }
    }

    [HttpPost("forgot-password")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email))
                return BadRequest(new { Message = "Email is required" });

            await _authService.ForgotPasswordAsync(request);
            return Ok(new { Message = "Reset email sent if user exists" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password request");
            return Ok(new { Message = "Reset email sent if user exists" });
        }
    }

    [HttpPost("reset-password")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.NewPassword))
                return BadRequest(new { Message = "Token and new password are required" });

            var success = await _authService.ResetPasswordAsync(request);
            if (!success) 
                return BadRequest(new { Message = "Invalid or expired token" });

            return Ok(new { Message = "Password reset successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset");
            return StatusCode(500, new { Message = "An error occurred during password reset" });
        }
    }

    [HttpGet("validate/{token}")]
    public IActionResult ValidateToken(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest(new { Message = "Token is required" });

            var jwtService = GetJwtService();
            var userId = jwtService.ValidateToken(token);
            
            return Ok(new { Valid = userId.HasValue, UserId = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token validation");
            return StatusCode(500, new { Message = "An error occurred during token validation" });
        }
    }

    private IJwtService GetJwtService()
    {
        return HttpContext.RequestServices.GetRequiredService<IJwtService>();
    }
}