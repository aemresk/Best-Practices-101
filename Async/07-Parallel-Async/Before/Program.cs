// ❌ Sıralı await Anti-pattern:
//    Birbirinden bağımsız async işlemler sırayla bekleniyor.
//    Her işlem bir önceki bitmeden başlamıyor — toplam süre: A + B + C
//    Oysa hepsi aynı anda başlayabilir — toplam süre: max(A, B, C)

var sw = System.Diagnostics.Stopwatch.StartNew();

Console.WriteLine("Dashboard verileri yükleniyor...");

// ❌ Sıralı — 300 + 500 + 200 = 1000ms
var user     = await GetUserAsync();        // 300ms bekle
var orders   = await GetOrdersAsync();      // sonra 500ms bekle
var stats    = await GetStatsAsync();       // sonra 200ms bekle

sw.Stop();
Console.WriteLine($"Kullanıcı : {user}");
Console.WriteLine($"Siparişler: {orders}");
Console.WriteLine($"İstatistik: {stats}");
Console.WriteLine($"\nToplam süre: {sw.ElapsedMilliseconds}ms (beklenen ~1000ms)");

async Task<string> GetUserAsync()
{
    await Task.Delay(300);
    return "Ahmet Yılmaz";
}

async Task<string> GetOrdersAsync()
{
    await Task.Delay(500);
    return "5 sipariş";
}

async Task<string> GetStatsAsync()
{
    await Task.Delay(200);
    return "128 ziyaret";
}
