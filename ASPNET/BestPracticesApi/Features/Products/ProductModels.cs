namespace BestPracticesApi.Features.Products;

// v1 domain modeli
public record Product(int Id, string Name, decimal Price, int Stock);

// İstek modelleri
public record CreateProductRequest(string Name, decimal Price, int Stock);
public record UpdateProductRequest(string Name, decimal Price, int Stock);

// v2: genişletilmiş yanıt modeli
public record ProductV2(int Id, string Name, decimal Price, int Stock, DateTime CreatedAt, string Category);

// v2: sayfalı yanıt
public record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);
