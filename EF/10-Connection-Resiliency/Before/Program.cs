// ============================================================
// BEFORE — Yaygın Hatalar
// Bu dosyada kasıtlı olarak yanlış pratikler gösterilmektedir.
// ============================================================

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ❌ Retry policy yok — geçici bağlantı hataları direkt exception fırlatır
// Bulut veritabanları (Azure SQL, RDS, Cloud SQL) geçici kesintiler yaşar:
//   - Maintenance window
//   - Otomatik failover
//   - Bağlantı havuzu dolması
// Bu kesintiler genellikle 1-2 saniye sürer — retry ile kendiliğinden çözülür
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite("Data Source=before.db")); // ❌ EnableRetryOnFailure yok

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/orders", async (AppDbContext db) =>
    await db.Orders.AsNoTracking().ToListAsync());

// ❌ Bağlantı hatası anında 500 döner — hiç retry yapılmaz
// Müşteri siparişini kaybeder; gerçekte veritabanı 1 saniye sonra geri döner
app.MapPost("/orders", async (Order order, AppDbContext db) =>
{
    db.Orders.Add(order);
    await db.SaveChangesAsync(); // ❌ SqlException → 500 Internal Server Error
    return Results.Created($"/orders/{order.Id}", order);
});

// ❌ Timeout değeri ayarlanmamış — varsayılan 30 saniye
// Yük altında uzun sorgular kullanıcıyı 30 saniye bekletir, sonra hata verir
app.MapGet("/orders/report", async (AppDbContext db) =>
    await db.Orders
            .AsNoTracking()
            // ❌ Uzun süren sorgu için özel timeout yok
            .ToListAsync());

app.Run();
