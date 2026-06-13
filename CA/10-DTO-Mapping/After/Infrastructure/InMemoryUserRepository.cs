using Application;
using Domain;

namespace Infrastructure;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _store = new();
    private int _sequence = 1;

    public User? GetById(int id) => _store.FirstOrDefault(u => u.Id == id);
    public void  Save(User user)  => _store.Add(user);
    public int   NextId()         => _sequence++;
}
