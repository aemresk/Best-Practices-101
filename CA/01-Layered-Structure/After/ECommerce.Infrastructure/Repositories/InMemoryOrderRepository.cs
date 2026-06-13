using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Repositories;

// ✅ Infrastructure, Domain arayüzünü uygular — Domain bu sınıfı tanımaz
public class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _store = new();
    private int _sequence = 1;

    public void Save(Order order)             => _store.Add(order);
    public Order? GetById(int id)             => _store.FirstOrDefault(o => o.Id == id);
    public IReadOnlyList<Order> GetAll()      => _store.AsReadOnly();
    public int NextId()                       => _sequence++;
}
