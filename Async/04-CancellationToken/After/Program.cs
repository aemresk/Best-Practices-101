// ✅ CancellationToken — her async metod token alır ve zincir boyunca iletir.

// Senaryo 1: Zaman aşımı — 1.5 saniye sonra otomatik iptal
using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(1.5));

Console.WriteLine("Rapor oluşturuluyor... (1.5s zaman aşımı)");
try
{
    var result = await GenerateReportAsync(timeoutCts.Token);
    Console.WriteLine($"Rapor tamamlandı: {result}");
}
catch (OperationCanceledException)
{
    Console.WriteLine("⏱ Zaman aşımı — rapor iptal edildi, kaynaklar serbest.");
}

// Senaryo 2: Kullanıcı isteği — 800ms sonra manuel iptal
Console.WriteLine("\nKullanıcı isteğiyle iptal senaryosu:");
using var userCts = new CancellationTokenSource();
_ = Task.Run(async () =>
{
    await Task.Delay(800);
    userCts.Cancel(); // kullanıcı "İptal" butonuna bastı
    Console.WriteLine("🛑 Kullanıcı iptal etti.");
});

try
{
    await GenerateReportAsync(userCts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("✅ İstek iptal edildi, gereksiz işlem durduruldu.");
}

// ✅ CancellationToken her metodun son parametresi — zincir boyunca iletilir
async Task<string> GenerateReportAsync(CancellationToken ct)
{
    Console.WriteLine("[1/3] Veriler çekiliyor...");
    await FetchDataAsync(ct);           // ✅ token iletiliyor

    Console.WriteLine("[2/3] Veriler işleniyor...");
    await ProcessDataAsync(ct);         // ✅ token iletiliyor

    Console.WriteLine("[3/3] Rapor yazılıyor...");
    await WriteReportAsync(ct);         // ✅ token iletiliyor

    return "rapor_2024.pdf";
}

async Task FetchDataAsync(CancellationToken ct)
{
    await Task.Delay(2000, ct); // ✅ iptal edilebilir gecikme
}

async Task ProcessDataAsync(CancellationToken ct)
{
    // ✅ Kendi döngüsünde token kontrolü — uzun CPU işlemlerinde elle kontrol
    for (int i = 0; i < 10; i++)
    {
        ct.ThrowIfCancellationRequested(); // ✅ iptal sinyali geldi mi?
        await Task.Delay(150, ct);
    }
}

async Task WriteReportAsync(CancellationToken ct)
{
    await Task.Delay(1000, ct);
}
