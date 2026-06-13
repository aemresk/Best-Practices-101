var builder = WebApplication.CreateBuilder(args);

var full = new FullNotificationService();
// ✅ Aynı nesne farklı sözleşmelerle servis ediliyor
builder.Services.AddSingleton<IEmailSender>(full);
builder.Services.AddSingleton<ISmsSender>(full);
builder.Services.AddSingleton<IPushNotifier>(full);

// ✅ Alternatif kanallar — sınırlı sorumlulukla
builder.Services.AddSingleton<MarketingSmsService>();
builder.Services.AddSingleton<MobilePushService>();

var app = builder.Build();

// ✅ Endpoint yalnızca ihtiyacı olan interface'i alıyor
app.MapPost("/notify/email", (EmailNotification n, IEmailSender svc) =>
    { svc.SendEmail(n); return Results.Ok(); });

app.MapPost("/notify/sms", (SmsNotification n, ISmsSender svc) =>
    { svc.SendSms(n); return Results.Ok(); });

app.MapPost("/notify/push", (PushNotification n, IPushNotifier svc) =>
    { svc.SendPush(n); return Results.Ok(); });

// ✅ Pazarlama SMS'i ayrı kanaldan
app.MapPost("/marketing/sms", (SmsNotification n, MarketingSmsService svc) =>
    { svc.SendSms(n); return Results.Ok(); });

app.Run();
