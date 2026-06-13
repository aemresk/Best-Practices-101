using Domain;

namespace Infrastructure;

// ✅ Implementasyon Infrastructure'da — EfProductRepository ile değiştirilebilir, servis habersiz kalır
public class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _store = new();
    private int _sequence = 1;

    public void Add(Product product)    => _store.Add(product);
    public void Update(Product product) { /* In-memory: değişiklik zaten yansıdı */ }
    public int  NextId()                => _sequence++;

    public Product? GetByName(string name) =>
        _store.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<Product> GetByMinPrice(decimal minPrice) =>
        _store.Where(p => p.Price >= minPrice).ToList().AsReadOnly();
}
