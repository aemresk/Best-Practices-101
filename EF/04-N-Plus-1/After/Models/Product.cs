public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public List<Review> Reviews { get; set; } = new();
    public List<Image> Images { get; set; } = new();
}
