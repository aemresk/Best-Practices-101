// ❌ DIP İHLALİ: Yüksek seviyeli modül somut sınıflara doğrudan bağımlı
// SmtpEmailSender'ı SendGrid ile değiştirmek için bu sınıfı açmak gerekir
// FileLogger'ı ElasticSearch ile değiştirmek için bu sınıfı açmak gerekir
// Unit test yazarken gerçek SMTP sunucusu olmadan test etmek imkânsız
public class OrderService
{
    private static readonly List<Order> _orders = new();
    private static int _nextId = 1;

    // ❌ Somut sınıflar doğrudan field olarak yaratılmış
    private readonly SmtpEmailSender _emailSender = new();
    private readonly FileLogger _logger           = new();

    public Order CreateOrder(CreateOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName))
            throw new ArgumentException("Müşteri adı zorunlu");

        var order = new Order
        {
            Id            = _nextId++,
            CustomerName  = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            Amount        = request.Amount
        };
        _orders.Add(order);

        // ❌ Somut SmtpEmailSender kullanılıyor — swap edilemez
        _emailSender.Send(order.CustomerEmail,
            $"Siparişiniz #{order.Id} alındı",
            $"Merhaba {order.CustomerName}, siparişiniz hazırlanıyor.");

        // ❌ Somut FileLogger kullanılıyor — swap edilemez
        _logger.Log($"OrderCreated | id={order.Id} | müşteri={order.CustomerName}");

        return order;
    }

    public List<Order> GetAll() => _orders;
}
