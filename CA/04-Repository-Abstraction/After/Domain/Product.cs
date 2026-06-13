namespace Domain;

public class Product
{
    public int     Id    { get; private set; }
    public string  Name  { get; private set; }
    public decimal Price { get; private set; }
    public int     Stock { get; private set; }

    private Product() { Name = ""; }

    public static Product Create(int id, string name, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Ürün adı zorunlu");
        if (price <= 0)                      throw new ArgumentException("Fiyat sıfırdan büyük olmalı");
        if (stock < 0)                       throw new ArgumentException("Stok negatif olamaz");

        return new Product { Id = id, Name = name, Price = price, Stock = stock };
    }

    public void Sell(int quantity)
    {
        if (quantity <= 0)    throw new ArgumentException("Miktar sıfırdan büyük olmalı");
        if (Stock < quantity) throw new InvalidOperationException($"Yetersiz stok (mevcut: {Stock})");
        Stock -= quantity;
    }
}
