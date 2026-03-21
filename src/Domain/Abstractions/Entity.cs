namespace Domain.Abstractions;

public abstract class Entity
{
    private readonly List<object> domainEvents = [];

    public Guid Id { get; protected init; } = Guid.NewGuid();

    public IReadOnlyCollection<object> DomainEvents => domainEvents.AsReadOnly();

    protected void AddDomainEvent(object domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }
}
