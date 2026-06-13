// ============================================================
// AFTER — Doğru Yaklaşım
// ============================================================

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Repository DI'a kaydediliyor — endpoint'ler interface'e bağımlı, implementasyona değil
builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Categories.Any())
    {
        var elektronik = new Category { Name = "Elektronik" };
        var giyim      = new Category { Name = "Giyim" };
        db.Categories.AddRange(elektronik, giyim);
        db.SaveChanges();
        db.Products.AddRange(
            new Product { Name = "Laptop",    Price = 25000, IsActive = true,  CategoryId = elektronik.Id },
            new Product { Name = "Mouse",     Price = 350,   IsActive = true,  CategoryId = elektronik.Id },
            new Product { Name = "Klavye",    Price = 450,   IsActive = false, CategoryId = elektronik.Id },
            new Product { Name = "T-Shirt",   Price = 200,   IsActive = true,  CategoryId = giyim.Id },
            new Product { Name = "Eski Şort", Price = 50,    IsActive = false, CategoryId = giyim.Id }
        );
        db.SaveChanges();
    }
}

// ✅ Endpoint'ler ince (thin) — sadece repository metodunu çağırıyor
// ✅ "Aktif ürün" tanımı endpoint'te yok — ProductRepository.ActiveProducts'ta
app.MapGet("/products", async (IProductRepository repo) =>
    await repo.GetActiveAsync());

app.MapGet("/products/cheap", async (IProductRepository repo) =>
    await repo.GetActiveCheaperThanAsync(maxPrice: 500));

app.MapGet("/categories/{id}/products", async (int id, IProductRepository repo) =>
    await repo.GetActiveByCategoryAsync(id));

// ✅ Test edilebilir: IProductRepository mock'lanabilir — gerçek DB gerekmez
app.MapGet("/products/{id}", async (int id, IProductRepository repo) =>
{
    var product = await repo.GetActiveByIdAsync(id);
    return product is null ? Results.NotFound() : Results.Ok(product);
});

app.MapPost("/products", async (Product product, IProductRepository repo) =>
{
    await repo.AddAsync(product);
    return Results.Created($"/products/{product.Id}", product);
});

app.MapDelete("/products/{id}", async (int id, IProductRepository repo) =>
{
    await repo.DeleteAsync(id);
    return Results.NoContent();
});

app.Run();
