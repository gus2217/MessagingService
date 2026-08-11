using MediatR;
using MessagingService.Domain.Entities;

namespace MessagingService.Domain.Events;

public record NotificationCreatedDomainEvent(Notification Notification) : INotification;