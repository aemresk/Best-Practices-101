using Domain;

var cart = Cart.Create(id: 1, customerId: 42);

cart.AddItem(productId: 1, name: "Laptop", price: 15000m, quantity: 1);
cart.AddItem(productId: 2, name: "Mouse",    price: 250m,   quantity: 2);
cart.AddItem(productId: 1, name: "Laptop", price: 15000m, quantity: 1); // duplicate → quantity artırılır
cart.PrintCart();

cart.RemoveItem(productId: 2);
cart.PrintCart();

// ✅ Dışarıdan geçersiz ürün eklemeye çalışalım
try
{
    // cart.Items.Add(...) ← derleme hatası: IReadOnlyList, Add metodu içermez
    cart.AddItem(productId: 99, name: "", price: -999m, quantity: -5); // ❌ kural içeride yakalar
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Kural ihlali engellendi: {ex.Message}");
}
