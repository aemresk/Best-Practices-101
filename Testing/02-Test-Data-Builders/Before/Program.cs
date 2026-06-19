// ❌ Test verisi gürültülü:
//    Testin amacı uzun object initializer içinde kayboluyor.
//    Aynı geçerli değerler her testte tekrar ediliyor.

CanShip_WhenOrderHasAddressAndItems_ShouldReturnTrue();
CanShip_WhenOrderHasNoItems_ShouldReturnFalse();

void CanShip_WhenOrderHasAddressAndItems_ShouldReturnTrue()
{
    var order = new Order
    {
        Id = 101,
        CustomerId = 55,
        CustomerEmail = "ali@ornek.com",
        ShippingAddress = "İstanbul",
        CreatedAt = new DateTime(2026, 1, 10),
        Status = "Pending",
        Items = [new OrderItem("Keyboard", 1, 100m)]
    };

    AssertEqual(true, new ShippingPolicy().CanShip(order), "Adres ve ürün varsa kargolanabilir");
}

void CanShip_WhenOrderHasNoItems_ShouldReturnFalse()
{
    var order = new Order
    {
        Id = 102,
        CustomerId = 55,
        CustomerEmail = "ali@ornek.com",
        ShippingAddress = "İstanbul",
        CreatedAt = new DateTime(2026, 1, 10),
        Status = "Pending",
        Items = []
    };

    AssertEqual(false, new ShippingPolicy().CanShip(order), "Ürün yoksa kargolanamaz");
}

void AssertEqual(bool expected, bool actual, string message)
{
    Console.WriteLine(expected == actual ? $"PASS: {message}" : $"FAIL: {message}");
}

class ShippingPolicy
{
    public bool CanShip(Order order) =>
        order.Status == "Pending" &&
        !string.IsNullOrWhiteSpace(order.ShippingAddress) &&
        order.Items.Count > 0;
}

class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerEmail { get; set; } = "";
    public string ShippingAddress { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "";
    public List<OrderItem> Items { get; set; } = [];
}

record OrderItem(string Name, int Quantity, decimal UnitPrice);
