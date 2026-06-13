// ❌ DTO Mapping Eksikliği:
//    Domain entity doğrudan dışarıya döndürülüyor.
//    Hassas alanlar (PasswordHash, FailedLoginCount) istemciye görünüyor.
//    Domain modeli değiştirildiğinde API kontratı otomatik kırılıyor.
//    Entity üzerinde JSON serializer attribute'ları birikmek zorunda kalıyor.

var repo = new UserRepository();
repo.Add(new User(1, "ali@ornek.com", "Ali Veli", HashPassword("Gizli!123"), role: "Admin",
                  failedLoginCount: 2, lastLoginAt: DateTime.UtcNow.AddHours(-3)));

// ❌ Domain entity doğrudan döndürülüyor
User user = repo.GetById(1)!;
PrintResponse(user);  // PasswordHash, FailedLoginCount API response'da gözükür

void PrintResponse(User u)
{
    // Simüle edilmiş JSON serialization — gerçekte tüm field'lar gidecek
    Console.WriteLine("API Response (JSON):");
    Console.WriteLine($"  id             : {u.Id}");
    Console.WriteLine($"  email          : {u.Email}");
    Console.WriteLine($"  fullName       : {u.FullName}");
    Console.WriteLine($"  role           : {u.Role}");
    Console.WriteLine($"  passwordHash   : {u.PasswordHash}");      // ❌ GÜVENLIK RİSKİ
    Console.WriteLine($"  failedLoginCount: {u.FailedLoginCount}"); // ❌ iç detay sızdı
    Console.WriteLine($"  lastLoginAt    : {u.LastLoginAt}");
}

string HashPassword(string p) => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(p));

// Domain entity — tüm alanlar dahil iç/hassas bilgiler
class User(int id, string email, string fullName, string passwordHash,
           string role, int failedLoginCount, DateTime lastLoginAt)
{
    public int      Id               { get; } = id;
    public string   Email            { get; } = email;
    public string   FullName         { get; } = fullName;
    public string   PasswordHash     { get; } = passwordHash;   // ❌ hassas
    public string   Role             { get; } = role;
    public int      FailedLoginCount { get; } = failedLoginCount; // ❌ iç durum
    public DateTime LastLoginAt      { get; } = lastLoginAt;
}

class UserRepository
{
    private readonly List<User> _store = new();
    public void Add(User u)      => _store.Add(u);
    public User? GetById(int id) => _store.FirstOrDefault(u => u.Id == id);
}
