using Domain;

namespace Domain;

public interface IUserRepository
{
    void Save(User user);
    bool ExistsByEmail(string email);
    int  NextId();
}

public interface IPasswordHasher
{
    string Hash(string password);
}

public interface IEmailService
{
    void SendWelcome(string email, string name);
}
