using BestPracticesApi.Common.Errors;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace BestPracticesApi.Features.Products;

// 03. Result Pattern — servis exception fırlatmaz, Result<T> döner
// 09. IMemoryCache — GetAll sonucu 30 saniye cache'lenir
public sealed class ProductService
{
    private const string CacheKey = "products:all";

    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<int, Product> _store;
    private int _nextId = 4;

    public ProductService(IMemoryCache cache)
    {
        _cache = cache;
        _store = new ConcurrentDictionary<int, Product>(new Dictionary<int, Product>
        {
            [1] = new(1, "Laptop",    15_000m, 10),
            [2] = new(2, "Mouse",        250m, 50),
            [3] = new(3, "Keyboard",     750m, 30)
        });
    }

    public Result<IReadOnlyList<Product>> GetAll()
    {
        // IMemoryCache: sık erişilen liste için önbellekte tut
        var products = _cache.GetOrCreate(CacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
            return (IReadOnlyList<Product>)_store.Values.OrderBy(p => p.Id).ToList();
        });

        return Result.Success(products!);
    }

    public Result<Product> GetById(int id)
    {
        return _store.TryGetValue(id, out var product)
            ? Result.Success(product)
            : Result.Failure<Product>(AppError.NotFound("Product"));
    }

    public Result<Product> Create(CreateProductRequest request)
    {
        if (_store.Values.Any(p => p.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
            return Result.Failure<Product>(
                AppError.Conflict("Product", $"'{request.Name}' adlı ürün zaten mevcut."));

        var id      = Interlocked.Increment(ref _nextId);
        var product = new Product(id, request.Name, request.Price, request.Stock);

        _store[id] = product;
        _cache.Remove(CacheKey); // IMemoryCache invalidation
        return Result.Success(product);
    }

    public Result<Product> Update(int id, UpdateProductRequest request)
    {
        if (!_store.ContainsKey(id))
            return Result.Failure<Product>(AppError.NotFound("Product"));

        var updated = new Product(id, request.Name, request.Price, request.Stock);
        _store[id]  = updated;
        _cache.Remove(CacheKey);
        return Result.Success(updated);
    }

    public Result<bool> Delete(int id)
    {
        if (!_store.TryRemove(id, out _))
            return Result.Failure<bool>(AppError.NotFound("Product"));

        _cache.Remove(CacheKey);
        return Result.Success(true);
    }
}
