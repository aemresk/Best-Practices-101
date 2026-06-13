// ✅ Tek sorumluluk: yalnızca iş akışını orkestre etmek
// Değişim sebebi: sipariş iş kuralları değiştiğinde
// Validasyon, indirim, e-posta veya log kuralları değiştiğinde bu sınıf açılmaz
public class OrderService
{
    private static readonly List<Order> _orders = new();
    private static int _nextId = 1;

    private readonly OrderValidator _validator;
    private readonly DiscountCalculator _discount;
    private readonly IEmailSender _email;
    private readonly IOrderLogger _logger;

    public OrderService(
        OrderValidator validator,
        DiscountCalculator discount,
        IEmailSender email,
        IOrderLogger logger)
    {
        _validator = validator;
        _discount  = discount;
        _email     = email;
        _logger    = logger;
    }

    public Order CreateOrder(CreateOrderRequest request)
    {
        _validator.Validate(request);

        var discountRate = _discount.Calculate(request.Amount);
        var order = new Order
        {
            Id            = _nextId++,
            CustomerName  = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            Amount        = request.Amount,
            Discount      = discountRate,
            FinalAmount   = request.Amount * (1 - discountRate)
        };
        _orders.Add(order);

        _email.Send(order.CustomerEmail,
            $"Siparişiniz #{order.Id} alındı",
            $"Merhaba {order.CustomerName}, {order.FinalAmount:C2} tutarındaki siparişiniz alındı.");

        _logger.LogCreated(order);

        return order;
    }

    public List<Order> GetAll() => _orders;
}
