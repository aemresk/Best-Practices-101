using Microsoft.EntityFrameworkCore;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db) => _db = db;

    // ✅ "Aktif ürün" filtresi tek yerde tanımlı — tüm metodlar buraya delegate eder
    private IQueryable<Product> ActiveProducts =>
        _db.Products.Where(p => p.IsActive);

    public Task<List<Product>> GetActiveAsync() =>
        ActiveProducts
            .AsNoTracking()
            .Include(p => p.Category)
            .ToListAsync();

    public Task<List<Product>> GetActiveByCategoryAsync(int categoryId) =>
        ActiveProducts
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .ToListAsync();

    public Task<List<Product>> GetActiveCheaperThanAsync(decimal maxPrice) =>
        ActiveProducts
            .AsNoTracking()
            .Where(p => p.Price < maxPrice)
            .ToListAsync();

    public Task<Product?> GetActiveByIdAsync(int id) =>
        ActiveProducts
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return;
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
    }
}
