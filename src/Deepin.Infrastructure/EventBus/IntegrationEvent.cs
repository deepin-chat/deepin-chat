namespace Deepin.Infrastructure.EventBus;
public abstract record IntegrationEvent
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}