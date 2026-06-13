public interface IProductRepository
{
    // ✅ Domain dilinde isimlendirilmiş metodlar — "aktif ürün" tanımı tek yerde
    Task<List<Product>> GetActiveAsync();
    Task<List<Product>> GetActiveByCategoryAsync(int categoryId);
    Task<List<Product>> GetActiveCheaperThanAsync(decimal maxPrice);
    Task<Product?> GetActiveByIdAsync(int id);
    Task AddAsync(Product product);
    Task DeleteAsync(int id);
}
