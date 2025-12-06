using CurrencyService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyService.Infrastructure.Persistence;

public class CurrencyDbContext : DbContext
{
    public CurrencyDbContext(DbContextOptions<CurrencyDbContext> options) : base(options)
    {
    }

    public DbSet<Currency> Currencies => Set<Currency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>(builder =>
        {
            builder.ToTable("currency");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Code).HasColumnName("code").IsRequired().HasMaxLength(16);
            builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(256);
            builder.Property(x => x.Rate).HasColumnName("rate").HasColumnType("numeric(18,6)");
            builder.Property(x => x.UpdatedAt).HasColumnName("updatedat").IsRequired();
            builder.HasIndex(x => x.Code).IsUnique();
        });
    }
}
