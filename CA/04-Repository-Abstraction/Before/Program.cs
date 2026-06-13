// ❌ Repository Abstraction Eksikliği:
//    Uygulama servisi veritabanına (List / DbContext) doğrudan bağlı.
//    Depolama mekanizması değişirse tüm servis yeniden yazılmak zorunda.
//    Unit test yazmak imkânsız — gerçek List kullanılmadan test edilemiyor.

var service = new ProductService();
service.Add("Laptop", 15000m, 10);
service.Add("Mouse",    250m, 50);

var expensive = service.GetExpensive(minPrice: 1000m);
Console.WriteLine($"Pahalı ürünler ({expensive.Count} adet):");
foreach (var p in expensive)
    Console.WriteLine($"  {p.Name}: {p.Price:C2}");

service.UpdateStock("Laptop", sold: 3);
Console.WriteLine($"\nLaptop stok: {service.GetByName("Laptop")?.Stock}");

class Product
{
    public int     Id    { get; set; }
    public string  Name  { get; set; } = "";
    public decimal Price { get; set; }
    public int     Stock { get; set; }
}

class ProductService
{
    // ❌ Uygulama servisi storage implementasyonunu doğrudan barındırıyor
    private readonly List<Product> _db = new();  // gerçek kodda: new AppDbContext()
    private int _nextId = 1;

    public void Add(string name, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Ürün adı zorunlu");
        if (price <= 0)                      throw new ArgumentException("Fiyat sıfırdan büyük olmalı");
        if (stock < 0)                       throw new ArgumentException("Stok negatif olamaz");

        _db.Add(new Product { Id = _nextId++, Name = name, Price = price, Stock = stock });
        Console.WriteLine($"[DB] Ürün eklendi: {name}");
    }

    public List<Product> GetExpensive(decimal minPrice) =>
        _db.Where(p => p.Price >= minPrice).ToList();  // ❌ LINQ to List; EF'e geçince bu da değişmeli

    public Product? GetByName(string name) =>
        _db.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public void UpdateStock(string name, int sold)
    {
        var product = GetByName(name) ?? throw new KeyNotFoundException($"{name} bulunamadı");
        if (product.Stock < sold) throw new InvalidOperationException("Yetersiz stok");
        product.Stock -= sold;  // ❌ Nesneyi doğrudan manipüle ediyor, SaveChanges yok
        Console.WriteLine($"[DB] Stok güncellendi: {name} → {product.Stock}");
    }
}
