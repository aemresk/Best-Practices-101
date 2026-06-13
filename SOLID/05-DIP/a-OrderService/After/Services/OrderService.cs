// ✅ DIP: Yüksek seviyeli modül soyutlamalara bağımlı — somut sınıflara değil
// SmtpEmailSender → SendGridEmailSender: Program.cs'de tek satır değişir, bu sınıf açılmaz
// ConsoleOrderLogger → ElasticLogger: Program.cs'de tek satır değişir, bu sınıf açılmaz
// Unit test: FakeEmailSender inject edilir, gerçek SMTP sunucusu gerekmez
public class OrderService
{
    private static readonly List<Order> _orders = new();
    private static int _nextId = 1;

    private readonly IEmailSender _emailSender;
    private readonly IOrderLogger _logger;

    // ✅ Bağımlılıklar constructor'dan enjekte ediliyor
    public OrderService(IEmailSender emailSender, IOrderLogger logger)
    {
        _emailSender = emailSender;
        _logger      = logger;
    }

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

        // ✅ Interface üzerinden çağrı — hangi implementasyon gelirse gelsin
        _emailSender.Send(order.CustomerEmail,
            $"Siparişiniz #{order.Id} alındı",
            $"Merhaba {order.CustomerName}, siparişiniz hazırlanıyor.");

        _logger.Log($"OrderCreated | id={order.Id} | müşteri={order.CustomerName}");

        return order;
    }

    public List<Order> GetAll() => _orders;
}
