using Microsoft.EntityFrameworkCore;

// ✅ Constructor ile DbContextOptions alınıyor — DI uyumlu ve test edilebilir
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ✅ Fluent API ile konfigürasyon — Data Annotations'a göre daha esnek
        // ✅ Model konfigürasyonu tek yerde, okunabilir
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(e => e.Price)
                  .HasPrecision(18, 2);

            entity.Property(e => e.CreatedAt)
                  .IsRequired();
        });
    }
}
