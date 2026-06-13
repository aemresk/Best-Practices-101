var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<DiscountCalculator>();
var app = builder.Build();

app.MapPost("/calculate-discount", (CalculateRequest req, DiscountCalculator calc) =>
    calc.Calculate(req));

app.Run();
