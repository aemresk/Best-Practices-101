// ✅ CQRS: Command (yazma) ve Query (okuma) tamamen ayrı.
//    Her biri bağımsız optimize edilebilir, ölçeklendirilebilir.
using Commands;
using Domain;
using Queries;

// Paylaşılan in-memory store (gerçek sistemde: write DB + read replica)
var store = new List<Product>();

// Command handlers
var createHandler      = new CreateProductHandler(store);
var updatePriceHandler = new UpdatePriceHandler(store);

// Query handlers
var getAllHandler     = new GetAllProductsHandler(store);
var summaryHandler   = new GetProductSummaryHandler(store);

// Yazma operasyonları — Command'lar
createHandler.Handle(new CreateProductCommand("Laptop", 15000m, 10));
createHandler.Handle(new CreateProductCommand("Mouse",    250m, 50));
updatePriceHandler.Handle(new UpdatePriceCommand("Laptop", 14500m));

// Okuma operasyonları — Query'ler
var products = getAllHandler.Handle(new GetAllProductsQuery());
Console.WriteLine("\nTüm ürünler:");
foreach (var p in products)
    Console.WriteLine($"  {p.Name}: {p.Price:C2} (stok: {p.Stock})");

var summary = summaryHandler.Handle(new GetProductSummaryQuery());
Console.WriteLine($"\nÖzet: {summary.TotalProducts} ürün, ort. fiyat {summary.AveragePrice:C2}, toplam stok {summary.TotalStock}");
