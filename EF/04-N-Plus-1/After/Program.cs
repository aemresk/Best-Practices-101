// ============================================================
// AFTER — Doğru Yaklaşım
// Uygulamayı çalıştırıp konsolu izleyin: kaç SQL sorgusu gittiğini görün.
// ============================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));

    // ✅ SQL loglama sadece Development ortamında açık
    // Konsola bakarak tam olarak hangi sorguların gittiğini izleyebilirsiniz
    if (builder.Environment.IsDevelopment())
        options.LogTo(Console.WriteLine, LogLevel.Information);
});

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
                new Review { Comment = "Yorum A", Rating = 5, ProductId = product.Id },
                new Review { Comment = "Yorum B", Rating = 3, ProductId = product.Id },
                new Review { Comment = "Yorum C", Rating = 4, ProductId = product.Id }
            );
            db.Images.AddRange(
                new Image { Url = $"/img/{i}-1.jpg", ProductId = product.Id },
                new Image { Url = $"/img/{i}-2.jpg", ProductId = product.Id }
            );
            db.SaveChanges();
        }
    }
}

// ✅ N+1 çözümü: Include ile tek JOIN sorgusu
// Konsola bakın — 5 ürün için yalnızca 1 sorgu gidiyor
app.MapGet("/products/with-reviews", async (AppDbContext db) =>
    await db.Products
            .AsNoTracking()
            .Include(p => p.Reviews)
            .ToListAsync());

// ✅ AsSplitQuery: iki collection'ı ayrı sorgularla yükle — cartesian explosion yok
//
// Tek sorguda:   5 ürün × 3 yorum × 2 resim = 30 satır (JOIN şişmesi)
// AsSplitQuery:  Sorgu 1: 5 ürün
//                Sorgu 2: 15 yorum (5×3)
//                Sorgu 3: 10 resim (5×2) → toplam 30 satır ama 3 temiz sorgu
//
// Not: AsSplitQuery transaction bağlamında tutarlılığı siz sağlamalısınız.
app.MapGet("/products/full", async (AppDbContext db) =>
    await db.Products
            .AsNoTracking()
            .Include(p => p.Reviews)
            .Include(p => p.Images)
            .AsSplitQuery()
            .ToListAsync());

// ✅ Projection ile N+1 olmadan özet veri — Include bile gerekmiyor
// Yorum sayısı ve ortalama puan SQL'de hesaplanıyor
app.MapGet("/products/summary", async (AppDbContext db) =>
    await db.Products
            .AsNoTracking()
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                ReviewCount  = p.Reviews.Count,
                AverageRating = p.Reviews.Any()
                    ? p.Reviews.Average(r => r.Rating)
                    : 0,
                ImageCount = p.Images.Count
            })
            .ToListAsync());

app.Run();
