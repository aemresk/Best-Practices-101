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

// ❌ N+1 Problemi: 1 sorgu ile ürünler çekildi, ardından her ürün için +1 sorgu
// 3 ürün → 4 sorgu; 100 ürün → 101 sorgu
app.MapGet("/products/with-reviews", async (AppDbContext db) =>
{
    var products = await db.Products.ToListAsync(); // Sorgu 1

    var result = new List<object>();
    foreach (var product in products)
    {
        // ❌ Her iterasyonda veritabanına gidiyor
        var reviews = await db.Reviews
            .Where(r => r.ProductId == product.Id)
            .ToListAsync(); // Sorgu 2, 3, 4...

        result.Add(new { product.Name, product.Price, Reviews = reviews });
    }
    return result;
});

// ❌ Her endpoint aynı "her şeyi yükle" sorgusunu kullanıyor
// Sadece ürün adı gereken yerde bile Category + Reviews yükleniyor
app.MapGet("/products/names", async (AppDbContext db) =>
{
    var products = await db.Products
        .Include(p => p.Category)   // ❌ Bu endpoint Category'e ihtiyaç duymaz
        .Include(p => p.Reviews)    // ❌ Bu endpoint Reviews'a hiç ihtiyaç duymaz
        .ToListAsync();

    return products.Select(p => p.Name);
});

// ❌ Include unutulmuş — Category her zaman null gelir
app.MapGet("/products/with-category", async (AppDbContext db) =>
{
    var products = await db.Products.ToListAsync(); // Category yüklenmedi

    // ❌ p.Category null — NullReferenceException riski
    return products.Select(p => new
    {
        p.Name,
        CategoryName = p.Category?.Name ?? "(boş — Include eksik)"
    });
});

app.Run();
