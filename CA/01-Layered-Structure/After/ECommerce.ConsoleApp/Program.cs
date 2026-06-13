// ✅ Composition Root: bağımlılıklar yalnızca burada bir araya getirilir.
//    Application ve Domain katmanları Infrastructure'ı hiç tanımaz.
//    Gerçek projede bu kısmın yerini DI container (Microsoft.Extensions.DI) alır.

using ECommerce.Application.UseCases.PlaceOrder;
using ECommerce.Infrastructure.Repositories;
using ECommerce.Infrastructure.Services;

var repository    = new InMemoryOrderRepository();      // Infrastructure
var notifications = new ConsoleNotificationService();   // Infrastructure
var handler       = new PlaceOrderHandler(repository, notifications); // Application

Console.WriteLine("=== Sipariş Yönetim Sistemi ===\n");

var r1 = handler.Handle(new PlaceOrderCommand("Ahmet Yılmaz", "ahmet@ornek.com", 1500m));
Console.WriteLine($"✅ Sipariş #{r1.OrderId}: {r1.FinalAmount:C2} (%{r1.DiscountRate * 100:0} indirim)\n");

var r2 = handler.Handle(new PlaceOrderCommand("Ayşe Demir", "ayse@ornek.com", 300m));
Console.WriteLine($"✅ Sipariş #{r2.OrderId}: {r2.FinalAmount:C2}\n");

Console.WriteLine("--- Tüm Siparişler ---");
foreach (var o in handler.GetAll())
    Console.WriteLine($"  #{o.Id} | {o.CustomerName} | {o.FinalAmount:C2}");
