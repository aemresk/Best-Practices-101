using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace BestPracticesApi.Extensions;

// 05. Rate Limiting — Global (sabit pencere per-IP) + named "strict" policy
public static class RateLimitingExtensions
{
    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Global: tüm endpoint'ler için — IP başına 10 istek / 10 saniye
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: ctx.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit          = 10,
                        Window               = TimeSpan.FromSeconds(10),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit           = 2
                    }));

            // "strict" policy: POST/PUT gibi mutasyon endpoint'leri için
            // 3 istek / 5 saniye (brute-force koruması)
            options.AddFixedWindowLimiter("strict", o =>
            {
                o.PermitLimit          = 3;
                o.Window               = TimeSpan.FromSeconds(5);
                o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                o.QueueLimit           = 0;
            });
        });

        return services;
    }
}
