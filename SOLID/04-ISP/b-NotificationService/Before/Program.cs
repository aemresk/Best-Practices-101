var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<INotificationService, FullNotificationService>();
var app = builder.Build();

app.MapPost("/notify/email", (EmailNotification n, INotificationService svc) => { svc.SendEmail(n); return Results.Ok(); });
app.MapPost("/notify/sms",   (SmsNotification n,   INotificationService svc) => { svc.SendSms(n);   return Results.Ok(); });
app.MapPost("/notify/push",  (PushNotification n,  INotificationService svc) => { svc.SendPush(n);  return Results.Ok(); });

app.Run();
