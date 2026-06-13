// ❌ DIP İHLALİ: Yüksek seviyeli ReportService somut sınıflara bağımlı
// Excel → PDF'e geçmek için bu sınıfı açmak gerekir
// DB → API'ye geçmek için bu sınıfı açmak gerekir
// Unit test: gerçek DB bağlantısı olmadan test etmek imkânsız
public class ReportService
{
    // ❌ Somut sınıflar doğrudan field olarak yaratılmış
    private readonly ExcelExporter _exporter       = new();
    private readonly DatabaseDataLoader _dataLoader = new();

    public ReportResult GenerateReport()
    {
        // ❌ Somut DatabaseDataLoader — kaynak değiştirilemez
        var data = _dataLoader.Load();

        // ❌ Somut ExcelExporter — format değiştirilemez
        var content = _exporter.Export(data);

        return new ReportResult("Excel", content, data.Count);
    }
}
