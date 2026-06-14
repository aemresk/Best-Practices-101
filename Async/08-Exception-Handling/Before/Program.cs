// ❌ Async Exception Handling Anti-pattern:
//    Task.WhenAll tüm exception'ları flatten eder — sadece ilki görünür.
//    Fire-and-forget Task'lar exception'ları sessizce yutar.
//    AggregateException doğru unwrap edilmeden yakalanıyor.

Console.WriteLine("=== Senaryo 1: WhenAll — sadece ilk exception görünür ===");
try
{
    // ❌ WhenAll birden fazla exception fırlatırsa sadece ilkini görürsün
    await Task.WhenAll(
        FailAsync("Hata A"),
        FailAsync("Hata B"),
        FailAsync("Hata C")
    );
}
catch (Exception ex)
{
    // ❌ Sadece "Hata A" görünür — B ve C sessizce kaybolur
    Console.WriteLine($"Yakalanan: {ex.Message}");
}

Console.WriteLine("\n=== Senaryo 2: Fire-and-forget — exception tamamen kaybolur ===");
// ❌ Task döndürülüyor ama await edilmiyor — exception yutulur
_ = DoWorkAsync("sessiz hata");
await Task.Delay(300); // exception'ın gelmesini bekle
Console.WriteLine("Exception sessizce kayboldu.");

Console.WriteLine("\n=== Senaryo 3: async lambda'da yanlış exception tipi ===");
try
{
    // ❌ async metodun fırlattığı exception AggregateException olarak wrap edilebilir
    var task = Task.Run(async () =>
    {
        await Task.Delay(50);
        throw new InvalidOperationException("İç exception");
    });
    task.Wait(); // ❌ AggregateException fırlar, InnerException'ı kaçırırsın
}
catch (AggregateException ae)
{
    // ❌ AggregateException yerine doğrudan InnerException yakalanmalıydı
    Console.WriteLine($"AggregateException: {ae.InnerException?.Message}");
}

async Task FailAsync(string message)
{
    await Task.Delay(50);
    throw new InvalidOperationException(message);
}

async Task DoWorkAsync(string label)
{
    await Task.Delay(100);
    throw new Exception($"Fire-and-forget hatası: {label}");
}
