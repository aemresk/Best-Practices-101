// Somut Excel ihracatçısı — DIP öncesinde doğrudan kullanılıyor
public class ExcelExporter
{
    public string Export(List<SalesData> data)
    {
        // Gerçekte ClosedXML veya EPPlus kullanılır
        var rows = data.Select(d => $"{d.Product}\t{d.Quantity}\t{d.Revenue:C2}\t{d.Date:d}");
        return $"[EXCEL]\n{string.Join("\n", rows)}";
    }
}
