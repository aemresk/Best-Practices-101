var builder = WebApplication.CreateBuilder(args);

// ✅ Yeni strateji eklemek = sadece buraya AddSingleton satırı eklemek
builder.Services.AddSingleton<IPaymentStrategy, CreditCardPaymentStrategy>();
builder.Services.AddSingleton<IPaymentStrategy, PayPalPaymentStrategy>();
builder.Services.AddSingleton<IPaymentStrategy, BankTransferPaymentStrategy>();
builder.Services.AddSingleton<IPaymentStrategy, CryptoPaymentStrategy>();
builder.Services.AddSingleton<PaymentProcessor>();

var app = builder.Build();

app.MapGet("/payments", (PaymentProcessor pp) => pp.GetAll());
app.MapPost("/payments", (ProcessPaymentRequest req, PaymentProcessor pp) =>
{
    var payment = pp.Process(req);
    return Results.Created($"/payments/{payment.Id}", payment);
});

app.Run();
