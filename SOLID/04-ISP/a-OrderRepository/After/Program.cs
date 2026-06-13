var builder = WebApplication.CreateBuilder(args);

var repo = new OrderRepository();
// Aynı nesne, farklı arayüzlerle servis ediliyor
builder.Services.AddSingleton<IOrderReader>(repo);
builder.Services.AddSingleton<IOrderWriter>(repo);
builder.Services.AddSingleton<IOrderReporter>(repo);
builder.Services.AddSingleton<DashboardOrderRepository>();

var app = builder.Build();

// ✅ Yalnızca okuma gerektiği yerlerde IOrderReader — fazla bağımlılık yok
app.MapGet("/orders", (IOrderReader r) => r.GetAll());
app.MapGet("/orders/status/{status}", (string status, IOrderReader r) => r.GetByStatus(status));

// ✅ Yalnızca raporlama gerektiği yerlerde IOrderReporter
app.MapGet("/orders/summary", (IOrderReporter r) => r.GetSummary());
app.MapGet("/orders/top/{count:int}", (int count, IOrderReporter r) => r.GetTopOrders(count));

// ✅ Yazma işlemlerinde IOrderWriter
app.MapPost("/orders", (Order o, IOrderWriter w) => { w.Add(o); return Results.Created($"/orders/{o.Id}", o); });
app.MapDelete("/orders/{id:int}", (int id, IOrderWriter w) => { w.Delete(id); return Results.NoContent(); });

app.Run();
