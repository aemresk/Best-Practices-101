// ============================================================
// BEFORE — Yaygın Hatalar
// Bu dosyada kasıtlı olarak yanlış pratikler gösterilmektedir.
// ============================================================

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite("Data Source=before.db"));
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Products.Any())
    {
        var categories = new[] { "Elektronik", "Giyim", "Kitap", "Spor", "Ev" };
        for (int i = 1; i <= 200; i++)
            db.Products.Add(new Product
            {
                Name     = $"Ürün {i:D3}",
                Price    = Random.Shared.Next(50, 50000),
                Category = categories[i % categories.Length]
            });
        db.SaveChanges();
    }
}

// ❌ Tüm kayıtları belleğe çekip C#'ta sayfalıyor
// 200 kayıt küçük görünür; 1 milyon kayıtta uygulama belleği tükenir
app.MapGet("/products", async (int page, AppDbContext db) =>
{
    var all = await db.Products.ToListAsync(); // ❌ 200 (veya 1M) kayıt belleğe
    return all
        .Skip((page - 1) * 10)
        .Take(10);                             // ❌ sayfalama C#'ta, geç kalındı
});

// ❌ Hiç sayfalama yok — tüm tablo dönüyor
app.MapGet("/products/all", async (AppDbContext db) =>
    await db.Products.ToListAsync()); // ❌ sınırsız veri

// ❌ Count için de tüm veriyi belleğe çekiyor
app.MapGet("/products/paged", async (int page, AppDbContext db) =>
{
    var all    = await db.Products.ToListAsync(); // ❌ hepsi belleğe
    var total  = all.Count;                       // ❌ SELECT * sadece Count için
    var items  = all.Skip((page - 1) * 10).Take(10);
    return new { total, items };
});

// ❌ Sıralama yok — veritabanı kayıtları tutarsız sırayla dönebilir
// Aynı sayfayı iki kez sorgularsanız farklı sonuç alabilirsiniz
app.MapGet("/products/unsafe-paged", async (int page, AppDbContext db) =>
    await db.Products               // ❌ OrderBy yok
            .Skip((page - 1) * 10)
            .Take(10)
            .ToListAsync());

app.Run();
