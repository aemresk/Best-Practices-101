// ❌ async void Anti-pattern:
//    async void metodların fırlattığı exception'lar yakalanamaz.
//    Çağıran, metodun tamamlanmasını await edemez.
//    Uygulama sessizce çöker veya exception kaybolur.

Console.WriteLine("İşlem başlıyor...");

// ❌ async void — exception'ı yakalamak imkânsız
ProcessOrderAsync(orderId: 42);

Console.WriteLine("Bu satır hemen çalışır — işlem bitmeden.");
await Task.Delay(500); // exceptio n'ın gelmesini bekleyelim

async void ProcessOrderAsync(int orderId)
{
    await Task.Delay(100); // simüle asenkron iş
    throw new InvalidOperationException($"Sipariş {orderId} işlenemedi!");
    // ❌ Bu exception yukarıdaki try/catch'e ulaşmaz
    //    UnobservedTaskException veya process crash olur
}

// ❌ try/catch ile sarmak da işe yaramaz
try
{
    ProcessOrderAsync(orderId: 99); // ❌ Task dönmüyor, await edilemiyor
}
catch (Exception ex)
{
    // Bu blok hiçbir zaman çalışmaz — exception kaybolur
    Console.WriteLine($"❌ Exception yakalandı: {ex.Message}");
}

Console.WriteLine("Program sona erdi. Exception sessizce kayboldu.");
