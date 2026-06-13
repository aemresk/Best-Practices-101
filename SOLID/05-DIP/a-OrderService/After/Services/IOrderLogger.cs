// ✅ DIP: Logger soyutlaması — ElasticSearch, Seq, vb. ile swap edilebilir
public interface IOrderLogger
{
    void Log(string message);
}

public class ConsoleOrderLogger : IOrderLogger
{
    public void Log(string message) =>
        Console.WriteLine($"[LOG] {DateTime.UtcNow:O} | {message}");
}
