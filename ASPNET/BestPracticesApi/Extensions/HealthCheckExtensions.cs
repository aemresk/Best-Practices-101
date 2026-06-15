using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BestPracticesApi.Extensions;

// 07. Health Checks — /health, /health/live, /health/ready endpoint'leri
public static class HealthCheckExtensions
{
    public static IServiceCollection AddApiHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("api",    () => HealthCheckResult.Healthy("API çalışıyor"),   tags: ["live"])
            .AddCheck<MemoryHealthCheck>("memory",                                  tags: ["ready"]);

        return services;
    }

    public static IEndpointRouteBuilder MapApiHealthChecks(this IEndpointRouteBuilder app)
    {
        // JSON formatında zengin yanıt
        static async Task WriteJsonResponse(HttpContext ctx, HealthReport report)
        {
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsJsonAsync(new
            {
                status   = report.Status.ToString(),
                duration = report.TotalDuration.TotalMilliseconds,
                checks   = report.Entries.Select(e => new
                {
                    name        = e.Key,
                    status      = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    duration    = e.Value.Duration.TotalMilliseconds
                })
            });
        }

        // Tüm check'ler
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate      = _ => true,
            ResponseWriter = WriteJsonResponse
        });

        // Kubernetes liveness probe — sadece "live" tag'li check'ler
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate      = c => c.Tags.Contains("live"),
            ResponseWriter = WriteJsonResponse
        });

        // Kubernetes readiness probe — sadece "ready" tag'li check'ler
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate      = c => c.Tags.Contains("ready"),
            ResponseWriter = WriteJsonResponse
        });

        return app;
    }
}

// Bellek kullanımını izleyen özel health check
internal sealed class MemoryHealthCheck : IHealthCheck
{
    private const long ThresholdMb = 512;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        var allocatedMb = GC.GetTotalMemory(forceFullCollection: false) / 1024 / 1024;
        var data        = new Dictionary<string, object> { ["allocated_mb"] = allocatedMb };

        return Task.FromResult(allocatedMb < ThresholdMb
            ? HealthCheckResult.Healthy($"Bellek normal: {allocatedMb} MB", data)
            : HealthCheckResult.Degraded($"Bellek yüksek: {allocatedMb} MB", data: data));
    }
}
