namespace Domain;

// ✅ Domain Event: domain'de olan bir şeyi temsil eder — infrastructure bilgisi yok
public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}

public record OrderPlacedEvent(
    string CustomerName,
    string ProductName,
    int    Quantity,
    DateTime OccurredAt) : IDomainEvent;
