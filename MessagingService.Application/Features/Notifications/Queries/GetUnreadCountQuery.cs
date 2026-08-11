using MediatR;
using System.Text.Json;
using MessagingService.Application.Common.Interfaces;

namespace MessagingService.Application.Features.Notifications.Queries;

public record GetUnreadCountQuery : IRequest<int>;

public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, int>
{
    private readonly INotificationRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly IDistributedCache _cache;

    public GetUnreadCountQueryHandler(INotificationRepository repo, ICurrentUserService currentUser, IDistributedCache cache)
    {
        _repo = repo;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<int> Handle(GetUnreadCountQuery request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        var cacheKey = $"unread_count_{userId}";

        var cached = await _cache.GetStringAsync(cacheKey, ct);
        if (cached is not null)
            return JsonSerializer.Deserialize<int>(cached);

        var count = await _repo.GetUnreadCountAsync(userId, ct);
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(count), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5) // short TTL for freshness
        }, ct);

        return count;
    }
}