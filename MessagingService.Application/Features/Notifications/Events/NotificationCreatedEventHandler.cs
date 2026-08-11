using MediatR;
using Microsoft.AspNetCore.SignalR;
using MessagingService.Domain.Events;

namespace MessagingService.Application.Features.Notifications.Events;

public class NotificationCreatedEventHandler : INotificationHandler<NotificationCreatedDomainEvent>
{
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
    private readonly IDistributedCache _cache;

    public NotificationCreatedEventHandler(IHubContext<NotificationHub, INotificationClient> hubContext, IDistributedCache cache)
    {
        _hubContext = hubContext;
        _cache = cache;
    }

    public async Task Handle(NotificationCreatedDomainEvent domainEvent, CancellationToken ct)
    {
        var notification = domainEvent.Notification;
        // Invalidate cache for this user
        await _cache.RemoveAsync($"unread_count_{notification.UserId}", ct);

        // Broadcast real-time to the specific user's group
        await _hubContext.Clients.Group($"user_{notification.UserId}")
            .ReceiveNotification(new
            {
                notification.Id,
                notification.Title,
                notification.Message,
                notification.Type,
                notification.Priority,
                notification.CreatedAt,
                notification.ActionUrl
            });
    }
}