using System.Threading.Tasks;

namespace MessagingService.Application.Interfaces;

public interface INotificationClient
{
    Task ReceiveNotification(object notification);
    Task UnreadCountUpdated(int count);
}
