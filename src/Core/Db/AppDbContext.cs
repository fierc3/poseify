using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Db
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
    }
}