var builder = WebApplication.CreateBuilder(args);

// ✅ Implementasyonu buradan yönetiyoruz — OrderService'e dokunmadan swap edilebilir
// Prod: SmtpEmailSender, Test: ConsoleEmailSender
builder.Services.AddSingleton<IEmailSender, ConsoleEmailSender>();
builder.Services.AddSingleton<IOrderLogger, ConsoleOrderLogger>();
builder.Services.AddSingleton<OrderService>();

var app = builder.Build();

app.MapGet("/orders", (OrderService svc) => svc.GetAll());
app.MapPost("/orders", (CreateOrderRequest req, OrderService svc) =>
{
    var order = svc.CreateOrder(req);
    return Results.Created($"/orders/{order.Id}", order);
});

app.Run();
