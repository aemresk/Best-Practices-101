// ============================================================
// BEFORE — Yaygın Hatalar
// Bu dosyada kasıtlı olarak yanlış pratikler gösterilmektedir.
// ============================================================

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=before.db"));

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

// ❌ Sadece okuma amaçlı sorgu — ama ChangeTracker her entity'yi izliyor
// ❌ Değiştirilmeyecek nesneler için gereksiz bellek ve CPU yükü
app.MapGet("/products", async (AppDbContext db) =>
    await db.Products
            .Include(p => p.Category)
            .ToListAsync());

// ❌ FindAsync, ChangeTracker'a önce bakar ve takip eder — salt okunur için gereksiz
app.MapGet("/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    return product is null ? Results.NotFound() : Results.Ok(product);
});

// ❌ Sadece gruplama/hesaplama için tüm entity graph belleğe tracking'li yükleniyor
// ❌ Gruplama uygulama katmanında yapılıyor — veritabanına iş yaptırılmıyor
app.MapGet("/products/report", async (AppDbContext db) =>
{
    var products = await db.Products
        .Include(p => p.Category)
        .ToListAsync();

    return products
        .GroupBy(p => p.Category.Name)
        .Select(g => new { Category = g.Key, Count = g.Count(), Total = g.Sum(p => p.Price) });
});

app.MapPost("/products", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

app.MapDelete("/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    db.Products.Remove(product);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
