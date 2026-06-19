// ✅ İyi test yapısı:
//    Test adı davranışı anlatır.
//    Arrange, Act ve Assert net ayrılır.
//    Beklenen değer açıkça görünür.

CalculateTotal_WhenCustomerIsPremium_ShouldApplyDiscount();
CalculateTotal_WhenCustomerIsNotPremium_ShouldReturnFullTotal();

void CalculateTotal_WhenCustomerIsPremium_ShouldApplyDiscount()
{
    // Arrange
    var service = new OrderService();
    var order = new Order("Ali", true, [new("Keyboard", 100m)]);

    // Act
    var total = service.CalculateTotal(order);

    // Assert
    AssertEqual(90m, total, "Premium müşteri yüzde 10 indirim almalı");
}

void CalculateTotal_WhenCustomerIsNotPremium_ShouldReturnFullTotal()
{
    // Arrange
    var service = new OrderService();
    var order = new Order("Ayşe", false, [new("Mouse", 40m), new("Pad", 10m)]);

    // Act
    var total = service.CalculateTotal(order);

    // Assert
    AssertEqual(50m, total, "Normal müşteri indirimsiz toplam ödemeli");
}

void AssertEqual(decimal expected, decimal actual, string message)
{
    Console.WriteLine(expected == actual
        ? $"PASS: {message}"
        : $"FAIL: {message} | expected: {expected}, actual: {actual}");
}

record Order(string CustomerName, bool IsPremiumCustomer, List<OrderItem> Items);
record OrderItem(string Name, decimal UnitPrice);

class OrderService
{
    public decimal CalculateTotal(Order order)
    {
        var total = order.Items.Sum(i => i.UnitPrice);
        return order.IsPremiumCustomer ? total * 0.90m : total;
    }
}
