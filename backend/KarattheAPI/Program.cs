using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using KarattheAPI.AuthService.Data;
using KarattheAPI.AuthService.Services;
using KarattheAPI.AuthService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Auth Service Database
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AuthConnection")));

// Auth Service Dependencies
builder.Services.AddScoped<IAuthService, KarattheAPI.AuthService.Services.AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["token"];
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Allow frontend dev server to access this API (CORS)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocal",
        policy => policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
});

var app = builder.Build();

// TDD: Seed database with test admin user
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    context.Database.EnsureCreated();
    
    // Seed admin user if not exists
    if (!context.Users.Any(u => u.Role == "Admin"))
    {
        var adminSalt = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
        var adminHash = Convert.ToBase64String(new System.Security.Cryptography.Rfc2898DeriveBytes("admin123", Convert.FromBase64String(adminSalt), 10000, System.Security.Cryptography.HashAlgorithmName.SHA256).GetBytes(32));
        
        context.Users.Add(new User
        {
            Email = "admin@karatthe.com",
            PasswordHash = adminHash,
            Salt = adminSalt,
            Role = "Admin"
        });
        context.SaveChanges();
    }
}

app.UseDeveloperExceptionPage();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Enable CORS for local frontend during development
app.UseCors("AllowLocal");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// TDD: Health check endpoint
app.MapGet("/health", () => new { Status = "OK", Service = "AuthService", Timestamp = DateTime.UtcNow });

app.Run();
