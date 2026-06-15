using BestPracticesApi.Common.Errors;
using BestPracticesApi.Common.Filters;
using Microsoft.AspNetCore.OutputCaching;

namespace BestPracticesApi.Features.Products;

// v1 endpoint'leri — tüm best practice'ler bir arada:
//   02. FluentValidation filter
//   03. Result Pattern
//   05. Rate Limiting ("strict" policy)
//   09. Output Cache (tag bazlı invalidation)
public static class ProductEndpoints
{
    public static RouteGroupBuilder MapProductEndpoints(this RouteGroupBuilder group)
    {
        // 09. Output Cache — "Products" policy: 30s, tag: "products"
        group.MapGet("/", GetAll)
             .CacheOutput("Products");

        group.MapGet("/{id:int}", GetById);

        // 02. ValidationFilter — request body FluentValidation ile doğrulanır
        // 05. Rate Limiting — POST için "strict" policy (3/5s)
        group.MapPost("/", Create)
             .AddEndpointFilter<ValidationFilter<CreateProductRequest>>()
             .RequireRateLimiting("strict");

        group.MapPut("/{id:int}", Update)
             .AddEndpointFilter<ValidationFilter<UpdateProductRequest>>();

        group.MapDelete("/{id:int}", Delete);

        return group;
    }

    // 03. Result Pattern — servis Result<T> döner, endpoint HTTP yanıtına çevirir
    private static IResult GetAll(ProductService service) =>
        service.GetAll().ToHttpResult();

    private static IResult GetById(int id, ProductService service) =>
        service.GetById(id).ToHttpResult();

    private static async Task<IResult> Create(
        CreateProductRequest request,
        ProductService       service,
        IOutputCacheStore    outputCache,
        CancellationToken    ct)
    {
        var result = service.Create(request);

        if (result.IsSuccess)
        {
            // 09. Output Cache invalidation — "products" tag'li cache temizlenir
            await outputCache.EvictByTagAsync("products", ct);
            return Results.Created($"/api/v1/products/{result.Value!.Id}", result.Value);
        }

        return result.ToHttpResult();
    }

    private static async Task<IResult> Update(
        int                  id,
        UpdateProductRequest request,
        ProductService       service,
        IOutputCacheStore    outputCache,
        CancellationToken    ct)
    {
        var result = service.Update(id, request);

        if (result.IsSuccess)
            await outputCache.EvictByTagAsync("products", ct);

        return result.ToHttpResult();
    }

    private static async Task<IResult> Delete(
        int               id,
        ProductService    service,
        IOutputCacheStore outputCache,
        CancellationToken ct)
    {
        var result = service.Delete(id);

        if (result.IsSuccess)
        {
            await outputCache.EvictByTagAsync("products", ct);
            return Results.NoContent();
        }

        return result.ToHttpResult();
    }
}
