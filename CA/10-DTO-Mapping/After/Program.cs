using Application;
using Domain;
using Infrastructure;

var repo = new InMemoryUserRepository();
var queryService = new UserQueryService(repo);

// Kullanıcı oluştur ve kaydet
var user = User.Create(
    repo.NextId(), "ali@ornek.com", "Ali Veli",
    passwordHash: Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Gizli!123")),
    role: "Admin"
);
repo.Save(user);

// ✅ Public API: UserProfileDto döndürür — hassas alanlar yok
var profile = queryService.GetProfile(user.Id);
Console.WriteLine("=== Public API (UserProfileDto) ===");
Console.WriteLine($"  id          : {profile!.Id}");
Console.WriteLine($"  email       : {profile.Email}");
Console.WriteLine($"  fullName    : {profile.FullName}");
Console.WriteLine($"  role        : {profile.Role}");
Console.WriteLine($"  lastLoginAt : {profile.LastLoginAt}");
Console.WriteLine("  passwordHash: [YOK — gönderilmedi]");    // ✅ güvenli

// ✅ Admin API: ek alanlar içeriyor ama DTO ile kontrollü
var adminView = queryService.GetAdminView(user.Id);
Console.WriteLine("\n=== Admin API (UserAdminDto) ===");
Console.WriteLine($"  failedLoginCount: {adminView!.FailedLoginCount}");
Console.WriteLine($"  isActive        : {adminView.IsActive}");
Console.WriteLine("  passwordHash    : [YOK — admin DTO'sunda da yok]");  // ✅ hâlâ güvenli
