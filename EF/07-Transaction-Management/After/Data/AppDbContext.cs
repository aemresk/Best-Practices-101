using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.CustomerName).IsRequired().HasMaxLength(200);
            e.Property(x => x.TotalAmount).HasPrecision(18, 2);
            e.HasMany(x => x.Items).WithOne(i => i.Order).HasForeignKey(i => i.OrderId);
        });

        modelBuilder.Entity<OrderItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.UnitPrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Payment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Amount).HasPrecision(18, 2);
            e.Property(x => x.Status).IsRequired().HasMaxLength(50);
        });
    }
}
