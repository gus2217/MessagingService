using MassTransit;

namespace MessagingService.Application.Contracts;

public interface ICreateUserNotification
{
    string UserId { get; }
    string Title { get; }
    string Message { get; }
    string Type { get; }
    string Priority { get; }
    string? ActionUrl { get; }
}
