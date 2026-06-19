// ❌ Kötü test yapısı:
//    Test isimleri davranışı anlatmıyor.
//    Arrange, Act ve Assert birbirine karışıyor.
//    Assertion beklenen sonucu açık söylemiyor.

Test1();
ShouldWork();

void Test1()
{
    var service = new OrderService();
    var total = service.CalculateTotal(new Order("Ali", true, [new("Keyboard", 100m)]));

    AssertTrue(total < 100m, "Premium müşteriye indirim uygulanmalı");
}

void ShouldWork()
{
    var service = new OrderService();
    var order = new Order("Ayşe", false, [new("Mouse", 40m), new("Pad", 10m)]);

    AssertTrue(service.CalculateTotal(order) == 50m, "Normal müşteri indirimsiz toplam ödemeli");
}

void AssertTrue(bool condition, string message)
{
    Console.WriteLine(condition ? $"PASS: {message}" : $"FAIL: {message}");
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
