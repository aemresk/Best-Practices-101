// ❌ Task — hot path'te gereksiz heap allocation:
//    Her çağrıda sonuç zaten hazır olsa bile yeni bir Task nesnesi heap'e tahsis edilir.
//    Sık çağrılan metodlarda (cache hit, in-memory lookup) bu GC baskısı yaratır.

var cache   = new ProductCache();
var service = new ProductService(cache);

// Simüle: 1000 istek — çoğu cache'ten gelecek
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

    // ❌ Task<Product> — cache hit'te bile heap allocation oluşur
    public async Task<Product?> GetAsync(int id)
    {
        // Sonuç zaten hazır — async'e gerek yok ama Task allocate edildi
        if (_store.TryGetValue(id, out var product))
            return product; // ❌ compiler Task.FromResult wrap eder → heap allocation

        await Task.Delay(1); // simüle: cache miss, disk/DB oku
        return null;
    }
}

class ProductService(ProductCache cache)
{
    // ❌ Task<Product> — her çağrıda cache hit olsa bile Task allocate
    public async Task<Product> GetProductAsync(int productId)
    {
        var cached = await cache.GetAsync(productId);
        if (cached is not null) return cached;

        await Task.Delay(100); // simüle DB sorgusu
        return new Product(productId, "Bilinmeyen", 0m);
    }
}
