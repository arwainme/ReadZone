using Microsoft.EntityFrameworkCore;
using ReadZone.Models;

namespace ReadZone.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

    }
}
