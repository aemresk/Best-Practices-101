using Domain;

namespace Application.RegisterUser;

// ✅ Use Case sınıfı: tek bir iş akışını yönetir, test edilebilir, bağımlılıkları bellidir
public class RegisterUserHandler
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IEmailService   _email;

    public RegisterUserHandler(IUserRepository users, IPasswordHasher hasher, IEmailService email)
    {
        _users  = users;
        _hasher = hasher;
        _email  = email;
    }

    public RegisterUserResult Handle(RegisterUserCommand command)
    {
        // ✅ Validasyon tek yerde — her caller buraya güvenir
        Validate(command);

        if (_users.ExistsByEmail(command.Email))
            throw new InvalidOperationException("Bu e-posta zaten kayıtlı");

        var hash = _hasher.Hash(command.Password);
        var user = User.Register(_users.NextId(), command.Email, command.FullName, hash);

        _users.Save(user);

        // ✅ E-posta: arayüz üzerinden — SMTP mi, SendGrid mi bilmiyoruz
        _email.SendWelcome(user.Email, user.FullName);

        Console.WriteLine($"[LOG] {DateTime.UtcNow:O} | UserRegistered | {user.Email}");

        return new RegisterUserResult(user.Id, user.Email);
    }

    private static void Validate(RegisterUserCommand cmd)
    {
        if (string.IsNullOrWhiteSpace(cmd.Email) || !cmd.Email.Contains('@'))
            throw new ArgumentException("Geçerli e-posta zorunlu");
        if (string.IsNullOrWhiteSpace(cmd.FullName))
            throw new ArgumentException("Ad soyad zorunlu");
        if (cmd.Password.Length < 8)
            throw new ArgumentException("Şifre en az 8 karakter olmalı");
        if (!cmd.Password.Any(char.IsUpper) || !cmd.Password.Any(char.IsDigit))
            throw new ArgumentException("Şifre büyük harf ve rakam içermeli");
    }
}
