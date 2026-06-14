// ❌ CancellationToken eksikliği:
//    Uzun süren işlemler iptal edilemiyor.
//    Kullanıcı isteği iptal etse de (tarayıcıyı kapasa da) işlem sonuna kadar çalışmaya devam eder.
//    Kaynak israfı: gereksiz DB sorguları, HTTP çağrıları, CPU işlemleri sürer.

Console.WriteLine("Rapor oluşturuluyor... (iptal edilemez)");

// ❌ İptal desteği yok — başlatıldıktan sonra durdurulamaz
var result = await GenerateReportAsync();
Console.WriteLine($"Rapor tamamlandı: {result}");

// ❌ CancellationToken almıyor — zincirin her halkası aynı sorunlu
async Task<string> GenerateReportAsync()
{
    Console.WriteLine("[1/3] Veriler çekiliyor...");
    await FetchDataAsync();          // ❌ iptal edilemez

    Console.WriteLine("[2/3] Veriler işleniyor...");
    await ProcessDataAsync();        // ❌ iptal edilemez

    Console.WriteLine("[3/3] Rapor yazılıyor...");
    await WriteReportAsync();        // ❌ iptal edilemez

    return "rapor_2024.pdf";
}

async Task FetchDataAsync()
{
    await Task.Delay(2000); // simüle uzun DB sorgusu
}

async Task ProcessDataAsync()
{
    await Task.Delay(1500); // simüle CPU yoğun işlem
}

async Task WriteReportAsync()
{
    await Task.Delay(1000); // simüle dosya yazma
}
