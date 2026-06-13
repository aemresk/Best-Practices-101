// ============================================================
// AFTER — Doğru Yaklaşım
// ============================================================

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ DbContext DI container'a Scoped olarak kaydediliyor
// ✅ Her HTTP isteği için ayrı bir instance oluşturulur ve otomatik dispose edilir
// ✅ Connection string appsettings.json'dan okunuyor — ortama göre değişebilir
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ✅ Migration / DB başlangıcı tek noktada, scoped context ile
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// ✅ DbContext constructor injection ile alınıyor
// ✅ async/await ile non-blocking I/O
// ✅ Framework, request bitince context'i otomatik dispose eder
app.MapGet("/products", async (AppDbContext db) =>
    await db.Products.ToListAsync());

app.MapGet("/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    return product is null ? Results.NotFound() : Results.Ok(product);
});

app.MapPost("/products", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

app.MapDelete("/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    db.Products.Remove(product);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
