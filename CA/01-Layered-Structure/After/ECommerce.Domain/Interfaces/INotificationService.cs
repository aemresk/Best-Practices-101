namespace ECommerce.Domain.Interfaces;

// ✅ Domain, bildirimi nasıl gönderileceğini bilmez — sadece arayüzü tanımlar
public interface INotificationService
{
    void Send(string recipient, string subject, string body);
}
