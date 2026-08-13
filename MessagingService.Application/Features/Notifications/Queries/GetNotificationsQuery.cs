using MediatR;
using MessagingService.Domain.Features.Notifications.DTOs;
using MessagingService.Domain.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace MessagingService.Application.Features.Notifications.Queries;

public record GetNotificationsQuery(DateTime? Cursor, int Take = 20) : IRequest<(IEnumerable<NotificationDto> Items, DateTime? NextCursor)>;

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, (IEnumerable<NotificationDto> Items, DateTime? NextCursor)>
{
    private readonly INotificationRepository _repo;
    private readonly IMemoryCache _cache;
    private const string CacheKeyPrefix = "Notifications_";

    public GetNotificationsQueryHandler(INotificationRepository repo, IMemoryCache cache)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<(IEnumerable<NotificationDto> Items, DateTime? NextCursor)> Handle(GetNotificationsQuery request, CancellationToken ct)
    {
        string cacheKey = $"{CacheKeyPrefix}_{request.Cursor}_{request.Take}";

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await _repo.GetAllNotificationsAsync(request.Cursor, request.Take, ct);
        });
    }
}