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
        var elektronik = new Category { Name = "Elektronik" };
        var giyim      = new Category { Name = "Giyim" };
        db.Categories.AddRange(elektronik, giyim);
        db.SaveChanges();
        db.Products.AddRange(
            new Product { Name = "Laptop",   Price = 25000, IsActive = true,  CategoryId = elektronik.Id },
            new Product { Name = "Mouse",    Price = 350,   IsActive = true,  CategoryId = elektronik.Id },
            new Product { Name = "Klavye",   Price = 450,   IsActive = false, CategoryId = elektronik.Id },
            new Product { Name = "T-Shirt",  Price = 200,   IsActive = true,  CategoryId = giyim.Id },
            new Product { Name = "Eski Şort",Price = 50,    IsActive = false, CategoryId = giyim.Id }
        );
        db.SaveChanges();
    }
}

// ❌ "Aktif ürün" tanımı (IsActive == true) her endpoint'e kopyalanıyor
// IsActive kontrolünü kaldırmak veya değiştirmek istesek tüm endpoint'leri bulmak gerekir
app.MapGet("/products", async (AppDbContext db) =>
    await db.Products
            .AsNoTracking()
            .Where(p => p.IsActive)          // ❌ tekrar
            .Include(p => p.Category)
            .ToListAsync());

// ❌ Aynı IsActive filtresi — fiyat filtresi eklendi ama IsActive tekrar yazıldı
app.MapGet("/products/cheap", async (AppDbContext db) =>
    await db.Products
            .AsNoTracking()
            .Where(p => p.IsActive && p.Price < 500)  // ❌ tekrar
            .ToListAsync());

// ❌ Aynı filtre, bu sefer kategori ile birlikte
app.MapGet("/categories/{id}/products", async (int id, AppDbContext db) =>
    await db.Products
            .AsNoTracking()
            .Where(p => p.IsActive && p.CategoryId == id)  // ❌ tekrar
            .Include(p => p.Category)
            .ToListAsync());

// ❌ Endpoint doğrudan DbContext'e bağlı — test yazılamaz, mock'lanamaz
app.MapGet("/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products
                          .AsNoTracking()
                          .Include(p => p.Category)
                          .FirstOrDefaultAsync(p => p.Id == id && p.IsActive); // ❌ tekrar
    return product is null ? Results.NotFound() : Results.Ok(product);
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
