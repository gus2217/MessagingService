
using MessagingService.Domain.Enums;

namespace MessagingService.Domain.Features.Notifications.DTOs;

public record NotificationDto(
    Guid Id,
    string Title,
    string Message,
    NotificationType Type,
    NotificationPriority Priority,
    NotificationStatus Status,
    DateTime CreatedAt,
    DateTime? ReadAt,
    string? ActionUrl,
    Dictionary<string, object>? Metadata
);