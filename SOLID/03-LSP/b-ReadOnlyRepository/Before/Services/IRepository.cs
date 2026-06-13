// ❌ LSP İHLALİ: Tek bir "şişman" repository sözleşmesi
// ReadOnly implementasyonu bu sözleşmeyi tam karşılayamaz
public interface IRepository<T>
{
    T? GetById(int id);
    List<T> GetAll();
    void Add(T entity);       // ❌ ReadOnly repo bunu destekleyemez
    void Update(T entity);    // ❌ ReadOnly repo bunu destekleyemez
    void Delete(int id);      // ❌ ReadOnly repo bunu destekleyemez
}

// ✅ Normal repo — sorun yok
public class ProductRepository : IRepository<Product>
{
    private static readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Laptop",  Price = 15000, Stock = 5  },
        new Product { Id = 2, Name = "Mouse",   Price = 250,   Stock = 50 },
        new Product { Id = 3, Name = "Keyboard",Price = 800,   Stock = 30 }
    };
    private static int _nextId = 4;

    public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);
    public List<Product> GetAll()   => _products;
    public void Add(Product p)      { p.Id = _nextId++; _products.Add(p); }
    public void Update(Product p)   { var i = _products.FindIndex(x => x.Id == p.Id); if (i >= 0) _products[i] = p; }
    public void Delete(int id)      => _products.RemoveAll(p => p.Id == id);
}

// ❌ LSP İHLALİ: ReadOnlyProductRepository, IRepository sözleşmesini BOZUYOR
// IRepository yerine bu kullanıldığında Add/Update/Delete exception fırlatır
public class ReadOnlyProductRepository : IRepository<Product>
{
    private static readonly List<Product> _catalog = new()
    {
        new Product { Id = 1, Name = "Laptop",  Price = 15000, Stock = 5  },
        new Product { Id = 2, Name = "Mouse",   Price = 250,   Stock = 50 }
    };

    public Product? GetById(int id) => _catalog.FirstOrDefault(p => p.Id == id);
    public List<Product> GetAll()   => _catalog;

    // ❌ Sözleşme ihlali — üst interface Add desteği garantiliyor ama bu impl fırlatıyor
    public void Add(Product entity)    => throw new NotImplementedException("Bu repo salt okunur");
    public void Update(Product entity) => throw new NotImplementedException("Bu repo salt okunur");
    public void Delete(int id)         => throw new NotImplementedException("Bu repo salt okunur");
}
