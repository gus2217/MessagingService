using Microsoft.AspNetCore.SignalR;
using MessagingService.Application.Interfaces;

namespace MessagingService.Application.Hubs;

public class NotificationHub : Hub<INotificationClient>
{
    // Placeholder to allow Application layer to reference the hub type
    // Actual implementation is in Infrastructure
}
