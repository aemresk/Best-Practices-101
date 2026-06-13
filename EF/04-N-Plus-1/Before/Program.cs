// ============================================================
// BEFORE — Yaygın Hatalar
// Uygulamayı çalıştırıp konsolu izleyin: kaç SQL sorgusu gittiğini görün.
// ============================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=before.db")
           // ❌ SQL loglaması her ortamda açık — production'da ciddi performans sorunu
           .LogTo(Console.WriteLine, LogLevel.Information)
           .EnableSensitiveDataLogging());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Products.Any())
    {
        for (int i = 1; i <= 5; i++)
        {
            var product = new Product { Name = $"Ürün {i}", Price = i * 1000 };
            db.Products.Add(product);
            db.SaveChanges();
            db.Reviews.AddRange(
                new Review { Comment = $"Yorum A",  Rating = 5, ProductId = product.Id },
                new Review { Comment = $"Yorum B",  Rating = 3, ProductId = product.Id },
                new Review { Comment = $"Yorum C",  Rating = 4, ProductId = product.Id }
            );
            db.Images.AddRange(
                new Image { Url = $"/img/{i}-1.jpg", ProductId = product.Id },
                new Image { Url = $"/img/{i}-2.jpg", ProductId = product.Id }
            );
            db.SaveChanges();
        }
    }
}

// ❌ N+1: Konsola bakın — 1 + 5 = 6 sorgu gidiyor (5 ürün varken)
// Ürün sayısı arttıkça sorgu sayısı doğrusal büyür
app.MapGet("/products/with-reviews", async (AppDbContext db) =>
{
    var products = await db.Products.ToListAsync(); // Sorgu 1

    var result = new List<object>();
    foreach (var product in products)
    {
        // ❌ Her ürün için ayrı sorgu — N+1
        var reviews = await db.Reviews
            .Where(r => r.ProductId == product.Id)
            .ToListAsync();

        result.Add(new { product.Name, ReviewCount = reviews.Count, Reviews = reviews });
    }
    return result;
});

// ❌ Cartesian Explosion: İki collection'ı aynı anda Include edince JOIN sonucu şişer
// 5 ürün × 3 yorum × 2 resim = 30 satır DB'den çekiliyor, ama 5 ürün bekliyoruz
// Veri doğru gelir ama ağ ve bellek trafiği gereksiz yere N×M katına çıkar
app.MapGet("/products/full", async (AppDbContext db) =>
    await db.Products
            .Include(p => p.Reviews)  // ❌
            .Include(p => p.Images)   // ❌ ikinci collection → cartesian explosion
            .ToListAsync());

app.Run();
