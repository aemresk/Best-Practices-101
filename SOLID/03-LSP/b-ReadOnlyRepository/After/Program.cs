var builder = WebApplication.CreateBuilder(args);

// Tam CRUD için IRepository
builder.Services.AddSingleton<IRepository<Product>, ProductRepository>();
// Salt okuma için IReadRepository — yazma endpointlerinde kullanılamaz (derleme hatası verir)
builder.Services.AddSingleton<IReadRepository<Product>, ReadOnlyProductRepository>();

var app = builder.Build();

// ✅ Catalog (salt okunur) endpointleri — IReadRepository yeterli
app.MapGet("/catalog", (IReadRepository<Product> repo) => repo.GetAll());
app.MapGet("/catalog/{id:int}", (int id, IReadRepository<Product> repo) => repo.GetById(id));

// ✅ Yönetim endpointleri — tam IRepository gerektirir
app.MapGet("/products", (IRepository<Product> repo) => repo.GetAll());
app.MapPost("/products", (Product p, IRepository<Product> repo) => { repo.Add(p); return Results.Created($"/products/{p.Id}", p); });
app.MapDelete("/products/{id:int}", (int id, IRepository<Product> repo) => { repo.Delete(id); return Results.NoContent(); });

app.Run();
