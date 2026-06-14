// ❌ Blocking Calls Anti-pattern:
//    .Result ve .Wait() thread'i bloke eder.
//    SynchronizationContext olan ortamlarda (ASP.NET Classic, WPF, WinForms) deadlock'a yol açar.
//    Thread pool thread'lerini tüketir — yük altında sunucu yanıt veremez hale gelir.

Console.WriteLine("=== Sipariş Servisi ===");

var service = new OrderService();

// ❌ .Result — async metodu senkron çağırıyor, thread bloke
var order = service.GetOrderAsync(1).Result;
Console.WriteLine($"Sipariş: {order}");

// ❌ .Wait() — aynı sorun, dönüş değeri yok
service.SaveOrderAsync("Yeni Sipariş").Wait();

// ❌ GetAwaiter().GetResult() — .Result ile özdeş, deadlock riski aynı
var count = service.GetOrderCountAsync().GetAwaiter().GetResult();
Console.WriteLine($"Toplam sipariş: {count}");

class OrderService
{
    public async Task<string> GetOrderAsync(int id)
    {
        await Task.Delay(100); // simüle DB sorgusu
        return $"Sipariş #{id}";
    }

    public async Task SaveOrderAsync(string order)
    {
        await Task.Delay(100); // simüle DB yazma
        Console.WriteLine($"[DB] Kaydedildi: {order}");
    }

    public async Task<int> GetOrderCountAsync()
    {
        await Task.Delay(50);
        return 42;
    }
}

// ❌ Deadlock senaryosu (ASP.NET Classic / SynchronizationContext olan ortam):
//
//   UI Thread → service.GetOrderAsync(1).Result  ← thread bloke, context tutuluyor
//   GetOrderAsync tamamlandığında continuation için aynı context'e ihtiyaç duyar
//   Ama context UI thread tarafından bloke edilmiş → DEADLOCK
