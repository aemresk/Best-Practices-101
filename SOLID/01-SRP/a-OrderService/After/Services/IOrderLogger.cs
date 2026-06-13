// ✅ Tek sorumluluk: yalnızca loglama
public interface IOrderLogger
{
    void LogCreated(Order order);
}

public class ConsoleOrderLogger : IOrderLogger
{
    public void LogCreated(Order order) =>
        Console.WriteLine($"[LOG] {DateTime.UtcNow:O} | OrderCreated | id={order.Id} müşteri={order.CustomerName} tutar={order.FinalAmount}");
}
