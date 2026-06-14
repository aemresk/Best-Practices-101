// ✅ Async all the way — .Result / .Wait() yerine her yerde await kullan.

Console.WriteLine("=== Sipariş Servisi ===");

var service = new OrderService();

// ✅ await — thread bloke olmaz, deadlock riski yok
var order = await service.GetOrderAsync(1);
Console.WriteLine($"Sipariş: {order}");

// ✅ await — Task tamamlanana kadar thread pool'a geri döner
await service.SaveOrderAsync("Yeni Sipariş");

// ✅ await
var count = await service.GetOrderCountAsync();
Console.WriteLine($"Toplam sipariş: {count}");

class OrderService
{
    public async Task<string> GetOrderAsync(int id)
    {
        await Task.Delay(100);
        return $"Sipariş #{id}";
    }

    public async Task SaveOrderAsync(string order)
    {
        await Task.Delay(100);
        Console.WriteLine($"[DB] Kaydedildi: {order}");
    }

    public async Task<int> GetOrderCountAsync()
    {
        await Task.Delay(50);
        return 42;
    }
}

// ✅ Gerçekten senkron bir entry point gerekiyorsa:
//    Ana metodun imzasını async yapamıyorsan → Task.Run kullan (son çare):
//
//    Task.Run(async () => await DoWorkAsync()).GetAwaiter().GetResult();
//
//    Bu yaklaşım SynchronizationContext'i atlayarak deadlock riskini ortadan kaldırır.
//    Ama yine de tercih edilmemeli — async all the way daha temiz.
