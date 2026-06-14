// ❌ Async Initialization Anti-pattern:
//    Constructor'da async iş yapılamaz — constructor senkron olmak zorunda.
//    Çözüm olarak .Result veya blocking çağrı kullanılıyor → deadlock riski.
//    Ya da "henüz hazır değil" durumunda nesne kullanılabiliyor.

Console.WriteLine("Bağlantı kuruluyor...");
var repo = new UserRepository();   // ❌ constructor'da blocking init
Console.WriteLine($"Kullanıcılar: {await repo.GetCountAsync()}");

class UserRepository
{
    private List<string> _users = new();

    public UserRepository()
    {
        // ❌ Constructor async olamaz — .Result ile zorla bloke ediliyor
        InitializeAsync().GetAwaiter().GetResult(); // ← deadlock riski!
    }

    private async Task InitializeAsync()
    {
        Console.WriteLine("[DB] Bağlanılıyor...");
        await Task.Delay(300); // simüle DB bağlantısı
        _users = new List<string> { "Ali", "Ayşe", "Mehmet" };
        Console.WriteLine("[DB] Bağlantı kuruldu, veriler yüklendi.");
    }

    public async Task<int> GetCountAsync()
    {
        await Task.Delay(10);
        return _users.Count;
    }
}
