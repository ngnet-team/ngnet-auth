using Microsoft.EntityFrameworkCore;

using Database.Models;

namespace Database
{
    public class NgnetAuthDbContext : DbContext
    {
        public NgnetAuthDbContext(DbContextOptions<NgnetAuthDbContext> options)
            :base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Entry> Entries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            base.OnModelCreating(builder);
        }
    }
}
