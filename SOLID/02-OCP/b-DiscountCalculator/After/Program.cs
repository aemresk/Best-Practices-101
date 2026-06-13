var builder = WebApplication.CreateBuilder(args);

// ✅ Yeni indirim kuralı = sadece buraya bir satır eklemek
builder.Services.AddSingleton<IDiscountRule, PremiumMemberDiscountRule>();
builder.Services.AddSingleton<IDiscountRule, FirstOrderDiscountRule>();
builder.Services.AddSingleton<IDiscountRule, CouponDiscountRule>();
builder.Services.AddSingleton<IDiscountRule, CartAmountDiscountRule>();
builder.Services.AddSingleton<IDiscountRule, BlackFridayDiscountRule>();
builder.Services.AddSingleton<DiscountCalculator>();

var app = builder.Build();

app.MapPost("/calculate-discount", (CalculateRequest req, DiscountCalculator calc) =>
    calc.Calculate(req));

app.Run();
