// Somut veri yükleyici — DIP öncesinde doğrudan yaratılıyor
public class DatabaseDataLoader
{
    public List<SalesData> Load()
    {
        // Gerçekte DB sorgusu yapılır; burada sabit veri
        return new()
        {
            new SalesData { Product = "Laptop",  Quantity = 10, Revenue = 150000, Date = DateTime.Today.AddDays(-1) },
            new SalesData { Product = "Mouse",   Quantity = 50, Revenue = 12500,  Date = DateTime.Today.AddDays(-2) },
            new SalesData { Product = "Monitor", Quantity = 8,  Revenue = 48000,  Date = DateTime.Today }
        };
    }
}
