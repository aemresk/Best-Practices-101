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
        db.Products.AddRange(
            new Product { Name = "Laptop",  Price = 25000 },
            new Product { Name = "Mouse",   Price = 350 },
            new Product { Name = "Klavye",  Price = 450 },
            new Product { Name = "Monitor", Price = 8000 }
        );
        db.SaveChanges();
    }
}

app.MapGet("/products", async (AppDbContext db) =>
    await db.Products.AsNoTracking().ToListAsync());

app.MapPost("/products", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

// ❌ Fiziksel (hard) delete — kayıt veritabanından tamamen kaldırılıyor
// ❌ Geri alma imkansız — yetersiz stok, yanlış silme durumunda veri kaybı
// ❌ Denetim izi (audit trail) yok — kim sildi, ne zaman sildi bilinmiyor
// ❌ Bu ürüne referans veren siparişler bozulabilir (FK ihlali veya orphan kayıt)
app.MapDelete("/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    db.Products.Remove(product); // ❌ Fiziksel silme
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// ❌ "Silinen" ürünleri listeleme yolu yok — veri gitti
// ❌ Raporlama sistemleri geçmiş verilere erişemez

app.Run();
