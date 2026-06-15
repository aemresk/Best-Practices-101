using BestPracticesApi.Common.Errors;

namespace BestPracticesApi.Features.Products;

// 04. API Versioning — v2: sayfalama + genişletilmiş yanıt modeli
public static class ProductEndpointsV2
{
    public static RouteGroupBuilder MapProductEndpointsV2(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllPaged)
             .CacheOutput("Products");

        group.MapGet("/{id:int}", GetById);

        return group;
    }

    // v2: query param ile sayfalama
    private static IResult GetAllPaged(
        ProductService service,
        int            page     = 1,
        int            pageSize = 10)
    {
        page     = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var result = service.GetAll();
        if (result.IsFailure) return result.ToHttpResult();

        var all   = result.Value!;
        var items = all
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToV2)
            .ToList();

        return Results.Ok(new PagedResult<ProductV2>(items, all.Count, page, pageSize));
    }

    // v2: genişletilmiş yanıt modeli (CreatedAt + Category eklendi)
    private static IResult GetById(int id, ProductService service)
    {
        var result = service.GetById(id);
        return result.IsSuccess
            ? Results.Ok(MapToV2(result.Value!))
            : result.ToHttpResult();
    }

    private static ProductV2 MapToV2(Product p) =>
        new(p.Id, p.Name, p.Price, p.Stock,
            CreatedAt: DateTime.UtcNow.AddDays(-p.Id * 30),
            Category:  p.Price >= 10_000m ? "Üst Segment" : "Standart");
}
