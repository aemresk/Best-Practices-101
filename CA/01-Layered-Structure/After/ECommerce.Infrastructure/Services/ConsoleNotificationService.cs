using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Services;

public class ConsoleNotificationService : INotificationService
{
    public void Send(string recipient, string subject, string body)
    {
        Console.WriteLine($"[EMAIL] Kime   : {recipient}");
        Console.WriteLine($"        Konu   : {subject}");
        Console.WriteLine($"        İçerik : {body}");
    }
}
