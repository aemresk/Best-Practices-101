var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ReportService>();
var app = builder.Build();

app.MapGet("/report", (ReportService svc) => svc.GenerateReport());

app.Run();
