// ✅ Task.WhenAll — bağımsız işlemler aynı anda başlatılır.
//    Toplam süre: max(A, B, C) = ~500ms (en uzun olanı kadar)

var sw = System.Diagnostics.Stopwatch.StartNew();

Console.WriteLine("Dashboard verileri yükleniyor...");

// ✅ Hepsi aynı anda başlatılıyor — await etmeden Task'ları al
var userTask   = GetUserAsync();
var ordersTask = GetOrdersAsync();
var statsTask  = GetStatsAsync();

// ✅ Hepsi tamamlanana kadar bekle — süre: max(300, 500, 200) = ~500ms
var (user, orders, stats) = await (userTask, ordersTask, statsTask).WhenAll();

sw.Stop();
Console.WriteLine($"Kullanıcı : {user}");
Console.WriteLine($"Siparişler: {orders}");
Console.WriteLine($"İstatistik: {stats}");
Console.WriteLine($"\nToplam süre: {sw.ElapsedMilliseconds}ms (beklenen ~500ms)");

// ✅ Alternatif: Task.WhenAll ile dizi döndürme
Console.WriteLine("\n--- WhenAll ile dizi ---");
var sw2 = System.Diagnostics.Stopwatch.StartNew();
var results = await Task.WhenAll(GetUserAsync(), GetOrdersAsync(), GetStatsAsync());
sw2.Stop();
Console.WriteLine($"Sonuçlar: {string.Join(", ", results)}");
Console.WriteLine($"Süre: {sw2.ElapsedMilliseconds}ms");

// ✅ Task.WhenAny — ilk tamamlanan yeterli (race, timeout senaryosu)
Console.WriteLine("\n--- WhenAny: ilk gelen kazanır ---");
var sw3 = System.Diagnostics.Stopwatch.StartNew();
var fastest = await Task.WhenAny(GetUserAsync(), GetOrdersAsync(), GetStatsAsync());
sw3.Stop();
Console.WriteLine($"İlk gelen: {await fastest} ({sw3.ElapsedMilliseconds}ms)");

async Task<string> GetUserAsync()
{
    await Task.Delay(300).ConfigureAwait(false);
    return "Ahmet Yılmaz";
}

async Task<string> GetOrdersAsync()
{
    await Task.Delay(500).ConfigureAwait(false);
    return "5 sipariş";
}

async Task<string> GetStatsAsync()
{
    await Task.Delay(200).ConfigureAwait(false);
    return "128 ziyaret";
}

// WhenAll için tuple extension
static class TaskExtensions
{
    public static async Task<(T1, T2, T3)> WhenAll<T1, T2, T3>(
        this (Task<T1> t1, Task<T2> t2, Task<T3> t3) tasks)
    {
        await Task.WhenAll(tasks.t1, tasks.t2, tasks.t3).ConfigureAwait(false);
        return (tasks.t1.Result, tasks.t2.Result, tasks.t3.Result);
    }
}
