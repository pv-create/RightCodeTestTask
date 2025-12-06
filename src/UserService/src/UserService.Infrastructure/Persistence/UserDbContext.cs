using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserFavoriteCurrency> Favorites => Set<UserFavoriteCurrency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.ToTable("users");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(128);
            builder.Property(x => x.PasswordHash).HasColumnName("passwordhash").IsRequired();
            builder.HasIndex(x => x.Name).IsUnique();
            builder.HasMany(x => x.Favorites).WithOne().HasForeignKey(f => f.UserId);
        });

        modelBuilder.Entity<UserFavoriteCurrency>(builder =>
        {
            builder.ToTable("user_favorites");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.UserId).HasColumnName("userid");
            builder.Property(x => x.CurrencyCode).HasColumnName("currencycode").IsRequired().HasMaxLength(8);
        });
    }
}
