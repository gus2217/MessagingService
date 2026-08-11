using MediatR;
using MessagingService.Application.Common.Interfaces;

namespace MessagingService.Application.Features.Notifications.Queries;

public record GetUnreadCountQuery : IRequest<int>;

public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, int>
{
    private readonly INotificationRepository _repo;

    public GetUnreadCountQueryHandler(INotificationRepository repo)
    {
        _repo = repo;
    }

    public async Task<int> Handle(GetUnreadCountQuery request, CancellationToken ct)
    {
        return await _repo.GetGlobalUnreadCountAsync(ct);
    }
}