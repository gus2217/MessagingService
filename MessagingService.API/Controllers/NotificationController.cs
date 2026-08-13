using MassTransit;
using MessagingService.Application.Contracts;
using MessagingService.Application.Features.Notifications.Commands;
using MessagingService.API.Models;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace MessagingService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPublishEndpoint _publishEndpoint;

    public NotificationController(IMediator mediator, IPublishEndpoint publishEndpoint)
    {
        _mediator = mediator;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTime? cursor, [FromQuery] int take = 20)
        => Ok(await _mediator.Send(new MessagingService.Application.Features.Notifications.Queries.GetNotificationsQuery(cursor, take)));

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
        => Ok(await _mediator.Send(new MessagingService.Application.Features.Notifications.Queries.GetUnreadCountQuery()));

    [HttpPost("mark-read")]
    public async Task<IActionResult> MarkAsRead([FromBody] MarkAsReadCommand command)
        => Ok(await _mediator.Send(command));

    [HttpPost("trigger")]
    public async Task<IActionResult> Trigger([FromBody] CreateUserNotificationRequest request)
    {
        await _publishEndpoint.Publish<ICreateUserNotification>(new
        {
            request.UserId,
            request.Title,
            request.Message,
            request.Type,
            request.Priority,
            request.ActionUrl
        });
        return Accepted();
    }
}
