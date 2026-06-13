namespace Domain;

// ✅ Domain entity: saf iş mantığı, infrastructure'a sıfır bağımlılık
//    Yan etkiler event üzerinden dışarıya iletilir
public class Order
{
    private readonly List<IDomainEvent> _events = new();

    public string CustomerName { get; private set; } = "";
    public string ProductName  { get; private set; } = "";
    public int    Quantity     { get; private set; }

    public IReadOnlyList<IDomainEvent> DomainEvents => _events.AsReadOnly();

    public void Place(string customerName, string productName, int quantity)
    {
        CustomerName = customerName;
        ProductName  = productName;
        Quantity     = quantity;

        Console.WriteLine($"Sipariş oluşturuldu: {customerName} → {productName} x{quantity}");

        // ✅ Sadece event yayıyoruz — kim ne yapacağını bilmiyoruz, bilmemize gerek yok
        _events.Add(new OrderPlacedEvent(customerName, productName, quantity, DateTime.UtcNow));
    }

    public void ClearEvents() => _events.Clear();
}
