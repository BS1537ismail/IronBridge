using Microsoft.EntityFrameworkCore;

namespace UserAuth.Repository.Data;

public class UserAuthDbContext : DbContext
{
    public UserAuthDbContext(DbContextOptions<UserAuthDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Role).HasConversion<string>();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
