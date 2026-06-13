using Application;
using Infrastructure;

// ✅ Composition Root: hangi repository kullanılacağına burada karar verilir
var service = new ProductService(new InMemoryProductRepository());

service.Add("Laptop", 15000m, 10);
service.Add("Mouse",    250m, 50);

var expensive = service.GetExpensive(minPrice: 1000m);
Console.WriteLine($"\nPahalı ürünler ({expensive.Count} adet):");
foreach (var p in expensive)
    Console.WriteLine($"  {p.Name}: {p.Price:C2}");

service.Sell("Laptop", quantity: 3);
Console.WriteLine($"\nLaptop stok: {service.GetByName("Laptop")?.Stock}");
