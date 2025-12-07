using Microsoft.EntityFrameworkCore;
using KarattheAPI.AuthService.Data;
using KarattheAPI.AuthService.Models;
using System.Security.Cryptography;

namespace KarattheAPI.AuthService.Services;

public class AuthService : IAuthService
{
    private readonly AuthDbContext _context;
    private readonly IJwtService _jwt;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AuthDbContext context, IJwtService jwt, IConfiguration config, ILogger<AuthService> logger)
    {
        _context = context;
        _jwt = jwt;
        _config = config;
        _logger = logger;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Email and password are required");

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return null;

            var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var hash = Convert.ToBase64String(new Rfc2898DeriveBytes(request.Password, Convert.FromBase64String(salt), 10000, HashAlgorithmName.SHA256).GetBytes(32));

            var user = new User
            {
                Email = request.Email,
                PasswordHash = hash,
                Salt = salt,
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwt.GenerateToken(user);
            return new AuthResponse(token, user.Email, user.Role, user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RegisterAsync for email {Email}", request?.Email);
            throw;
        }
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        // Test: Should return null if user not found or inactive
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);
        if (user == null) return null;

        // Test: Should validate password hash
        var hash = Convert.ToBase64String(new Rfc2898DeriveBytes(request.Password, Convert.FromBase64String(user.Salt), 10000, HashAlgorithmName.SHA256).GetBytes(32));
        if (hash != user.PasswordHash) return null;

        // Test: Should return AuthResponse with valid token
        var token = _jwt.GenerateToken(user);
        return new AuthResponse(token, user.Email, user.Role, user.Id);
    }

    public async Task<AuthResponse?> AdminLoginAsync(AdminLoginRequest request)
    {
        // Test: Should validate admin key first
        var adminKey = _config["AdminKey"];
        if (string.IsNullOrEmpty(adminKey) || request.AdminKey != adminKey)
            return null;

        // Test: Should only allow users with Admin role
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Role == "Admin" && u.IsActive);
        if (user == null) return null;

        // Test: Should validate admin password
        var hash = Convert.ToBase64String(new Rfc2898DeriveBytes(request.Password, Convert.FromBase64String(user.Salt), 10000, HashAlgorithmName.SHA256).GetBytes(32));
        if (hash != user.PasswordHash) return null;

        // Test: Should return admin token
        var token = _jwt.GenerateToken(user);
        return new AuthResponse(token, user.Email, user.Role, user.Id);
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        // Test: Should return false if user not found
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null) return false;

        // Test: Should generate reset token with expiry
        user.ResetToken = Guid.NewGuid().ToString();
        user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
    {
        // Test: Should validate reset token and expiry
        var user = await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == request.Token && u.ResetTokenExpiry > DateTime.UtcNow);
        if (user == null) return false;

        // Test: Should update password and clear reset token
        var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        user.PasswordHash = Convert.ToBase64String(new Rfc2898DeriveBytes(request.NewPassword, Convert.FromBase64String(salt), 10000, HashAlgorithmName.SHA256).GetBytes(32));
        user.Salt = salt;
        user.ResetToken = null;
        user.ResetTokenExpiry = null;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        // Test: Should return active user by ID
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
    }
}