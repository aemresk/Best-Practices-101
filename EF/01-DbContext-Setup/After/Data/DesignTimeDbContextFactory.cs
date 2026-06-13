using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

// ✅ dotnet ef migrations add / update komutları design-time'da bu factory'yi kullanır
// ✅ DI container çalışmadığında (CLI ortamı) DbContext üretebilmek için gerekli
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite("Data Source=products_after.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}
