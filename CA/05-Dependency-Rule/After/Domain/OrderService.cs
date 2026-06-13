namespace Domain;

// ✅ Domain katmanı: Infrastructure arayüzlerine bağlı, implementasyona değil
//    Bağımlılık yönü doğru: Infrastructure → Domain (Interface)
public class OrderService
{
    private readonly IEmailSender  _emailSender;
    private readonly IAuditLogger  _auditLogger;

    public OrderService(IEmailSender emailSender, IAuditLogger auditLogger)
    {
        _emailSender = emailSender;
        _auditLogger = auditLogger;
    }

    public void Place(string customerName, decimal amount)
    {
        Console.WriteLine($"Sipariş oluşturuluyor: {customerName}, {amount:C2}");
        _emailSender.Send(customerName, $"Siparişiniz alındı: {amount:C2}");
        _auditLogger.Log($"Order.Place | {customerName} | {amount}");
    }
}

// ✅ Arayüzler Domain'de — Infrastructure bu sözleşmeleri uygular
public interface IEmailSender
{
    void Send(string recipient, string body);
}

public interface IAuditLogger
{
    void Log(string message);
}
