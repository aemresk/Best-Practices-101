// ✅ DIP: Yüksek seviyeli modül soyutlamalara bağlı
// Excel → PDF: Program.cs'de IReportExporter kaydı değişir, bu sınıf açılmaz
// DB → API:   Program.cs'de IDataLoader kaydı değişir, bu sınıf açılmaz
// Unit test:  FakeDataLoader + FakeExporter inject edilir, gerçek bağlantı gerekmez
public class ReportService
{
    private readonly IReportExporter _exporter;
    private readonly IDataLoader _dataLoader;

    // ✅ Bağımlılıklar dışarıdan enjekte ediliyor
    public ReportService(IReportExporter exporter, IDataLoader dataLoader)
    {
        _exporter   = exporter;
        _dataLoader = dataLoader;
    }

    public ReportResult GenerateReport()
    {
        var data    = _dataLoader.Load();
        var content = _exporter.Export(data);
        return new ReportResult(_exporter.Format, content, data.Count);
    }
}
