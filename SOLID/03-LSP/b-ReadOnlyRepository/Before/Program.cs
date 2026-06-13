var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IRepository<Product>, ProductRepository>();
var app = builder.Build();

app.MapGet("/products", (IRepository<Product> repo) => repo.GetAll());
app.MapGet("/products/{id:int}", (int id, IRepository<Product> repo) => repo.GetById(id));
app.MapPost("/products", (Product p, IRepository<Product> repo) => { repo.Add(p); return Results.Created($"/products/{p.Id}", p); });
app.MapDelete("/products/{id:int}", (int id, IRepository<Product> repo) => { repo.Delete(id); return Results.NoContent(); });

app.Run();
