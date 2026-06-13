// ✅ Bu sınıf hiç değişmez — yeni ödeme yöntemi = yeni IPaymentStrategy sınıfı
public class PaymentProcessor
{
    private static readonly List<Payment> _payments = new();
    private static int _nextId = 1;

    private readonly Dictionary<string, IPaymentStrategy> _strategies;

    public PaymentProcessor(IEnumerable<IPaymentStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.Method, StringComparer.OrdinalIgnoreCase);
    }

    public Payment Process(ProcessPaymentRequest request)
    {
        if (!_strategies.TryGetValue(request.Method, out var strategy))
            throw new NotSupportedException($"Desteklenmeyen ödeme yöntemi: {request.Method}");

        var status = strategy.Execute(request.Amount);
        var payment = new Payment
        {
            Id     = _nextId++,
            Amount = request.Amount,
            Method = request.Method,
            Status = status
        };
        _payments.Add(payment);
        return payment;
    }

    public List<Payment> GetAll() => _payments;
}
