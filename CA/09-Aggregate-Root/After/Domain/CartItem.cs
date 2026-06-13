namespace Domain;

// ✅ CartItem: aggregate içindeki child entity — doğrudan dışarıya açılmıyor
public class CartItem
{
    public int     ProductId { get; private set; }
    public string  Name      { get; private set; }
    public decimal Price     { get; private set; }
    public int     Quantity  { get; private set; }

    public decimal LineTotal => Price * Quantity;

    private CartItem() { Name = ""; }

    internal static CartItem Create(int productId, string name, decimal price, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Ürün adı zorunlu");
        if (price <= 0)                      throw new ArgumentException("Fiyat sıfırdan büyük olmalı");
        if (quantity <= 0)                   throw new ArgumentException("Adet sıfırdan büyük olmalı");

        return new CartItem { ProductId = productId, Name = name, Price = price, Quantity = quantity };
    }

    internal void IncreaseQuantity(int amount)
    {
        if (amount <= 0) throw new ArgumentException("Artış miktarı sıfırdan büyük olmalı");
        Quantity += amount;
    }
}
