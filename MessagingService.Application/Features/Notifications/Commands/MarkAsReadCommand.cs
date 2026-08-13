using MediatR;
using MessagingService.Domain.Common.Interfaces;

namespace MessagingService.Application.Features.Notifications.Commands;

public record MarkAsReadCommand(Guid? NotificationId = null) : IRequest<bool>;

public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, bool>
{
    private readonly INotificationRepository _repo;

    public MarkAsReadCommandHandler(INotificationRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(MarkAsReadCommand request, CancellationToken ct)
    {
        if (request.NotificationId.HasValue)
        {
            var notification = await _repo.GetByIdAsync(request.NotificationId.Value, ct);
            if (notification is null)
                return false;

            notification.MarkAsRead();
            await _repo.UpdateAsync(notification, ct);
            return true;
        }

        // Global MarkAllAsRead
        await _repo.MarkAllAsReadAsync(ct);
        return true;
    }
}