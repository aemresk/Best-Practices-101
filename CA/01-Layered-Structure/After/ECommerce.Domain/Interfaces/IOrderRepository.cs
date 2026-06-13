using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces;

// ✅ Arayüz Domain katmanında tanımlanır — Infrastructure'ı bilmez
public interface IOrderRepository
{
    void Save(Order order);
    Order? GetById(int id);
    IReadOnlyList<Order> GetAll();
    int NextId();
}
