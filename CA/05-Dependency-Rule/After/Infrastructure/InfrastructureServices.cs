using Domain;

namespace Infrastructure;

// ✅ Infrastructure, Domain arayüzlerini uygular — Domain bu sınıfları tanımaz
public class SmtpEmailSender : IEmailSender
{
    public void Send(string recipient, string body)
        => Console.WriteLine($"[SMTP] → {recipient}: {body}");
}

public class FileAuditLogger : IAuditLogger
{
    public void Log(string message)
        => Console.WriteLine($"[FILE LOG] {DateTime.UtcNow:O} | {message}");
}

// ✅ Alternatif implementasyonlar kolayca takılabilir
public class NoOpEmailSender : IEmailSender
{
    public void Send(string recipient, string body)
        => Console.WriteLine($"[NO-OP EMAIL] (test ortamı — gönderilmedi)");
}
