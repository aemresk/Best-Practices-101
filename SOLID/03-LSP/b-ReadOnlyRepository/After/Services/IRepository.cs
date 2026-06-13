// ✅ LSP: Sözleşmeler gerçekten karşılanabilecek kadar bölünmüş

// Okuma sözleşmesi — her repo bu sözleşmeyi tam karşılayabilir
public interface IReadRepository<T>
{
    T? GetById(int id);
    List<T> GetAll();
}

// Yazma sözleşmesi — yalnızca yazma destekleyen repolar implement eder
public interface IWriteRepository<T>
{
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
}

// İkisini birleştiren tam repository — tam CRUD desteği için
public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T> { }

// ✅ Tam repo — IRepository sözleşmesini eksiksiz karşılıyor
public class ProductRepository : IRepository<Product>
{
    private static readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Laptop",   Price = 15000, Stock = 5  },
        new Product { Id = 2, Name = "Mouse",    Price = 250,   Stock = 50 },
        new Product { Id = 3, Name = "Keyboard", Price = 800,   Stock = 30 }
    };
    private static int _nextId = 4;

    public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);
    public List<Product> GetAll()   => _products;
    public void Add(Product p)      { p.Id = _nextId++; _products.Add(p); }
    public void Update(Product p)   { var i = _products.FindIndex(x => x.Id == p.Id); if (i >= 0) _products[i] = p; }
    public void Delete(int id)      => _products.RemoveAll(p => p.Id == id);
}

// ✅ LSP uyumlu: Yalnızca IReadRepository implement ediyor — yazma metodları yok
// IReadRepository yerine bu kullanılabilir, hiçbir sözleşme ihlali olmaz
public class ReadOnlyProductRepository : IReadRepository<Product>
{
    private static readonly List<Product> _catalog = new()
    {
        new Product { Id = 1, Name = "Laptop",  Price = 15000, Stock = 5  },
        new Product { Id = 2, Name = "Mouse",   Price = 250,   Stock = 50 }
    };

    public Product? GetById(int id) => _catalog.FirstOrDefault(p => p.Id == id);
    public List<Product> GetAll()   => _catalog;
    // ✅ Add/Update/Delete yok — sözleşmede olmayan şeyi vaat etmiyoruz
}
