public class SalesData
{
    public required string Product { get; set; }
    public int Quantity { get; set; }
    public decimal Revenue { get; set; }
    public DateTime Date { get; set; }
}

public record ReportResult(string Format, string Content, int RecordCount);
