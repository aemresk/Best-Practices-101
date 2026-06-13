// ✅ Tek sorumluluk: kullanıcı verilerini tutmak ve temel CRUD
// Depolama katmanı değişirse (DB, Redis...) yalnızca bu sınıf açılır
public class UserStore
{
    private static readonly List<User> _users = new();
    private static int _nextId = 1;

    public void Add(User user) { user.Id = _nextId++; _users.Add(user); }
    public User? FindByEmail(string email) => _users.FirstOrDefault(u => u.Email == email);
    public User? FindById(int id) => _users.FirstOrDefault(u => u.Id == id);
    public User? FindByResetToken(string token) => _users.FirstOrDefault(u => u.ResetToken == token);
    public List<User> GetAll() => _users;
}
