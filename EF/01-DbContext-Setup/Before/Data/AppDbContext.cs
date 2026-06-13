using Microsoft.EntityFrameworkCore;

// ❌ DbContext DI olmadan, OnConfiguring ile konfigüre ediliyor
public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // ❌ Connection string hardcode edilmiş
        // ❌ Ortama (dev/staging/prod) göre değiştirilemiyor
        // ❌ Kaynak kodunda görünür — güvenlik riski
        // ❌ Test sırasında farklı DB kullanmak imkânsız
        optionsBuilder.UseSqlite("Data Source=products_before.db");
    }
}
