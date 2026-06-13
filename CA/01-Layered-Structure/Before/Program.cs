// ❌ Clean Architecture İHLALİ: Her şey tek dosyada, katman ayrımı yok.
//
//   Domain modeli, iş kuralları, veri erişimi ve bildirimler
//   tek bir "katman"da iç içe geçmiş durumda.
//   - İş kuralı değişirse    → bu dosya açılır
//   - Veritabanı değişirse   → bu dosya açılır
//   - E-posta şablonu değişirse → bu dosya açılır
//   - UI (konsol→web) değişirse → her şey yeniden yazılır

var orders = new List<Order>();
var nextId = 1;

Console.WriteLine("=== Sipariş Yönetim Sistemi ===\n");
PlaceOrder("Ahmet Yılmaz", "ahmet@ornek.com", 1500m);
PlaceOrder("Ayşe Demir",   "ayse@ornek.com",   300m);
ListOrders();

void PlaceOrder(string name, string email, decimal amount)
{
    // ❌ Validasyon — iş kuralı, ama infrastructure koduyla karışık
    if (string.IsNullOrWhiteSpace(name))  throw new Exception("Ad zorunlu");
    if (!email.Contains('@'))             throw new Exception("Geçersiz e-posta");
    if (amount <= 0)                      throw new Exception("Tutar sıfırdan büyük olmalı");

    // ❌ İndirim hesabı — domain logic, ama veri erişimiyle iç içe
    decimal discount = amount switch
    {
        >= 1000 => 0.10m,
        >= 500  => 0.05m,
        _       => 0m
    };

    // ❌ Veri saklama — infrastructure concern, business logic'in tam ortasında
    var order = new Order(nextId++, name, email, amount, discount);
    orders.Add(order);

    // ❌ Bildirim — SMTP/infrastructure concern, domain metodunun içinde
    Console.WriteLine($"[EMAIL] → {email}: #{order.Id} no'lu siparişiniz onaylandı ({order.FinalAmount:C2})");
    Console.WriteLine($"[LOG]   Sipariş kaydedildi: id={order.Id}");
}

void ListOrders()
{
    Console.WriteLine("\n--- Tüm Siparişler ---");
    foreach (var o in orders)
        Console.WriteLine($"  #{o.Id} | {o.CustomerName} | {o.FinalAmount:C2} | %{o.Discount * 100:0} indirim");
}

// ❌ "Model" — sadece veri taşıyıcı, davranışı yok (Anemic Domain Model)
record Order(int Id, string CustomerName, string Email, decimal Amount, decimal Discount)
{
    public decimal FinalAmount => Amount * (1 - Discount);
}
