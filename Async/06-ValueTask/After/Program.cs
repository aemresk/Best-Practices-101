// ✅ ValueTask — hot path'te senkron tamamlanma durumunda sıfır heap allocation.
//    Sonuç zaten hazırsa (cache hit) struct olarak stack'te taşınır — GC baskısı yok.
//    Gerçek async gerektiğinde (cache miss) normal Task gibi davranır.

var cache   = new ProductCache();
var service = new ProductService(cache);

for (int i = 0; i < 5; i++)
{
    var product = await service.GetProductAsync(productId: 1);
    Console.WriteLine($"Ürün: {product.Name} — {product.Price:C2}");
}

record Product(int Id, string Name, decimal Price);

class ProductCache
{
    private readonly Dictionary<int, Product> _store = new()
    {
        [1] = new Product(1, "Laptop", 15000m)
    };

    // ✅ ValueTask<Product?> — cache hit'te struct döner, heap allocation yok
    public ValueTask<Product?> GetAsync(int id)
    {
        if (_store.TryGetValue(id, out var product))
            return ValueTask.FromResult<Product?>(product); // ✅ sıfır allocation

        return new ValueTask<Product?>(FetchFromDiskAsync(id)); // cache miss → gerçek async
    }

    private async Task<Product?> FetchFromDiskAsync(int id)
    {
        await Task.Delay(1).ConfigureAwait(false);
        return null;
    }
}

class ProductService(ProductCache cache)
{
    // ✅ ValueTask<Product> — sık çağrılan hot path
    public async ValueTask<Product> GetProductAsync(int productId)
    {
        var cached = await cache.GetAsync(productId);
        if (cached is not null) return cached;

        await Task.Delay(100).ConfigureAwait(false); // simüle DB sorgusu
        return new Product(productId, "Bilinmeyen", 0m);
    }
}

// ✅ ValueTask ne zaman kullanılır:
//    - Senkron tamamlanma yaygın (cache hit, in-memory lookup)
//    - Çok sık çağrılan metodlar (hot path, loop içi)
//
// ❌ ValueTask ne zaman KULLANILMAZ:
//    - Her çağrıda gerçek async gerekiyorsa → Task daha açık ve güvenli
//    - ValueTask bir değişkende saklanıp birden fazla kez await edilmemelidir
