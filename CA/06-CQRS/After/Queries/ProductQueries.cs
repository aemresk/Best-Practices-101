using Domain;

namespace Queries;

// ✅ Read model (DTO): query'ye özgü projeksiyon — write model'den bağımsız optimize edilebilir
public record ProductListItem(int Id, string Name, decimal Price, int Stock);
public record ProductSummary(int TotalProducts, decimal AveragePrice, int TotalStock);

public record GetAllProductsQuery();
public record GetProductSummaryQuery();

// ✅ Query Handler: yalnızca okuma — hiçbir state değiştirmiyor
public class GetAllProductsHandler
{
    private readonly List<Product> _store;

    public GetAllProductsHandler(List<Product> store) => _store = store;

    public IReadOnlyList<ProductListItem> Handle(GetAllProductsQuery _) =>
        _store.Select(p => new ProductListItem(p.Id, p.Name, p.Price, p.Stock))
              .ToList()
              .AsReadOnly();
}

public class GetProductSummaryHandler
{
    private readonly List<Product> _store;

    public GetProductSummaryHandler(List<Product> store) => _store = store;

    public ProductSummary Handle(GetProductSummaryQuery _) =>
        new(
            TotalProducts: _store.Count,
            AveragePrice:  _store.Count > 0 ? _store.Average(p => p.Price) : 0,
            TotalStock:    _store.Sum(p => p.Stock)
        );
}
