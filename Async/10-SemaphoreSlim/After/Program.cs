// ✅ SemaphoreSlim — async-safe mutual exclusion ve eş zamanlılık sınırlama.
//    await ile birlikte çalışır, thread bloke etmez.

Console.WriteLine("=== Senaryo 1: Mutex (1 eş zamanlı erişim) ===");
var counter = new RequestCounter();
var tasks   = Enumerable.Range(1, 10).Select(i => counter.IncrementAsync(i)).ToArray();
await Task.WhenAll(tasks);
Console.WriteLine($"Son sayaç: {counter.Value} (beklenen: 10)\n");

Console.WriteLine("=== Senaryo 2: Rate limiting (3 eş zamanlı istek) ===");
var rateLimiter = new ApiClient(maxConcurrent: 3);
var apiTasks    = Enumerable.Range(1, 8).Select(i => rateLimiter.CallApiAsync(i)).ToArray();
await Task.WhenAll(apiTasks);
Console.WriteLine("Tüm API çağrıları tamamlandı.");

// ✅ SemaphoreSlim(1,1) — `lock` yerine: await ile async-safe mutex
class RequestCounter
{
    private readonly SemaphoreSlim _semaphore = new(1, 1); // max 1 eş zamanlı
    private int _value;

    public int Value => _value;

    public async Task IncrementAsync(int requestId)
    {
        await _semaphore.WaitAsync().ConfigureAwait(false); // ✅ async bekleme — thread bloke yok
        try
        {
            await Task.Delay(10).ConfigureAwait(false); // kritik bölgede async iş
            _value++;
            Console.WriteLine($"  İstek #{requestId:D2} → sayaç: {_value}");
        }
        finally
        {
            _semaphore.Release(); // ✅ finally bloğu — exception'da da serbest bırakılır
        }
    }
}

// ✅ SemaphoreSlim(N,N) — aynı anda en fazla N istek izni
class ApiClient(int maxConcurrent)
{
    private readonly SemaphoreSlim _throttle = new(maxConcurrent, maxConcurrent);

    public async Task CallApiAsync(int requestId)
    {
        await _throttle.WaitAsync().ConfigureAwait(false);
        try
        {
            Console.WriteLine($"  [API] İstek #{requestId} başladı (eş zamanlı: {maxConcurrent - _throttle.CurrentCount + 1})");
            await Task.Delay(200).ConfigureAwait(false); // simüle HTTP çağrısı
            Console.WriteLine($"  [API] İstek #{requestId} tamamlandı");
        }
        finally
        {
            _throttle.Release();
        }
    }
}
