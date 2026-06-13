// ❌ OCP İHLALİ: Her yeni ödeme yöntemi eklendiğinde bu sınıf değiştirilmek zorunda
// "Kripto ödeme ekle" → Process() metodunu aç, yeni case ekle → mevcut kodda hata riski
public class PaymentProcessor
{
    private static readonly List<Payment> _payments = new();
    private static int _nextId = 1;

    public Payment Process(ProcessPaymentRequest request)
    {
        string status;

        // ❌ Yeni ödeme yöntemi = bu switch'e yeni case
        switch (request.Method)
        {
            case "CreditCard":
                // ❌ Kredi kartı mantığı burada gömülü
                Console.WriteLine($"[CC] Kart doğrulanıyor... {request.Amount:C2} çekiliyor");
                Console.WriteLine("[CC] 3D Secure kontrolü yapıldı");
                status = "Approved";
                break;

            case "PayPal":
                // ❌ PayPal mantığı burada gömülü
                Console.WriteLine($"[PayPal] PayPal hesabına yönlendiriliyor... {request.Amount:C2}");
                Console.WriteLine("[PayPal] PayPal token doğrulandı");
                status = "Completed";
                break;

            case "BankTransfer":
                // ❌ Banka havalesi mantığı burada gömülü
                Console.WriteLine($"[Bank] IBAN doğrulanıyor... {request.Amount:C2} havale ediliyor");
                Console.WriteLine("[Bank] Havale 1-3 iş günü içinde tamamlanacak");
                status = "Pending";
                break;

            // ❌ "Crypto" eklemek için buraya yeni case + bu metodun değişmesi gerekiyor
            default:
                throw new NotSupportedException($"Desteklenmeyen ödeme yöntemi: {request.Method}");
        }

        var payment = new Payment { Id = _nextId++, Amount = request.Amount, Method = request.Method, Status = status };
        _payments.Add(payment);
        return payment;
    }

    public List<Payment> GetAll() => _payments;
}
