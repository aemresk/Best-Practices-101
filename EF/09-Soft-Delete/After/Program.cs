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
    if (!db.Products.IgnoreQueryFilters().Any())
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

// ✅ Global filter devrede — IsDeleted=true olanlar otomatik gizlenir
// Hiçbir endpoint'e Where(p => !p.IsDeleted) yazmak gerekmez
app.MapGet("/products", async (AppDbContext db) =>
    await db.Products.AsNoTracking().ToListAsync());

app.MapGet("/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    return product is null ? Results.NotFound() : Results.Ok(product);
});

app.MapPost("/products", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

// ✅ Remove() çağrısı AppDbContext tarafından soft delete'e çevriliyor
// Endpoint kodu değişmedi — sadece DbContext davranışı değişti
app.MapDelete("/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    db.Products.Remove(product); // ✅ Fiziksel silme değil — IsDeleted=true, DeletedAt=now
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// ✅ Admin: silinenleri görüntüle — IgnoreQueryFilters() global filter'ı bypass eder
app.MapGet("/products/deleted", async (AppDbContext db) =>
    await db.Products
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(p => p.IsDeleted)
            .ToListAsync());

// ✅ Geri al: silinen kaydı aktife döndür
app.MapPost("/products/{id}/restore", async (int id, AppDbContext db) =>
{
    var product = await db.Products
                          .IgnoreQueryFilters()
                          .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted);
    if (product is null) return Results.NotFound();

    product.IsDeleted  = false;
    product.DeletedAt  = null;
    await db.SaveChangesAsync();
    return Results.Ok(product);
});

app.Run();
