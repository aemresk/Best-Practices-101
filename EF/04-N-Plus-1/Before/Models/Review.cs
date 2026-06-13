public class Review
{
    public int Id { get; set; }
    public required string Comment { get; set; }
    public int Rating { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
