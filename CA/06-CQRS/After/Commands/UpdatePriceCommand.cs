using Domain;

namespace Commands;

public record UpdatePriceCommand(string ProductName, decimal NewPrice);

public class UpdatePriceHandler
{
    private readonly List<Product> _store;

    public UpdatePriceHandler(List<Product> store) => _store = store;

    public void Handle(UpdatePriceCommand command)
    {
        var product = _store.FirstOrDefault(p => p.Name == command.ProductName)
            ?? throw new KeyNotFoundException(command.ProductName);

        product.UpdatePrice(command.NewPrice);
        Console.WriteLine($"[COMMAND] Fiyat güncellendi: {command.ProductName} → {command.NewPrice:C2}");
    }
}
