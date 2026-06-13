// ✅ ISP: Sorumluluk başına ayrı interface

// Okuma sözleşmesi
public interface IOrderReader
{
    Order? GetById(int id);
    List<Order> GetAll();
    List<Order> GetByStatus(string status);
}

// Yazma sözleşmesi
public interface IOrderWriter
{
    void Add(Order order);
    void Update(Order order);
    void Delete(int id);
}

// Raporlama sözleşmesi
public interface IOrderReporter
{
    OrderSummary GetSummary();
    List<Order> GetTopOrders(int count);
}

// ✅ Tam CRUD + rapor — hepsini implement ediyor, hiçbir şeyi fırlatmıyor
public class OrderRepository : IOrderReader, IOrderWriter, IOrderReporter
{
    private static readonly List<Order> _orders = new()
    {
        new Order { Id = 1, CustomerName = "Ali",  Amount = 1200, Status = "Completed" },
        new Order { Id = 2, CustomerName = "Ayşe", Amount = 350,  Status = "Pending"   },
        new Order { Id = 3, CustomerName = "Veli", Amount = 5000, Status = "Completed" }
    };
    private static int _nextId = 4;

    public Order? GetById(int id)              => _orders.FirstOrDefault(o => o.Id == id);
    public List<Order> GetAll()                => _orders;
    public List<Order> GetByStatus(string s)   => _orders.Where(o => o.Status == s).ToList();
    public void Add(Order o)                   { o.Id = _nextId++; _orders.Add(o); }
    public void Update(Order o)                { var i = _orders.FindIndex(x => x.Id == o.Id); if (i >= 0) _orders[i] = o; }
    public void Delete(int id)                 => _orders.RemoveAll(o => o.Id == id);
    public OrderSummary GetSummary()           => new(_orders.Count, _orders.Sum(o => o.Amount), _orders.Count > 0 ? _orders.Average(o => o.Amount) : 0);
    public List<Order> GetTopOrders(int n)     => _orders.OrderByDescending(o => o.Amount).Take(n).ToList();
}

// ✅ ISP uyumlu: Yalnızca IOrderReader + IOrderReporter — yazma yok, fırlatma yok
public class DashboardOrderRepository : IOrderReader, IOrderReporter
{
    private readonly OrderRepository _inner = new();

    public Order? GetById(int id)            => _inner.GetById(id);
    public List<Order> GetAll()              => _inner.GetAll();
    public List<Order> GetByStatus(string s) => _inner.GetByStatus(s);
    public OrderSummary GetSummary()         => _inner.GetSummary();
    public List<Order> GetTopOrders(int n)   => _inner.GetTopOrders(n);
    // ✅ Add/Update/Delete yok — sözleşmede yok, implement zorunda değil
}
