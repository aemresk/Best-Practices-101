using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Application.UseCases.PlaceOrder;

// ✅ Application katmanı: iş akışını yönetir, infrastructure'ı bilmez
public class PlaceOrderHandler
{
    private readonly IOrderRepository    _orders;
    private readonly INotificationService _notifications;

    public PlaceOrderHandler(IOrderRepository orders, INotificationService notifications)
    {
        _orders        = orders;
        _notifications = notifications;
    }

    public PlaceOrderResult Handle(PlaceOrderCommand command)
    {
        // ✅ İndirim kuralı uygulama katmanında — tek değişim sebebi
        decimal discount = command.Amount switch
        {
            >= 1000 => 0.10m,
            >= 500  => 0.05m,
            _       => 0m
        };

        // ✅ Domain entity oluşturuluyor — iş kuralları Order.Create içinde korunuyor
        var order = Order.Create(
            _orders.NextId(),
            command.CustomerName,
            command.CustomerEmail,
            command.Amount,
            discount
        );

        _orders.Save(order);

        // ✅ Bildirim: arayüz üzerinden — SMTP mi, SMS mi bilmiyoruz, bilmemize gerek de yok
        _notifications.Send(
            command.CustomerEmail,
            $"Siparişiniz #{order.Id} onaylandı",
            $"Merhaba {command.CustomerName}, {order.FinalAmount:C2} tutarındaki siparişiniz alındı."
        );

        return new PlaceOrderResult(order.Id, order.FinalAmount, order.Discount);
    }

    public IReadOnlyList<Order> GetAll() => _orders.GetAll();
}
