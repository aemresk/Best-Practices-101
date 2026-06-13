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
        db.Products.AddRange(
            new Product { Name = "Laptop", Price = 25000, CategoryId = electronics.Id },
            new Product { Name = "Mouse", Price = 350, CategoryId = electronics.Id },
            new Product { Name = "T-Shirt", Price = 200, CategoryId = clothing.Id }
        );
        db.SaveChanges();
    }
}

// ✅ AsNoTracking: ChangeTracker bu entity'leri izlemez
// ✅ Okuma sonrası değişiklik yapılmayacaksa her zaman tercih edilmeli
// ✅ Include ile navigation property yüklenirken de geçerli
app.MapGet("/products", async (AppDbContext db) =>
    await db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .ToListAsync());

// ✅ FindAsync yerine FirstOrDefaultAsync + AsNoTracking
// Not: FindAsync önce ChangeTracker'a bakar — AsNoTracking ile bu adım atlanır,
//      doğrudan veritabanına gidilir.
app.MapGet("/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products
                          .AsNoTracking()
                          .FirstOrDefaultAsync(p => p.Id == id);
    return product is null ? Results.NotFound() : Results.Ok(product);
});

// ✅ Gruplama ve hesaplama veritabanında yapılıyor (SQL'e çevriliyor)
// ✅ AsNoTracking + projection: sadece ihtiyaç duyulan alanlar geliyor
// ✅ Uygulama katmanına ham entity taşınmıyor
app.MapGet("/products/report", async (AppDbContext db) =>
    await db.Products
            .AsNoTracking()
            .GroupBy(p => p.Category.Name)
            .Select(g => new { Category = g.Key, Count = g.Count(), Total = g.Sum(p => p.Price) })
            .ToListAsync());

// ✅ Yazma operasyonlarında tracking gerekli — AsNoTracking kullanılmıyor
app.MapPost("/products", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

// ✅ Silme için FindAsync + tracking: EF'in change detection'a ihtiyacı var
app.MapDelete("/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    db.Products.Remove(product);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
