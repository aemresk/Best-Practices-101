// ============================================================
// AFTER — Doğru Yaklaşım
// ============================================================

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Products.Any())
    {
        db.Products.AddRange(
            new Product { Name = "Laptop",  Price = 25000, Stock = 10 },
            new Product { Name = "Mouse",   Price = 350,   Stock = 50 },
            new Product { Name = "Klavye",  Price = 450,   Stock = 30 }
        );
        db.SaveChanges();
    }
}

// ✅ Tek SaveChangesAsync — tüm değişiklikler EF'in kendi transaction'ında atomik
// Order, OrderItem, stok düşümü ve Payment hepsi ya commit edilir ya da hiçbiri
app.MapPost("/orders", async (OrderRequest req, AppDbContext db) =>
{
    // Stok kontrolünü önce yap — hiçbir şey commit edilmeden
    var items = new List<(Product Product, int Quantity)>();
    foreach (var item in req.Items)
    {
        var product = await db.Products.FindAsync(item.ProductId);
        if (product is null)
            return Results.BadRequest($"Ürün bulunamadı: {item.ProductId}");
        if (product.Stock < item.Quantity)
            return Results.BadRequest($"Yetersiz stok: {product.Name}");
        items.Add((product, item.Quantity));
    }

    // ✅ Tüm değişiklikleri ChangeTracker'a ekle — henüz DB'ye yazılmadı
    var order = new Order { CustomerName = req.CustomerName, TotalAmount = 0 };
    db.Orders.Add(order);

    decimal total = 0;
    foreach (var (product, qty) in items)
    {
        product.Stock -= qty;
        db.OrderItems.Add(new OrderItem
        {
            Order = order, ProductId = product.Id,
            Quantity = qty, UnitPrice = product.Price
        });
        total += product.Price * qty;
    }

    order.TotalAmount = total;
    db.Payments.Add(new Payment { Order = order, Amount = total, Status = "Pending" });

    // ✅ Tek SaveChangesAsync: EF Core tüm INSERT/UPDATE'leri tek transaction'da çalıştırır
    // Herhangi bir adım başarısız olursa tamamı geri alınır
    await db.SaveChangesAsync();

    return Results.Created($"/orders/{order.Id}", order);
});

// ✅ Explicit transaction: iki ayrı SaveChanges gerektiren nadir durumlar için
// Örnek: Dış servise bildirim sonrası durumu güncellemek
app.MapPost("/orders/{id}/confirm", async (int id, AppDbContext db) =>
{
    var order = await db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
    if (order is null) return Results.NotFound();

    // ✅ BeginTransactionAsync: transaction'ı elle yönet
    await using var tx = await db.Database.BeginTransactionAsync();
    try
    {
        var payment = await db.Payments.FirstOrDefaultAsync(p => p.OrderId == id);
        if (payment is null) return Results.BadRequest("Ödeme kaydı bulunamadı.");

        payment.Status = "Confirmed";
        await db.SaveChangesAsync(); // ← SaveChanges #1, henüz commit yok

        // Burada dış bir servise çağrı yapılabilir (e-posta, bildirim vs.)
        // Başarısız olursa SaveChanges #1 de geri alınır

        await db.SaveChangesAsync(); // ← SaveChanges #2 (örn. audit log)

        await tx.CommitAsync(); // ✅ Her şey başarılıysa commit
        return Results.Ok(order);
    }
    catch
    {
        await tx.RollbackAsync(); // ✅ Herhangi bir hata → tüm değişiklikler geri alınır
        throw;
    }
});

app.MapGet("/orders", async (AppDbContext db) =>
    await db.Orders.AsNoTracking().Include(o => o.Items).ToListAsync());

app.Run();

record OrderRequest(string CustomerName, List<OrderItemRequest> Items);
record OrderItemRequest(int ProductId, int Quantity);
