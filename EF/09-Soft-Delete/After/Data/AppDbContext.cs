using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Price).HasPrecision(18, 2);

            // ✅ Global query filter: tüm Product sorgularına otomatik WHERE IsDeleted = 0 eklenir
            // Endpoint'lerde tek satır kod bile yazmaya gerek yok
            e.HasQueryFilter(x => !x.IsDeleted);
        });
    }

    // ✅ SaveChangesAsync override: fiziksel silme isteğini soft delete'e çevirir
    // db.Products.Remove(p) çağrısı artık DELETE yerine UPDATE IsDeleted=1 yapar
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<ISoftDeletable>()
                                           .Where(e => e.State == EntityState.Deleted))
        {
            entry.State          = EntityState.Modified;
            entry.Entity.IsDeleted  = true;
            entry.Entity.DeletedAt  = DateTime.UtcNow;
        }
        return await base.SaveChangesAsync(ct);
    }
}
