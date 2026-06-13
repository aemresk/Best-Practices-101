// Somut e-posta gönderici — DIP öncesinde doğrudan yaratılıyor
public class SmtpEmailSender
{
    public void Send(string to, string subject, string body)
    {
        // Gerçek projede SmtpClient kullanılır
        Console.WriteLine($"[SMTP] → {to} | {subject}");
    }
}
