using Dapper;
using System.Data;
using MessagingService.Application.Common.Interfaces;
using MessagingService.Application.Features.Notifications.DTOs;
using MessagingService.Domain.Entities;

namespace MessagingService.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;
    private readonly IDbConnection _connection; // Dapper connection (same DB)

    public NotificationRepository(AppDbContext context, IDbConnection connection)
    {
        _context = context;
        _connection = connection;
    }

    // --- EF Core for writes ---
    public async Task AddAsync(Notification notification, CancellationToken ct = default)
        => await _context.Notifications.AddAsync(notification, ct);

    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Notifications.FindAsync(new object[] { id }, ct);

    public async Task UpdateAsync(Notification notification, CancellationToken ct = default)
    {
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync(ct);
    }

    // --- Dapper for high-performance reads ---
    public async Task<(IEnumerable<NotificationDto> Items, DateTime? NextCursor)> GetAllNotificationsAsync(
        DateTime? cursor, int take, CancellationToken ct = default)
    {
        // Removed UserId filter for stateless/global notification access
        var sql = @"
            SELECT Id, Title, Message, Type, Priority, Status, CreatedAt, ReadAt, ActionUrl, Metadata
            FROM Notifications
            WHERE CreatedAt < @Cursor
            ORDER BY CreatedAt DESC
            LIMIT @Take;
        ";

        var parameters = new
        {
            Cursor = cursor ?? DateTime.UtcNow.AddSeconds(1),
            Take = take + 1
        };

        var items = await _connection.QueryAsync<NotificationDto>(sql, parameters);

        var list = items.AsList();
        bool hasMore = list.Count > take;
        var resultItems = list.Take(take);
        var nextCursor = hasMore ? resultItems.LastOrDefault()?.CreatedAt : null;

        return (resultItems, nextCursor);
    }

    public async Task<int> GetGlobalUnreadCountAsync(CancellationToken ct = default)
    {
        var sql = "SELECT COUNT(*) FROM Notifications WHERE Status = 0";
        return await _connection.ExecuteScalarAsync<int>(sql);
    }

    public async Task<int> MarkAllAsReadAsync(CancellationToken ct = default)
    {
        var sql = @"
            UPDATE Notifications
            SET Status = 1, ReadAt = @Now
            WHERE Status = 0
        ";
        return await _connection.ExecuteAsync(sql, new { Now = DateTime.UtcNow });
    }
}