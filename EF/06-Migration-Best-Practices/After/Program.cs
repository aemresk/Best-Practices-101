// ============================================================
// AFTER — Doğru Yaklaşım
// ============================================================

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // ✅ Database.Migrate() migration sistemini kullanır:
    //    - __EFMigrationsHistory tablosuna bakar
    //    - Uygulanmamış migration'ları sırayla çalıştırır
    //    - Var olan DB'de sadece eksik değişiklikler uygulanır
    //    - Yeni kolon, index, tablo değişiklikleri migration dosyalarında takip edilir
    db.Database.Migrate();

    // ✅ Seed verisi artık SeedAdminUser migration'ında — uygulama kodunda değil
    // Program.cs temiz; seed mantığı migration geçmişinde kayıtlı ve idempotent
}

app.MapGet("/users", async (AppDbContext db) =>
    await db.Users.AsNoTracking().ToListAsync());

app.MapPost("/users", async (User user, AppDbContext db) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
});

// ✅ Şemaya yeni alan eklemek istesek:
//    dotnet ef migrations add AddPhoneNumberToUsers
//    dotnet ef database update
//    → Var olan kayıtlar korunur, sadece yeni kolon eklenir.

app.Run();
