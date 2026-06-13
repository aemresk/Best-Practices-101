// ✅ Composition Root: bağımlılıklar burada kurulur, use case çağrılır.
using Application.RegisterUser;
using Infrastructure;

var handler = new RegisterUserHandler(
    new InMemoryUserRepository(),
    new Sha256PasswordHasher(),
    new ConsoleEmailService()
);

var result = handler.Handle(new RegisterUserCommand("ali@ornek.com", "Güçlü!Şifre123", "Ali Veli"));
Console.WriteLine($"✅ Kayıt başarılı: #{result.UserId} — {result.Email}");

try
{
    handler.Handle(new RegisterUserCommand("", "abc", ""));
}
catch (Exception ex)
{
    Console.WriteLine($"❌ {ex.Message}");
}
