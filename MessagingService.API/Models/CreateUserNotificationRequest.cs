namespace MessagingService.API.Models;

public record CreateUserNotificationRequest(
    string UserId,
    string Title,
    string Message,
    string Type,
    string Priority,
    string? ActionUrl
);
