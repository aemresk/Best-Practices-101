// ============================================================
// AFTER — Doğru Yaklaşım
// ============================================================

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Categories.Any())
    {
        var electronics = new Category { Name = "Elektronik" };
        var clothing = new Category { Name = "Giyim" };
        db.Categories.AddRange(electronics, clothing);
        db.SaveChanges();

        var laptop = new Product { Name = "Laptop", Price = 25000, CategoryId = electronics.Id };
        var mouse  = new Product { Name = "Mouse",  Price = 350,   CategoryId = electronics.Id };
        var shirt  = new Product { Name = "T-Shirt", Price = 200,  CategoryId = clothing.Id };
        db.Products.AddRange(laptop, mouse, shirt);
        db.SaveChanges();

        db.Reviews.AddRange(
            new Review { Comment = "Harika!",        Rating = 5, ProductId = laptop.Id },
            new Review { Comment = "Fiyatı yüksek.", Rating = 3, ProductId = laptop.Id },
            new Review { Comment = "Kullanışlı.",    Rating = 4, ProductId = mouse.Id }
        );
        db.SaveChanges();
    }
}

// ✅ Eager Loading: Include + ThenInclude — tek sorguda ilişkili veri
// N+1 yok; EF Core JOIN ile tek sorguda getirir
app.MapGet("/products/with-reviews", async (AppDbContext db) =>
    await db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Reviews)
            .ToListAsync());

// ✅ Projection: sadece ihtiyaç duyulan alanlar seçiliyor
// Navigation property yüklenmez, SQL'e doğrudan Select çevrilir
app.MapGet("/products/names", async (AppDbContext db) =>
    await db.Products
            .AsNoTracking()
            .Select(p => p.Name)
            .ToListAsync());

// ✅ Projection ile navigation property SQL'de çözülüyor — Include gerekmez
app.MapGet("/products/with-category", async (AppDbContext db) =>
    await db.Products
            .AsNoTracking()
            .Select(p => new { p.Name, CategoryName = p.Category.Name })
            .ToListAsync());

// ✅ Explicit Loading: önce ürünü yükle, ardından şarta göre yorumları yükle
// Koşullu yükleme — her zaman değil, gerektiğinde veri çek
app.MapGet("/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products
                          .Include(p => p.Category)
                          .FirstOrDefaultAsync(p => p.Id == id);
    if (product is null) return Results.NotFound();

    // ✅ Sadece pahalı ürünlerin yorumları yükleniyor
    if (product.Price > 1000)
        await db.Entry(product).Collection(p => p.Reviews).LoadAsync();

    return Results.Ok(new
    {
        product.Name,
        product.Price,
        Category = product.Category.Name,
        Reviews = product.Reviews
    });
});

// ✅ ThenInclude: çok seviyeli ilişki tek sorguda
// Category → Products → Reviews zinciri
app.MapGet("/categories/full", async (AppDbContext db) =>
    await db.Categories
            .AsNoTracking()
            .Include(c => c.Products)
                .ThenInclude(p => p.Reviews)
            .ToListAsync());

app.Run();
