// ✅ Async Initialization — 3 kabul görmüş pattern:
//    1. Static async factory method (en yaygın)
//    2. Lazy<Task<T>> (singleton, bir kez init)
//    3. IAsyncDisposable + InitializeAsync (DI container desteği)

Console.WriteLine("=== Pattern 1: Static Factory Method ===");
var repo = await UserRepository.CreateAsync();  // ✅ await edilebilir fabrika
Console.WriteLine($"Kullanıcılar: {await repo.GetCountAsync()}");

Console.WriteLine("\n=== Pattern 2: Lazy<Task<T>> — tek seferlik init ===");
var settings = new AppSettings();
var config1  = await settings.GetConfigAsync(); // ✅ ilk çağrıda init, sonra cache
var config2  = await settings.GetConfigAsync(); // ✅ init tekrar çalışmaz
Console.WriteLine($"Config: {config1} (tekrar: {config2})");

Console.WriteLine("\n=== Pattern 3: InitializeAsync metodu (DI senaryosu) ===");
var service = new DataService();
await service.InitializeAsync();  // ✅ DI container veya host bu metodu çağırır
Console.WriteLine($"Servis hazır: {await service.IsHealthyAsync()}");

// ✅ Pattern 1: Static async factory — constructor private, dışarıdan new yapılamaz
class UserRepository
{
    private List<string> _users = new();

    private UserRepository() { }

    public static async Task<UserRepository> CreateAsync()
    {
        var repo = new UserRepository();
        await repo.InitializeAsync().ConfigureAwait(false);
        return repo;
    }

    private async Task InitializeAsync()
    {
        Console.WriteLine("[DB] Bağlanılıyor...");
        await Task.Delay(300).ConfigureAwait(false);
        _users = new List<string> { "Ali", "Ayşe", "Mehmet" };
        Console.WriteLine("[DB] Hazır.");
    }

    public async Task<int> GetCountAsync()
    {
        await Task.Delay(10).ConfigureAwait(false);
        return _users.Count;
    }
}

// ✅ Pattern 2: Lazy<Task<T>> — init bir kez çalışır, thread-safe
class AppSettings
{
    private readonly Lazy<Task<string>> _config = new(LoadConfigAsync);

    public Task<string> GetConfigAsync() => _config.Value;

    private static async Task<string> LoadConfigAsync()
    {
        Console.WriteLine("[CONFIG] Yükleniyor...");
        await Task.Delay(200).ConfigureAwait(false);
        return "{ \"env\": \"production\" }";
    }
}

// ✅ Pattern 3: InitializeAsync — ASP.NET Core IHostedService / DI senaryosu
class DataService
{
    private bool _initialized;

    public async Task InitializeAsync()
    {
        Console.WriteLine("[SERVICE] Başlatılıyor...");
        await Task.Delay(150).ConfigureAwait(false);
        _initialized = true;
        Console.WriteLine("[SERVICE] Hazır.");
    }

    public async Task<bool> IsHealthyAsync()
    {
        await Task.Delay(10).ConfigureAwait(false);
        return _initialized;
    }
}
