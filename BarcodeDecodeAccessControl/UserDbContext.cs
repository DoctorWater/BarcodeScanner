using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BarcodeDecodeAccessControl;

public class UserDbContext : IdentityDbContext<IdentityUser>
{
    protected UserDbContext()
    {
    }

    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder
                .UseNpgsql("User ID=test_user;Include Error Detail=True;Password=HailTheKing;Host=localhost;Port=5431;Database=VKR_test_db;Enlist=true;")
                .EnableSensitiveDataLogging()
                .UseSnakeCaseNamingConvention()
                .EnableDetailedErrors(true);
        }
    }
}