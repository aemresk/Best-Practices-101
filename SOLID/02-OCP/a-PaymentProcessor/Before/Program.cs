var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<PaymentProcessor>();
var app = builder.Build();

app.MapGet("/payments", (PaymentProcessor pp) => pp.GetAll());
app.MapPost("/payments", (ProcessPaymentRequest req, PaymentProcessor pp) =>
{
    var payment = pp.Process(req);
    return Results.Created($"/payments/{payment.Id}", payment);
});

app.Run();
