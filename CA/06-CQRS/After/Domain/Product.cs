namespace Domain;

// ✅ Write model: zengin domain entity — command'lar bunu değiştirir
public class Product
{
    public int     Id    { get; private set; }
    public string  Name  { get; private set; }
    public decimal Price { get; private set; }
    public int     Stock { get; private set; }

    private Product() { Name = ""; }

    public static Product Create(int id, string name, decimal price, int stock) =>
        new() { Id = id, Name = name, Price = price, Stock = stock };

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0) throw new ArgumentException("Fiyat sıfırdan büyük olmalı");
        Price = newPrice;
    }
}
