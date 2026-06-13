// ❌ Dependency Rule İhlali:
//    Domain katmanı, Infrastructure sınıflarını doğrudan kullanıyor.
//    Bağımlılık yönü yanlış: Domain → Infrastructure
//
//    Sonuç:
//    - SMTP sunucusu değişirse Domain sınıfı değişmek zorunda
//    - Domain test edilirken gerçek e-posta göndermeye çalışılır
//    - Domain artık "saf iş mantığı" değil, infrastructure bilgisi taşıyor

var orderService = new OrderService(new SmtpEmailSender(), new FileAuditLogger());
orderService.Place("Mehmet Kaya", 2500m);

// ❌ Domain sınıfı — infrastructure'a doğrudan bağlı
class OrderService
{
    private readonly SmtpEmailSender _emailSender;   // ❌ Infrastructure sınıfı Domain'de
    private readonly FileAuditLogger _auditLogger;   // ❌ Infrastructure sınıfı Domain'de

    public OrderService(SmtpEmailSender emailSender, FileAuditLogger auditLogger)
    {
        _emailSender = emailSender;
        _auditLogger = auditLogger;
    }

    public void Place(string customerName, decimal amount)
    {
        Console.WriteLine($"Sipariş oluşturuluyor: {customerName}, {amount:C2}");
        _emailSender.Send(customerName, $"Siparişiniz alındı: {amount:C2}");  // ❌ Infrastructure çağrısı
        _auditLogger.Log($"Order.Place | {customerName} | {amount}");          // ❌ Infrastructure çağrısı
    }
}

// ❌ Infrastructure sınıfları — bunlar Domain'in bilmemesi gereken detaylar
class SmtpEmailSender
{
    public void Send(string to, string body)
        => Console.WriteLine($"[SMTP] → {to}: {body}");
}

class FileAuditLogger
{
    public void Log(string message)
        => Console.WriteLine($"[FILE LOG] {DateTime.UtcNow:O} | {message}");
}
