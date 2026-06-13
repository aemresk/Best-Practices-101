// ✅ Tek sorumluluk: e-posta gönderme sözleşmesi
public interface IEmailSender
{
    void Send(string to, string subject, string body);
}

// ✅ Konsol implementasyonu — prod'da SmtpEmailSender ile değiştirilir
public class ConsoleEmailSender : IEmailSender
{
    public void Send(string to, string subject, string body)
    {
        Console.WriteLine($"[EMAIL] Kime   : {to}");
        Console.WriteLine($"[EMAIL] Konu   : {subject}");
        Console.WriteLine($"[EMAIL] İçerik : {body}");
    }
}
