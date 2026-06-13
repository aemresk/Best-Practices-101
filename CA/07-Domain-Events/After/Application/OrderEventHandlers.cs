using Domain;

namespace Application;

// ✅ Her handler tek bir yan etkiden sorumlu — bağımsız olarak eklenip çıkarılabilir
public class SendOrderConfirmationHandler
{
    public void Handle(OrderPlacedEvent e)
        => Console.WriteLine($"[EMAIL] → {e.CustomerName}: Siparişiniz alındı — {e.ProductName} x{e.Quantity}");
}

public class UpdateInventoryHandler
{
    private readonly Dictionary<string, int> _stock;

    public UpdateInventoryHandler(Dictionary<string, int> stock) => _stock = stock;

    public void Handle(OrderPlacedEvent e)
    {
        if (!_stock.ContainsKey(e.ProductName) || _stock[e.ProductName] < e.Quantity)
            throw new InvalidOperationException("Yetersiz stok");

        var before = _stock[e.ProductName];
        _stock[e.ProductName] -= e.Quantity;
        Console.WriteLine($"[STOK] {e.ProductName}: {before} → {_stock[e.ProductName]}");
    }
}

public class AuditLogHandler
{
    public void Handle(OrderPlacedEvent e)
        => Console.WriteLine($"[AUDIT] {e.OccurredAt:O} | {e.CustomerName} | {e.ProductName} x{e.Quantity}");
}
