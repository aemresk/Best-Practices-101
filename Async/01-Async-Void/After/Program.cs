// ✅ async Task kullan — exception'lar yakalanabilir, await edilebilir.

Console.WriteLine("İşlem başlıyor...");

// ✅ async Task: await edilebilir, exception'ı çağırana taşır
try
{
    await ProcessOrderAsync(orderId: 42);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"❌ Sipariş hatası yakalandı: {ex.Message}");
}

Console.WriteLine("Program düzgün sonlandı.");

// ✅ async Task — void yerine Task döner
async Task ProcessOrderAsync(int orderId)
{
    await Task.Delay(100);
    throw new InvalidOperationException($"Sipariş {orderId} işlenemedi!");
}

// ✅ İSTİSNA: UI event handler'ları (Button.Click vb.) — sadece burada async void kabul edilebilir.
//    Çünkü event delegate imzası void döndürür, değiştiremeyiz.
//    Ama içinde try/catch ile sarılmalı:
//
// async void OnButtonClick(object sender, EventArgs e)  // ← zorunlu void
// {
//     try   { await DoWorkAsync(); }
//     catch { /* mutlaka yakala */ }
// }
