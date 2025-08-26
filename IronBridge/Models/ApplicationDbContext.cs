using Microsoft.EntityFrameworkCore;

namespace IronBridge.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<IronBridges> IronBridges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IronBridges>()
                .Property(b => b.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
