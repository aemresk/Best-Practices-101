// ❌ lock ile async — deadlock tuzağı:
//    `lock` bloğunun içinde `await` kullanılamaz (derleme hatası).
//    Çözüm olarak ya async kaldırılıyor (blocking) ya da lock çıkarılıyor (race condition).

Console.WriteLine("10 eş zamanlı istek simüle ediliyor...\n");

var counter = new RequestCounter();
var tasks   = Enumerable.Range(1, 10)
    .Select(i => counter.IncrementAsync(i))
    .ToArray();

await Task.WhenAll(tasks);
Console.WriteLine($"\nSon sayaç değeri: {counter.Value} (beklenen: 10)");

class RequestCounter
{
    private readonly object _lock = new();
    private int _value;

    public int Value => _value;  

    public async Task IncrementAsync(int requestId)
    {
        // ❌ Seçenek 1: lock içinde await — DERLEME HATASI
        // lock (_lock)
        // {
        //     await Task.Delay(10); // CS1996: Cannot await in the body of a lock statement
        //     _value++;
        // }

        // ❌ Seçenek 2: lock'suz async — race condition, sayaç yanlış
        await Task.Delay(10); // simüle async iş
        _value++;             // ❌ eş zamanlı erişim — race condition!
        Console.WriteLine($"  İstek #{requestId} tamamlandı → sayaç: {_value}");

        // ❌ Seçenek 3: lock ile blocking (async yok) — thread pool tükenir
        // lock (_lock)
        // {
        //     Thread.Sleep(10); // ❌ async faydası yok, thread bloke
        //     _value++;
        // }
    }
}
