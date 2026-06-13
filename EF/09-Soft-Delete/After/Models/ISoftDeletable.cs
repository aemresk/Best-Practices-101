// ✅ Interface ile soft delete sözleşmesi — tüm silinebilir entity'ler bunu uygular
// AppDbContext bu interface üzerinden generic olarak davranır
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
}
