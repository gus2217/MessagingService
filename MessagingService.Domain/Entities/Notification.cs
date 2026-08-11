using MediatR;
using MessagingService.Domain.Enums;
using MessagingService.Domain.Events;

namespace MessagingService.Domain.Entities;

public class Notification
{
    private Notification() { } // EF Core

    public Guid Id { get; private set; }
    public string UserId { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public NotificationType Type { get; private set; }
    public NotificationPriority Priority { get; private set; }
    public NotificationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public string? ActionUrl { get; private set; }          // Deep link
    public Dictionary<string, object> Metadata { get; private set; } // Flexible JSON
    public byte[] RowVersion { get; private set; }          // Concurrency token

    // Domain logic
    public static Notification Create(
        string userId,
        string title,
        string message,
        NotificationType type = NotificationType.System,
        NotificationPriority priority = NotificationPriority.Medium,
        string? actionUrl = null,
        Dictionary<string, object>? metadata = null)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            Priority = priority,
            Status = NotificationStatus.Unread,
            CreatedAt = DateTime.UtcNow,
            ActionUrl = actionUrl,
            Metadata = metadata ?? new Dictionary<string, object>()
        };

        // Raise domain event – will be handled to send SignalR
        notification.AddDomainEvent(new NotificationCreatedDomainEvent(notification));
        return notification;
    }

    public void MarkAsRead()
    {
        if (Status == NotificationStatus.Unread)
        {
            Status = NotificationStatus.Read;
            ReadAt = DateTime.UtcNow;
        }
    }

    public void MarkAsDismissed()
    {
        if (Status == NotificationStatus.Unread || Status == NotificationStatus.Read)
            Status = NotificationStatus.Dismissed;
    }

    // --- Domain Event support (in-memory) ---
    private List<INotification> _domainEvents = new();
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();
    public void AddDomainEvent(INotification eventItem) => _domainEvents.Add(eventItem);
    public void RemoveDomainEvent(INotification eventItem) => _domainEvents.Remove(eventItem);
    public void ClearDomainEvents() => _domainEvents.Clear();
}