using Microsoft.EntityFrameworkCore;
using KarattheAPI.AuthService.Models;

namespace KarattheAPI.AuthService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        try
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).HasDefaultValue("User");
            });
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to configure database model", ex);
        }
    }
}