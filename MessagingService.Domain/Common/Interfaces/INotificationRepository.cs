
using MessagingService.Domain.Features.Notifications.DTOs;
using MessagingService.Domain.Entities;

namespace MessagingService.Domain.Common.Interfaces;

public interface INotificationRepository
{
    // Write side (EF)
    Task AddAsync(Notification notification, CancellationToken ct = default);
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task UpdateAsync(Notification notification, CancellationToken ct = default);

    // Read side (Dapper) – highly optimized
    Task<(IEnumerable<NotificationDto> Items, DateTime? NextCursor)> GetAllNotificationsAsync(
        DateTime? cursor, int take, CancellationToken ct = default);

    Task<int> GetGlobalUnreadCountAsync(CancellationToken ct = default);

    // Bulk operations (raw SQL for speed)
    Task<int> MarkAllAsReadAsync(CancellationToken ct = default);
}