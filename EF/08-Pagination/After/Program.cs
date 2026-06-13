// ============================================================
// AFTER — Doğru Yaklaşım
// ============================================================

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
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

// ✅ Offset-based pagination — Skip/Take SQL'de çalışır, belleğe tüm veri çekilmez
// ✅ OrderBy zorunlu: sıralama olmadan sayfa içeriği tutarsız olabilir
// ✅ pageSize üst sınırı: istemci 10.000 isterse sunucu çökmez
// ✅ CountAsync() sadece COUNT(*) SQL'i — tüm veriyi çekmez
app.MapGet("/products", async (AppDbContext db, int page = 1, int pageSize = 10) =>
{
    page     = Math.Max(1, page);
    pageSize = Math.Clamp(pageSize, 1, 100);

    var query = db.Products.AsNoTracking().OrderBy(p => p.Id);

    var total = await query.CountAsync();
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new PagedResult<Product>(items, total, page, pageSize);
});

// ✅ Filtreyle sayfalama: filtre önce, Count sonra — sadece eşleşenleri say
app.MapGet("/products/by-category", async (
    AppDbContext db,
    string category,
    int page = 1,
    int pageSize = 10) =>
{
    page     = Math.Max(1, page);
    pageSize = Math.Clamp(pageSize, 1, 100);

    var query = db.Products
                  .AsNoTracking()
                  .Where(p => p.Category == category)
                  .OrderBy(p => p.Name);

    var total = await query.CountAsync();
    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new PagedResult<Product>(items, total, page, pageSize);
});

// ✅ Cursor-based pagination: büyük veri setleri için offset'ten daha performanslı
// Offset'te "sayfa 500'e git" derse DB 5000 satırı sayar/atlar.
// Cursor'da sadece "lastId'den sonraki N kayıt" — her zaman O(1) seek.
app.MapGet("/products/cursor", async (
    AppDbContext db,
    int? lastId  = null,
    int pageSize = 10) =>
{
    pageSize = Math.Clamp(pageSize, 1, 100);

    var query = db.Products.AsNoTracking().OrderBy(p => p.Id);

    if (lastId.HasValue)
        query = (IOrderedQueryable<Product>)query.Where(p => p.Id > lastId.Value);

    var items = await query.Take(pageSize).ToListAsync();

    return new
    {
        Items      = items,
        NextCursor = items.Count == pageSize ? items[^1].Id : (int?)null
    };
});

app.Run();
