using Asp.Versioning;
using BestPracticesApi.Common.Errors;
using BestPracticesApi.Common.Middleware;
using BestPracticesApi.Extensions;
using BestPracticesApi.Features.Products;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// ═══════════════════════════════════════════════════════
// Services
// ═══════════════════════════════════════════════════════

// 01. Global Exception Handling — IExceptionHandler + ProblemDetails (RFC 9457)
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// 02. FluentValidation — assembly taramasıyla tüm validator'lar otomatik register
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// 04. API Versioning — URL segment: /api/v1/... ve /api/v2/...
builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion                   = new ApiVersion(1);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions                   = true;
    o.ApiVersionReader                    = new UrlSegmentApiVersionReader();
});

// 05. Rate Limiting — Global (sabit pencere per-IP) + "strict" named policy
builder.Services.AddApiRateLimiting();

// 06. Response Caching + 09. Output Cache & IMemoryCache
builder.Services.AddApiCaching();

// 07. Health Checks — /health, /health/live, /health/ready
builder.Services.AddApiHealthChecks();

// Uygulama servisleri
builder.Services.AddSingleton<ProductService>();

// ═══════════════════════════════════════════════════════
// Middleware Pipeline
// ═══════════════════════════════════════════════════════
var app = builder.Build();

// 08. Custom Middleware — request/response loglama (süre + status code)
app.UseMiddleware<RequestLoggingMiddleware>();

// 01. Global exception handler — tüm işlenmeyen exception'ları ProblemDetails'a çevirir
app.UseExceptionHandler();

// 05. Rate Limiter
app.UseRateLimiter();

// 06. Response Caching — Cache-Control header bazlı istemci cache
app.UseResponseCaching();

// 09. Output Cache — sunucu taraflı yanıt tamponu
app.UseOutputCache();

// ═══════════════════════════════════════════════════════
// Endpoints
// ═══════════════════════════════════════════════════════

// 07. Health Checks
app.MapApiHealthChecks();

// 04. API Versioning — URL segment versiyonlama
var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .HasApiVersion(new ApiVersion(2))
    .ReportApiVersions()
    .Build();

// v1: tam CRUD — tüm best practice'ler aktif
app.MapGroup("/api/v{version:apiVersion}/products")
   .WithApiVersionSet(versionSet)
   .MapToApiVersion(1)
   .MapProductEndpoints();

// v2: salt-okunur + sayfalama + genişletilmiş model
app.MapGroup("/api/v{version:apiVersion}/products")
   .WithApiVersionSet(versionSet)
   .MapToApiVersion(2)
   .MapProductEndpointsV2();

app.Run();
