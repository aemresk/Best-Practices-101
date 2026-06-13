// ✅ DIP: İhracat soyutlaması — format değiştirmek için ReportService açılmaz
public interface IReportExporter
{
    string Format { get; }
    string Export(List<SalesData> data);
}

public class ExcelExporter : IReportExporter
{
    public string Format => "Excel";
    public string Export(List<SalesData> data)
    {
        var rows = data.Select(d => $"{d.Product}\t{d.Quantity}\t{d.Revenue:C2}\t{d.Date:d}");
        return $"[EXCEL]\n{string.Join("\n", rows)}";
    }
}

// ✅ PDF ihracatçısı eklendi — ReportService değişmedi
public class PdfExporter : IReportExporter
{
    public string Format => "PDF";
    public string Export(List<SalesData> data)
    {
        var rows = data.Select(d => $"| {d.Product,-12} | {d.Quantity,5} | {d.Revenue,12:C2} |");
        return $"[PDF REPORT]\n{string.Join("\n", rows)}";
    }
}
