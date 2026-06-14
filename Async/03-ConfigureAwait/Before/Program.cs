// ❌ ConfigureAwait eksikliği — Library kodu:
//    Library/infrastructure kodu await sonrasında SynchronizationContext'e geri döner.
//    Bu gereksiz context switch performansı düşürür.
//    Daha kötüsü: .Result ile çağıran biri varsa deadlock'a kapı açar.
//    Library kodu UI veya ASP.NET context'ine bağımlı olmamalı.

var repo = new ProductRepository();

Console.WriteLine($"Thread: {Environment.CurrentManagedThreadId}");

var products = await repo.GetAllAsync();
Console.WriteLine($"Thread: {Environment.CurrentManagedThreadId} (context'e döndü — gereksiz)");
Console.WriteLine($"{products.Count} ürün getirildi.");

await repo.SaveAsync("Laptop");
Console.WriteLine("Kaydedildi.");

// ❌ Library içindeki her await, SynchronizationContext'i yakalayıp geri döner.
//    UI uygulamalarında bu UI thread'ini gereksiz işlerle meşgul eder.
//    ASP.NET Classic'te blocking çağrıyla birleşince deadlock oluşur.

class ProductRepository
{
    private readonly List<string> _store = new();

    public async Task<List<string>> GetAllAsync()
    {
        await Task.Delay(50); // ❌ ConfigureAwait(false) yok — context yakalandı
        return _store.ToList();
    }

    public async Task SaveAsync(string product)
    {
        await Task.Delay(30); // ❌ ConfigureAwait(false) yok
        _store.Add(product);
        Console.WriteLine($"[DB] {product} eklendi");

        await Task.Delay(20); // ❌ Her await context'e geri döner
        Console.WriteLine("[DB] İndeks güncellendi");
    }
}
