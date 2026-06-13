namespace Domain;

// ✅ Repository arayüzü Domain katmanında — implementasyon bilgisi yok
public interface IProductRepository
{
    void     Add(Product product);
    void     Update(Product product);
    Product? GetByName(string name);
    IReadOnlyList<Product> GetByMinPrice(decimal minPrice);
    int      NextId();
}
