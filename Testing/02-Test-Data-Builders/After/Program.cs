// ✅ Test Data Builder:
//    Geçerli varsayılanlar tek yerde.
//    Testte sadece davranış için önemli farklar görünüyor.

CanShip_WhenOrderHasAddressAndItems_ShouldReturnTrue();
CanShip_WhenOrderHasNoItems_ShouldReturnFalse();
CanShip_WhenOrderHasNoAddress_ShouldReturnFalse();

void CanShip_WhenOrderHasAddressAndItems_ShouldReturnTrue()
{
    var order = OrderBuilder.ValidOrder().Build();

    AssertEqual(true, new ShippingPolicy().CanShip(order), "Geçerli sipariş kargolanabilir");
}

void CanShip_WhenOrderHasNoItems_ShouldReturnFalse()
{
    var order = OrderBuilder.ValidOrder()
        .WithItems([])
        .Build();

    AssertEqual(false, new ShippingPolicy().CanShip(order), "Ürün yoksa kargolanamaz");
}

void CanShip_WhenOrderHasNoAddress_ShouldReturnFalse()
{
    var order = OrderBuilder.ValidOrder()
        .WithShippingAddress("")
        .Build();

    AssertEqual(false, new ShippingPolicy().CanShip(order), "Adres yoksa kargolanamaz");
}

void AssertEqual(bool expected, bool actual, string message)
{
    Console.WriteLine(expected == actual
        ? $"PASS: {message}"
        : $"FAIL: {message} | expected: {expected}, actual: {actual}");
}

class OrderBuilder
{
    private readonly Order _order = new()
    {
        Id = 101,
        CustomerId = 55,
        CustomerEmail = "ali@ornek.com",
        ShippingAddress = "İstanbul",
        CreatedAt = new DateTime(2026, 1, 10),
        Status = "Pending",
        Items = [new OrderItem("Keyboard", 1, 100m)]
    };

    public static OrderBuilder ValidOrder() => new();

    public OrderBuilder WithItems(List<OrderItem> items)
    {
        _order.Items = items;
        return this;
    }

    public OrderBuilder WithShippingAddress(string address)
    {
        _order.ShippingAddress = address;
        return this;
    }

    public Order Build() => _order;
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
