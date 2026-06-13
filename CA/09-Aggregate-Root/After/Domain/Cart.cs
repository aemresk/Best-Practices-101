namespace Domain;

// ✅ Aggregate Root: Cart, CartItem'ların tek giriş noktasıdır.
//    Tüm iş kuralları burada — dışarıdan CartItem doğrudan değiştirilemez.
public class Cart
{
    private const int MaxItems = 10;
    private readonly List<CartItem> _items = new();

    public int Id         { get; private set; }
    public int CustomerId { get; private set; }

    // ✅ IReadOnlyList: okuma izni var, değiştirme yok
    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
    public decimal Total => _items.Sum(i => i.LineTotal);

    private Cart() { }

    public static Cart Create(int id, int customerId) =>
        new() { Id = id, CustomerId = customerId };

    // ✅ Tek giriş noktası — kural burada, atlanamaz
    public void AddItem(int productId, string name, decimal price, int quantity)
    {
        if (_items.Count >= MaxItems)
            throw new InvalidOperationException($"Sepette en fazla {MaxItems} ürün olabilir");

        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
        {
            existing.IncreaseQuantity(quantity);  // internal metod — dışarıdan çağrılamaz
            Console.WriteLine($"[EKLE] {name} miktarı güncellendi: x{existing.Quantity}");
        }
        else
        {
            _items.Add(CartItem.Create(productId, name, price, quantity));
            Console.WriteLine($"[EKLE] {name} x{quantity}");
        }
    }

    public void RemoveItem(int productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new KeyNotFoundException($"Ürün {productId} sepette bulunamadı");
        _items.Remove(item);
        Console.WriteLine($"[ÇIKAR] {item.Name}");
    }

    public void Clear()
    {
        _items.Clear();
        Console.WriteLine("[TEMİZLE] Sepet boşaltıldı");
    }

    public void PrintCart()
    {
        Console.WriteLine($"\nSepet (Müşteri #{CustomerId}) — Toplam: {Total:C2}");
        foreach (var i in Items)
            Console.WriteLine($"  {i.Name} x{i.Quantity} @ {i.Price:C2} = {i.LineTotal:C2}");
    }
}
