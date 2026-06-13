var builder = WebApplication.CreateBuilder(args);

// ✅ Format veya kaynak değiştirmek için yalnızca bu iki satırı değiştir
builder.Services.AddSingleton<IReportExporter, ExcelExporter>();  // PdfExporter ile değiştirilebilir
builder.Services.AddSingleton<IDataLoader, DatabaseDataLoader>(); // ApiDataLoader ile değiştirilebilir
builder.Services.AddSingleton<ReportService>();

var app = builder.Build();

app.MapGet("/report", (ReportService svc) => svc.GenerateReport());

// ✅ PDF raporu için farklı endpoint — farklı ihracatçı inject edilir
app.MapGet("/report/pdf", (IDataLoader loader) =>
{
    var svc = new ReportService(new PdfExporter(), loader);
    return svc.GenerateReport();
});

app.Run();
