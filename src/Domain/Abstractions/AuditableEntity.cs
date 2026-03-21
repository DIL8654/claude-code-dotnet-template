namespace Domain.Abstractions;

public abstract class AuditableEntity : Entity
{
    public DateTimeOffset CreatedUtc { get; protected set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? LastModifiedUtc { get; protected set; }

    protected void Touch(DateTimeOffset timestamp)
    {
        LastModifiedUtc = timestamp;
    }
}
