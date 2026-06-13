using Domain;

namespace Application;

// ✅ Event dispatcher: handler'ları event'lere bağlar
public class EventDispatcher
{
    private readonly Dictionary<Type, List<Action<IDomainEvent>>> _handlers = new();

    public void Register<TEvent>(Action<TEvent> handler) where TEvent : IDomainEvent
    {
        var type = typeof(TEvent);
        if (!_handlers.ContainsKey(type))
            _handlers[type] = new();

        _handlers[type].Add(e => handler((TEvent)e));
    }

    public void Dispatch(IEnumerable<IDomainEvent> events)
    {
        foreach (var e in events)
        {
            if (_handlers.TryGetValue(e.GetType(), out var handlers))
                foreach (var handler in handlers)
                    handler(e);
        }
    }
}
