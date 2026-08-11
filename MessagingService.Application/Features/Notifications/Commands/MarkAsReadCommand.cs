using MediatR;
using MessagingService.Application.Common.Interfaces;

namespace MessagingService.Application.Features.Notifications.Commands;

public record MarkAsReadCommand(Guid NotificationId) : IRequest<bool>;

public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, bool>
{
    private readonly INotificationRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly IDistributedCache _cache;

    public MarkAsReadCommandHandler(INotificationRepository repo, ICurrentUserService currentUser, IDistributedCache cache)
    {
        _repo = repo;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<bool> Handle(MarkAsReadCommand request, CancellationToken ct)
    {
        var notification = await _repo.GetByIdAsync(request.NotificationId, ct);
        if (notification is null || notification.UserId != _currentUser.UserId)
            return false;

        notification.MarkAsRead();
        await _repo.UpdateAsync(notification, ct);

        // Invalidate unread count cache
        await _cache.RemoveAsync($"unread_count_{_currentUser.UserId}", ct);

        return true;
    }
}