// ✅ DIP: Soyutlama — yüksek + düşük seviyeli modüller bu interface'e bağlı
public interface IEmailSender
{
    void Send(string to, string subject, string body);
}

// ✅ SMTP implementasyonu — interface arkasında gizli
public class SmtpEmailSender : IEmailSender
{
    public void Send(string to, string subject, string body) =>
        Console.WriteLine($"[SMTP] → {to} | {subject}");
}

// ✅ Test/geliştirme implementasyonu — DI ile kolayca swap edilir
public class ConsoleEmailSender : IEmailSender
{
    public void Send(string to, string subject, string body) =>
        Console.WriteLine($"[CONSOLE-EMAIL] → {to} | {subject} | {body}");
}
