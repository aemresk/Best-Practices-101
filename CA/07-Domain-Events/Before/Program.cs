// ❌ Domain Events Anti-pattern:
//    Yan etkiler (e-posta, stok güncellemesi, audit) doğrudan domain metodunda.
//    - Yeni bir yan etki eklemek → Order sınıfını değiştirmek gerekiyor
//    - E-posta servisini değiştirmek → Order sınıfını değiştirmek gerekiyor
//    - Domain entity infrastructure'a doğrudan bağlı
//    - Test: Order.Place çağrıldığında e-posta gerçekten gönderilmeye çalışılır

var inventory = new Inventory();
inventory.Add("Laptop", stock: 10);

var order = new Order(new EmailNotifier(), new AuditLog(), inventory);
order.Place(customerName: "Ali Veli", productName: "Laptop", quantity: 2);

class Order(EmailNotifier emailNotifier, AuditLog auditLog, Inventory inventory)
{
    public string CustomerName { get; private set; } = "";
    public string ProductName  { get; private set; } = "";
    public int    Quantity     { get; private set; }

    public void Place(string customerName, string productName, int quantity)
    {
        CustomerName = customerName;
        ProductName  = productName;
        Quantity     = quantity;

        Console.WriteLine($"Sipariş oluşturuldu: {customerName} → {productName} x{quantity}");

        // ❌ Yan etkiler inline — her biri Order'ı infrastructure'a bağlıyor
        emailNotifier.Send(customerName, $"Siparişiniz alındı: {productName} x{quantity}");  // ❌
        inventory.Decrease(productName, quantity);                                              // ❌
        auditLog.Write($"Order.Place | {customerName} | {productName} | {quantity}");          // ❌
    }
}

// Infrastructure sınıfları — bunlar Domain'in bilmemesi gerekenler
class EmailNotifier
{
    public void Send(string to, string body)
        => Console.WriteLine($"[EMAIL] → {to}: {body}");
}

class AuditLog
{
    public void Write(string entry)
        => Console.WriteLine($"[AUDIT] {DateTime.UtcNow:O} | {entry}");
}

class Inventory
{
    private readonly Dictionary<string, int> _stock = new();
    public void Add(string product, int stock) => _stock[product] = stock;
    public void Decrease(string product, int qty)
    {
        if (!_stock.ContainsKey(product) || _stock[product] < qty)
            throw new InvalidOperationException("Yetersiz stok");
        _stock[product] -= qty;
        Console.WriteLine($"[STOK] {product}: {_stock[product] + qty} → {_stock[product]}");
    }
}
