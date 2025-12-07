using KarattheAPI.AuthService.Models;

namespace KarattheAPI.AuthService.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    int? ValidateToken(string token);
}