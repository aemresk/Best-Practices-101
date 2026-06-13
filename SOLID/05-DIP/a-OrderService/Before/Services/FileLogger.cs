// Somut logger — DIP öncesinde doğrudan yaratılıyor
public class FileLogger
{
    public void Log(string message)
    {
        // Gerçek projede dosyaya yazar
        Console.WriteLine($"[FILE-LOG] {DateTime.UtcNow:O} | {message}");
    }
}
