using Domain;

namespace Commands;

public record CreateProductCommand(string Name, decimal Price, int Stock);

// ✅ Command Handler: sadece yazma — okuma modeli döndürmüyor
public class CreateProductHandler
{
    private readonly List<Product> _store;
    private int _nextId;

    public CreateProductHandler(List<Product> store, int startId = 1)
    {
        _store = store;
        _nextId = startId;
    }

    public int Handle(CreateProductCommand command)
    {
        var product = Product.Create(_nextId++, command.Name, command.Price, command.Stock);
        _store.Add(product);
        Console.WriteLine($"[COMMAND] Ürün oluşturuldu: {command.Name}");
        return product.Id;
    }
}
