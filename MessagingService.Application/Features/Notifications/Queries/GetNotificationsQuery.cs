using MediatR;
using MessagingService.Application.Common.Interfaces;
using MessagingService.Application.Features.Notifications.DTOs;

namespace MessagingService.Application.Features.Notifications.Queries;

public record GetNotificationsQuery(DateTime? Cursor, int Take = 20) : IRequest<(IEnumerable<NotificationDto> Items, DateTime? NextCursor)>;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, (IEnumerable<NotificationDto> Items, DateTime? NextCursor)>
{
    private readonly INotificationRepository _repo;
    private readonly ICurrentUserService _currentUser; // get userId from context

    public GetNotificationsQueryHandler(INotificationRepository repo, ICurrentUserService currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }

    public async Task<(IEnumerable<NotificationDto> Items, DateTime? NextCursor)> Handle(GetNotificationsQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        return await _repo.GetUserNotificationsAsync(userId, request.Cursor, request.Take, ct);
    }
}