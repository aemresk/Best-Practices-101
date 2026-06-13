using Domain;

namespace Application;

public interface IUserRepository
{
    User? GetById(int id);
    void  Save(User user);
    int   NextId();
}

// ✅ Query servisi: DTO döndürür, domain entity döndürmez
public class UserQueryService
{
    private readonly IUserRepository _repository;

    public UserQueryService(IUserRepository repository) => _repository = repository;

    public UserProfileDto? GetProfile(int userId)
    {
        var user = _repository.GetById(userId);
        return user is null ? null : UserMapper.ToProfileDto(user);
    }

    public UserAdminDto? GetAdminView(int userId)
    {
        var user = _repository.GetById(userId);
        return user is null ? null : UserMapper.ToAdminDto(user);
    }
}
