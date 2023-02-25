using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}