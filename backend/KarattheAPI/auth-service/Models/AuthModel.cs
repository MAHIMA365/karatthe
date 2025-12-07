using System.Text.Json.Serialization;

namespace KarattheAPI.AuthService.Models;

public record RegisterRequest(
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("password")] string? Password
);

public record LoginRequest(
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("password")] string Password
);
public record AdminLoginRequest(string Email, string Password, string AdminKey);
public record AuthResponse(string Token, string Email, string Role, int UserId);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Token, string NewPassword);