// ============================================================
// AFTER — Doğru Yaklaşım
// ============================================================
// NOT: EnableRetryOnFailure SQLite tarafından desteklenmez.
//      Bu örnek SQLite ile çalışır (demo için), ama retry konfigürasyonu
//      gerçek uygulamada SQL Server veya PostgreSQL için aktif edilir.
//      Kod, SQL Server konfigürasyonunu yorum satırı olarak içermektedir.
// ============================================================

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ──────────────────────────────────────────────────────────────
// Demo (SQLite — retry desteklenmez, örneği çalıştırmak için)
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ──────────────────────────────────────────────────────────────
// Production (SQL Server) — aşağıdaki bloğu etkinleştirin:
//
// builder.Services.AddDbContext<AppDbContext>(o =>
//     o.UseSqlServer(
//         builder.Configuration.GetConnectionString("SqlServer"),
//         sqlOptions => sqlOptions.EnableRetryOnFailure(
//             // ✅ Geçici hatalar için en fazla 5 deneme
//             maxRetryCount: 5,
//             // ✅ Denemeler arasında en fazla 30 saniye bekleme (exponential backoff)
//             maxRetryDelay: TimeSpan.FromSeconds(30),
//             // ✅ null = EF'in varsayılan geçici hata kodları kullanılır
//             errorNumbersToAdd: null)));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/orders", async (AppDbContext db) =>
    await db.Orders.AsNoTracking().ToListAsync());

app.MapPost("/orders", async (Order order, AppDbContext db) =>
{
    db.Orders.Add(order);
    // ✅ Retry policy aktifken: geçici bağlantı hatası → 5 kez yeniden dener
    // Müşteri yerine 500 görür, uygulama sessizce kurtarılır
    await db.SaveChangesAsync();
    return Results.Created($"/orders/{order.Id}", order);
});

// ✅ Explicit transaction + retry: ikisi birlikte kullanılırken
// ExecutionStrategy.ExecuteAsync() gereklidir — transaction içinde retry otomatik çalışmaz
app.MapPost("/orders/with-explicit-tx", async (Order order, AppDbContext db) =>
{
    var strategy = db.Database.CreateExecutionStrategy();

    await strategy.ExecuteAsync(async () =>
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        try
        {
            db.Orders.Add(order);
            await db.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    });

    return Results.Created($"/orders/{order.Id}", order);
});

// ✅ CommandTimeout: belirli sorgular için özel timeout
app.MapGet("/orders/report", async (AppDbContext db) =>
{
    // Uzun sürebilecek rapor sorgusu için timeout artırılıyor
    db.Database.SetCommandTimeout(TimeSpan.FromSeconds(120));
    return await db.Orders.AsNoTracking().ToListAsync();
});

app.Run();
