public class Product : ISoftDeletable
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }

    // ✅ Soft delete alanları — migration ile eklenir, uygulamaya şeffaf
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
