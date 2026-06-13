using Domain;

namespace Application;

// ✅ Uygulama servisi: sadece IProductRepository'e bağlı, implementasyonu bilmez
public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository) => _repository = repository;

    public void Add(string name, decimal price, int stock)
    {
        var product = Product.Create(_repository.NextId(), name, price, stock);
        _repository.Add(product);
        Console.WriteLine($"[Uygulama] Ürün eklendi: {name}");
    }

    public IReadOnlyList<Product> GetExpensive(decimal minPrice) =>
        _repository.GetByMinPrice(minPrice);

    public Product? GetByName(string name) =>
        _repository.GetByName(name);

    public void Sell(string name, int quantity)
    {
        var product = _repository.GetByName(name)
            ?? throw new KeyNotFoundException($"'{name}' ürünü bulunamadı");

        product.Sell(quantity);         // ✅ iş kuralı entity içinde
        _repository.Update(product);    // ✅ repository üzerinden kayıt
        Console.WriteLine($"[Uygulama] {quantity} adet {name} satıldı → Stok: {product.Stock}");
    }
}
