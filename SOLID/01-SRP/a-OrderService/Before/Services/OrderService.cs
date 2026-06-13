// ❌ SRP İHLALİ: Bu sınıfın 5 farklı değişim sebebi var.
//
//   1. İndirim kuralları değişirse  → bu sınıf açılır
//   2. E-posta şablonu değişirse    → bu sınıf açılır
//   3. Log formatı değişirse        → bu sınıf açılır
//   4. Sipariş validasyonu değişirse → bu sınıf açılır
//   5. Veri saklama yöntemi değişirse → bu sınıf açılır

public class OrderService
{
    private static readonly List<Order> _orders = new();
    private static int _nextId = 1;

    public Order CreateOrder(CreateOrderRequest request)
    {
        // ❌ 1. Sorumluluk: Validasyon
        if (string.IsNullOrWhiteSpace(request.CustomerName))
            throw new ArgumentException("Müşteri adı zorunlu");
        if (string.IsNullOrWhiteSpace(request.CustomerEmail) || !request.CustomerEmail.Contains('@'))
            throw new ArgumentException("Geçerli bir e-posta adresi giriniz");
        if (request.Amount <= 0)
            throw new ArgumentException("Tutar sıfırdan büyük olmalı");

        // ❌ 2. Sorumluluk: İndirim hesaplama
        decimal discount = 0;
        if (request.Amount >= 5000)      discount = 0.15m;
        else if (request.Amount >= 1000) discount = 0.10m;
        else if (request.Amount >= 500)  discount = 0.05m;

        // ❌ 3. Sorumluluk: Sipariş oluşturma / veri erişimi
        var order = new Order
        {
            Id          = _nextId++,
            CustomerName  = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            Amount      = request.Amount,
            Discount    = discount,
            FinalAmount = request.Amount * (1 - discount)
        };
        _orders.Add(order);

        // ❌ 4. Sorumluluk: E-posta gönderme (SMTP detayları burada)
        Console.WriteLine($"[EMAIL] Kime   : {order.CustomerEmail}");
        Console.WriteLine($"[EMAIL] Konu   : Siparişiniz #{order.Id} alındı");
        Console.WriteLine($"[EMAIL] İçerik : Merhaba {order.CustomerName}, {order.FinalAmount:C2} tutarındaki siparişiniz alındı.");

        // ❌ 5. Sorumluluk: Loglama
        Console.WriteLine($"[LOG] {DateTime.UtcNow:O} | OrderCreated | id={order.Id} müşteri={order.CustomerName} tutar={order.FinalAmount}");

        return order;
    }

    public List<Order> GetAll() => _orders;
}
