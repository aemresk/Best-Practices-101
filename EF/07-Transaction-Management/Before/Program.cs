// ============================================================
// BEFORE — Yaygın Hatalar
// Bu dosyada kasıtlı olarak yanlış pratikler gösterilmektedir.
// ============================================================

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite("Data Source=before.db"));
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

// DTO
app.MapPost("/orders", async (OrderRequest req, AppDbContext db) =>
{
    // ❌ 1. SaveChanges: Sipariş oluşturuldu ve commit edildi
    var order = new Order { CustomerName = req.CustomerName, TotalAmount = 0 };
    db.Orders.Add(order);
    await db.SaveChangesAsync(); // ← COMMIT #1: Order veritabanında

    decimal total = 0;
    foreach (var item in req.Items)
    {
        var product = await db.Products.FindAsync(item.ProductId);
        if (product is null) return Results.BadRequest($"Ürün bulunamadı: {item.ProductId}");
        if (product.Stock < item.Quantity)
        {
            // ❌ Hata durumunda: Order zaten commit edildi (COMMIT #1 geri alınamaz)
            // Veritabanında siparişsiz bir Order kaydı kalır
            return Results.BadRequest($"Yetersiz stok: {product.Name}");
        }

        product.Stock -= item.Quantity;
        db.OrderItems.Add(new OrderItem
        {
            OrderId = order.Id, ProductId = product.Id,
            Quantity = item.Quantity, UnitPrice = product.Price
        });
        total += product.Price * item.Quantity;

        // ❌ 2. SaveChanges: Her ürün için ayrı commit — kısmi stok düşümü mümkün
        await db.SaveChangesAsync(); // ← COMMIT #2, #3, ...
    }

    order.TotalAmount = total;

    // ❌ 3. SaveChanges: Payment ayrı commit'te — Payment başarısız olursa
    //    Order + OrderItems veritabanında, Payment yok → tutarsız durum
    db.Payments.Add(new Payment { OrderId = order.Id, Amount = total, Status = "Pending" });
    await db.SaveChangesAsync(); // ← COMMIT #3 (veya N+2)

    return Results.Created($"/orders/{order.Id}", order);
});

app.MapGet("/orders", async (AppDbContext db) =>
    await db.Orders.AsNoTracking().Include(o => o.Items).ToListAsync());

app.Run();

record OrderRequest(string CustomerName, List<OrderItemRequest> Items);
record OrderItemRequest(int ProductId, int Quantity);
