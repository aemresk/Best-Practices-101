// ✅ Async Exception Handling — tüm exception'lar görünür ve doğru yakalanır.

Console.WriteLine("=== Senaryo 1: WhenAll — TÜM exception'lar yakalanır ===");
var task = Task.WhenAll(
    FailAsync("Hata A"),
    FailAsync("Hata B"),
    FailAsync("Hata C")
);

try { await task; }
catch
{
    // ✅ task.Exception.InnerExceptions ile TÜMÜ görülür
    foreach (var ex in task.Exception!.InnerExceptions)
        Console.WriteLine($"  ❌ {ex.Message}");
}

Console.WriteLine("\n=== Senaryo 2: Fire-and-forget — güvenli sarmalama ===");
// ✅ Fire-and-forget gerekiyorsa: kendi içinde try/catch ile logla
FireAndForget(DoWorkAsync("güvenli fire-and-forget"));
await Task.Delay(300);

Console.WriteLine("\n=== Senaryo 3: await ile doğru exception tipi ===");
try
{
    // ✅ await kullandığında AggregateException otomatik unwrap edilir
    await Task.Run(async () =>
    {
        await Task.Delay(50).ConfigureAwait(false);
        throw new InvalidOperationException("İç exception");
    });
}
catch (InvalidOperationException ex)
{
    // ✅ Doğrudan asıl exception tipi yakalandı — AggregateException değil
    Console.WriteLine($"✅ Doğru tip yakalandı: {ex.Message}");
}

Console.WriteLine("\n=== Senaryo 4: Kısmi başarı — her Task ayrı kontrol ===");
var tasks = new[]
{
    FailAsync("Servis A"),
    Task.FromResult("Servis B başarılı"),   // başarılı
    FailAsync("Servis C")
};

var results = await Task.WhenAll(tasks.Select(t => t.ContinueWith(r => r)));
foreach (var r in results)
{
    if (r.IsFaulted)
        Console.WriteLine($"  ❌ {r.Exception!.InnerException!.Message}");
    else
        Console.WriteLine($"  ✅ {r.Result}");
}

// ✅ Fire-and-forget yardımcı — exception'ı loglar, sessizce yutmaz
static async void FireAndForget(Task task)
{
    try   { await task; }
    catch (Exception ex) { Console.WriteLine($"[FIRE-AND-FORGET HATA] {ex.Message}"); }
}

async Task<string> FailAsync(string message)
{
    await Task.Delay(50).ConfigureAwait(false);
    throw new InvalidOperationException(message);
}

async Task DoWorkAsync(string label)
{
    await Task.Delay(100).ConfigureAwait(false);
    throw new Exception($"Hata: {label}");
}
