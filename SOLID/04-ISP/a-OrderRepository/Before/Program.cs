var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
var app = builder.Build();

app.MapGet("/orders", (IOrderRepository r) => r.GetAll());
app.MapGet("/orders/summary", (IOrderRepository r) => r.GetSummary());
app.MapPost("/orders", (Order o, IOrderRepository r) => { r.Add(o); return Results.Created($"/orders/{o.Id}", o); });

app.Run();
