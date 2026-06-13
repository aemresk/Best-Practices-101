// ❌ CQRS İhlali:
//    Tek servis hem okuma hem yazma operasyonlarını karıştırıyor.
//    - Read modeli yavaşlarsa Write operasyonları da etkilenir
//    - Read için cache/read-replica eklemek tüm servisi etkiler
//    - Model hem yazma hem okuma için optimize edilemez

var store = new List<Product>();
var service = new ProductService(store);

service.Create("Laptop", 15000m, 10);
service.Create("Mouse",    250m, 50);
service.UpdatePrice("Laptop", 14500m);

var products = service.GetAll();
Console.WriteLine("Tüm ürünler:");
foreach (var p in products)
    Console.WriteLine($"  {p.Name}: {p.Price:C2} (stok: {p.Stock})");

var summary = service.GetSummary();
Console.WriteLine($"\nÖzet: {summary.TotalProducts} ürün, ortalama fiyat {summary.AveragePrice:C2}");

record Product(int Id, string Name, decimal Price, int Stock);

// ❌ Tek servis: okuma + yazma + farklı projeksiyonlar hepsi bir arada
class ProductService(List<Product> store)
{
    private int _nextId = 1;

    // WRITE operasyonları
    public void Create(string name, decimal price, int stock)
    {
        store.Add(new Product(_nextId++, name, price, stock));
        Console.WriteLine($"[WRITE] Ürün oluşturuldu: {name}");
    }

    public void UpdatePrice(string name, decimal newPrice)
    {
        var i = store.FindIndex(p => p.Name == name);
        if (i < 0) throw new KeyNotFoundException(name);
        store[i] = store[i] with { Price = newPrice };  // ❌ Read ve Write aynı modeli paylaşıyor
        Console.WriteLine($"[WRITE] Fiyat güncellendi: {name} → {newPrice:C2}");
    }

    // READ operasyonları — aynı model, ancak ihtiyaçlar farklı
    public List<Product> GetAll() => store.ToList();

    public (int TotalProducts, decimal AveragePrice) GetSummary() =>
        (store.Count, store.Count > 0 ? store.Average(p => p.Price) : 0);
}
