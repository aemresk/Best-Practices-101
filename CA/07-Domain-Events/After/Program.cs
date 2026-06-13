// ✅ Composition Root: event'ler ve handler'lar burada bağlanır.
//    Domain sınıfı (Order) hiçbir handler'ı tanımıyor.
using Application;
using Domain;

var stock = new Dictionary<string, int> { ["Laptop"] = 10 };

// Event dispatcher kurulumu
var dispatcher = new EventDispatcher();
dispatcher.Register<OrderPlacedEvent>(new SendOrderConfirmationHandler().Handle);
dispatcher.Register<OrderPlacedEvent>(new UpdateInventoryHandler(stock).Handle);
dispatcher.Register<OrderPlacedEvent>(new AuditLogHandler().Handle);

// Domain kullanımı
var order = new Order();
order.Place("Ali Veli", "Laptop", quantity: 2);

// ✅ Dispatch: domain event'leri handler'lara iletiliyor
dispatcher.Dispatch(order.DomainEvents);
order.ClearEvents();

Console.WriteLine($"\nKalan Laptop stoğu: {stock["Laptop"]}");
