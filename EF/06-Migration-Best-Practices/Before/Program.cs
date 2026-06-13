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

    // ❌ EnsureCreated migration sistemini tamamen devre dışı bırakır:
    //    - __EFMigrationsHistory tablosu oluşturulmaz
    //    - Hangi değişikliklerin uygulandığı takip edilemez
    //    - Sonradan migration eklenirse EnsureCreated'ı kaldırmak gerekir
    //    - Var olan bir veritabanında schema değişikliği yapılamaz
    db.Database.EnsureCreated();

    // ❌ Seed verisi uygulama kodu içinde — her deploy'da kontrol gerektirir
    // ❌ İdempotent değil: her seferinde kontrol yapılması gerekiyor
    // ❌ Seed verisi ile şema değişikliği aynı yerde karışıyor
    if (!db.Users.Any())
    {
        db.Users.Add(new User
        {
            Name = "Admin",
            Email = "admin@example.com",
            IsAdmin = true
        });
        db.SaveChanges();
    }
}

app.MapGet("/users", async (AppDbContext db) =>
    await db.Users.AsNoTracking().ToListAsync());

app.MapPost("/users", async (User user, AppDbContext db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
});

// ❌ Şemaya yeni bir alan eklemek istesek (örn. PhoneNumber):
//    1. User.cs'e ekle
//    2. EnsureCreated var olan DB'yi GÜNCELLEMİYOR — tabloya kolon eklemiyor
//    3. Ya DB'yi elle sil, ya da SQL ile ALTER TABLE çalıştır
//    Oysa migration sistemi bunu otomatik halleder.

app.Run();
