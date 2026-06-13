// ============================================================
// BEFORE — Yaygın Hatalar
// Bu dosyada kasıtlı olarak yanlış pratikler gösterilmektedir.
// ============================================================

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// ❌ DbContext DI'a kaydedilmemiş
// ❌ Veritabanı başlangıcı manuel ve dağınık
using (var context = new AppDbContext())
{
    context.Database.EnsureCreated();
}

// ❌ Her endpoint'te new ile DbContext oluşturuluyor
// ❌ Dispose edilmiyor — memory leak riski
// ❌ Hata durumunda context açık kalıyor
app.MapGet("/products", () =>
{
    var context = new AppDbContext();
    return context.Products.ToList(); // ❌ async değil, thread blocking
});

// ❌ Yine new ile yeni bir instance — bağımsız transaction riski
app.MapPost("/products", (Product product) =>
{
    var context = new AppDbContext();
    context.Products.Add(product);
    context.SaveChanges(); // ❌ async değil
    return Results.Created($"/products/{product.Id}", product);
});

app.MapDelete("/products/{id}", (int id) =>
{
    // ❌ Her işlem için ayrı context — tutarlılık garantisi yok
    var context = new AppDbContext();
    var product = context.Products.Find(id);
    if (product is null) return Results.NotFound();
    context.Products.Remove(product);
    context.SaveChanges();
    return Results.NoContent();
});

app.Run();
