// ✅ ConfigureAwait(false) — Library/infrastructure kodu:
//    Context'e geri dönmeye gerek yok → gereksiz switch ortadan kalkar.
//    Deadlock kapısı kapanır.
//    Kural: UI/uygulama katmanında ConfigureAwait(false) YOK,
//           library/infrastructure katmanında her await'te ConfigureAwait(false) VAR.

var repo = new ProductRepository();

// ✅ Uygulama katmanı: ConfigureAwait(false) KULLANMAZ
//    Çünkü burada UI context'ine (örn. WPF) geri dönmek gerekebilir
var products = await repo.GetAllAsync();
Console.WriteLine($"{products.Count} ürün getirildi.");

await repo.SaveAsync("Laptop");
Console.WriteLine("Kaydedildi.");

// ✅ Library/Repository sınıfı: her await'te ConfigureAwait(false)
class ProductRepository
{
    private readonly List<string> _store = new();

    public async Task<List<string>> GetAllAsync()
    {
        await Task.Delay(50).ConfigureAwait(false); // ✅ context'e geri dönme
        return _store.ToList();
    }

    public async Task SaveAsync(string product)
    {
        await Task.Delay(30).ConfigureAwait(false); // ✅
        _store.Add(product);
        Console.WriteLine($"[DB] {product} eklendi");

        await Task.Delay(20).ConfigureAwait(false); // ✅ her await'te tekrar
        Console.WriteLine("[DB] İndeks güncellendi");
    }
}

// ✅ ASP.NET Core'da SynchronizationContext olmadığından ConfigureAwait(false)
//    deadlock için zorunlu değil, ama yine de performans için önerilir.
//
// ✅ Modern yaklaşım: .NET library projelerinde
//    <Nullable>enable</Nullable> ile birlikte
//    global using ile tüm dosyalara ConfigureAwait analyzer eklenebilir:
//    dotnet add package ConfigureAwaitChecker.Analyzer
