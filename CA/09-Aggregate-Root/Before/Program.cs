// ❌ Aggregate Root Anti-pattern:
//    CartItem'lar dışarıdan doğrudan manipüle ediliyor.
//    İş kuralları servis sınıfında, entity her yerden keyfi değiştirilebilir.
//    Tutarlılık garantisi yok: geçersiz durum (negatif quantity, duplicate item) mümkün.

var cart = new Cart { Id = 1, CustomerId = 42 };
var cartService = new CartService();

cartService.AddItem(cart, productId: 1, name: "Laptop", price: 15000m, quantity: 1);
cartService.AddItem(cart, productId: 2, name: "Mouse",    price: 250m,   quantity: 2);
cartService.AddItem(cart, productId: 1, name: "Laptop", price: 15000m, quantity: 1); // duplicate
cartService.PrintCart(cart);
cartService.RemoveItem(cart, productId: 2);
cartService.PrintCart(cart);

// ❌ Dışarıdan doğrudan erişim — tutarlılık kuralı devre dışı
cart.Items.Add(new CartItem { ProductId = 99, Name = "Kaçak Ürün", Price = -999m, Quantity = -5 });
Console.WriteLine($"❌ Geçersiz ürün eklendi: {cart.Items.Last().Name} fiyat={cart.Items.Last().Price}");

class CartItem
{
    public int     ProductId { get; set; }
    public string  Name      { get; set; } = "";
    public decimal Price     { get; set; }
    public int     Quantity  { get; set; }
}

class Cart
{
    public int          Id         { get; set; }
    public int          CustomerId { get; set; }
    public List<CartItem> Items    { get; set; } = new();  // ❌ public List — herkes değiştirebilir
}

class CartService
{
    private const int MaxItems = 10;

    public void AddItem(Cart cart, int productId, string name, decimal price, int quantity)
    {
        // ❌ Kurallar servis sınıfında — başka bir servis bunları atlayabilir
        if (cart.Items.Count >= MaxItems) throw new InvalidOperationException("Sepet dolu");
        if (price <= 0)                   throw new ArgumentException("Fiyat geçersiz");
        if (quantity <= 0)                throw new ArgumentException("Adet geçersiz");

        var existing = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existing != null)
            existing.Quantity += quantity;
        else
            cart.Items.Add(new CartItem { ProductId = productId, Name = name, Price = price, Quantity = quantity });

        Console.WriteLine($"[EKLE] {name} x{quantity}");
    }

    public void RemoveItem(Cart cart, int productId)
    {
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new KeyNotFoundException($"Ürün {productId} sepette yok");
        cart.Items.Remove(item);
        Console.WriteLine($"[ÇIKAR] ProductId={productId}");
    }

    public void PrintCart(Cart cart)
    {
        var total = cart.Items.Sum(i => i.Price * i.Quantity);
        Console.WriteLine($"\nSepet (Müşteri #{cart.CustomerId}) — Toplam: {total:C2}");
        foreach (var i in cart.Items)
            Console.WriteLine($"  {i.Name} x{i.Quantity} @ {i.Price:C2}");
    }
}
