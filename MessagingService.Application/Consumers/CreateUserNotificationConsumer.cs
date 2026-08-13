using MassTransit;
using MediatR;
using MessagingService.Application.Contracts;
using MessagingService.Domain.Entities;
using MessagingService.Domain.Events;
using MessagingService.Domain.Common.Interfaces;

namespace MessagingService.Application.Consumers;

public class CreateUserNotificationConsumer : IConsumer<ICreateUserNotification>
{
    private readonly IMediator _mediator;
    private readonly INotificationRepository _repository;

    public CreateUserNotificationConsumer(IMediator mediator, INotificationRepository repository)
    {
        _mediator = mediator;
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<ICreateUserNotification> context)
    {
        var message = context.Message;

        var notification = Notification.Create(
            userId: message.UserId,
            title: message.Title,
            message: message.Message,
            type: Enum.TryParse<MessagingService.Domain.Enums.NotificationType>(message.Type, out var t) ? t : MessagingService.Domain.Enums.NotificationType.System,
            priority: Enum.TryParse<MessagingService.Domain.Enums.NotificationPriority>(message.Priority, out var p) ? p : MessagingService.Domain.Enums.NotificationPriority.Medium,
            actionUrl: message.ActionUrl
        );

        await _repository.AddAsync(notification, context.CancellationToken);

        await _mediator.Publish(new NotificationCreatedDomainEvent(notification), context.CancellationToken);
    }
}
