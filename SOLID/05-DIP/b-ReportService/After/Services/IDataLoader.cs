// ✅ DIP: Veri kaynağı soyutlaması — DB → API geçişi ReportService'i etkilemez
public interface IDataLoader
{
    List<SalesData> Load();
}

public class DatabaseDataLoader : IDataLoader
{
    public List<SalesData> Load() => new()
    {
        new SalesData { Product = "Laptop",  Quantity = 10, Revenue = 150000, Date = DateTime.Today.AddDays(-1) },
        new SalesData { Product = "Mouse",   Quantity = 50, Revenue = 12500,  Date = DateTime.Today.AddDays(-2) },
        new SalesData { Product = "Monitor", Quantity = 8,  Revenue = 48000,  Date = DateTime.Today }
    };
}

// ✅ API'den çekme — ReportService değişmez
public class ApiDataLoader : IDataLoader
{
    public List<SalesData> Load()
    {
        // Gerçekte HttpClient ile harici API çağrısı yapılır
        Console.WriteLine("[API-LOADER] Satış verisi harici API'den çekiliyor...");
        return new()
        {
            new SalesData { Product = "Tablet", Quantity = 20, Revenue = 60000, Date = DateTime.Today }
        };
    }
}
