// ❌ Anemic model: validation yok, nullable referanslar ignore edilmiş
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!; // ❌ null! ile uyarı bastırılmış
    public decimal Price { get; set; }
}
