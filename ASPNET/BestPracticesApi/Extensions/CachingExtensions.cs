namespace BestPracticesApi.Extensions;

// 06. Response Caching + 09. Output Caching & IMemoryCache
public static class CachingExtensions
{
    public static IServiceCollection AddApiCaching(this IServiceCollection services)
    {
        // 09-a: IMemoryCache — servis katmanında programatik key-value cache
        services.AddMemoryCache();

        // 09-b: Output Cache — HTTP yanıtını sunucu tarafında tampon bellekte saklar
        //        Tag bazlı invalidation ile POST/PUT/DELETE'de anında temizlenebilir
        services.AddOutputCache(options =>
        {
            // Varsayılan base policy: hiç cache yapma (açıkça belirtilmeli)
            options.AddBasePolicy(b => b.NoCache());

            // "Products" policy: 30s cache, "products" tag ile evict edilebilir
            options.AddPolicy("Products", b => b
                .Expire(TimeSpan.FromSeconds(30))
                .Tag("products")
                .SetVaryByQuery("page", "pageSize"));
        });

        // 06: Response Caching — istemci taraflı Cache-Control header desteği
        services.AddResponseCaching();

        return services;
    }
}
