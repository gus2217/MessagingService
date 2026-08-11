using MediatR;
using MessagingService.Application.Features.Notifications.Commands;
using Microsoft.AspNetCore.Mvc;

namespace MessagingService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTime? cursor, [FromQuery] int take = 20)
        => Ok(await _mediator.Send(new Application.Features.Notifications.Queries.GetNotificationsQuery(cursor, take)));

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
        => Ok(await _mediator.Send(new Application.Features.Notifications.Queries.GetUnreadCountQuery()));

    [HttpPost("mark-read")]
    public async Task<IActionResult> MarkAsRead([FromBody] MarkAsReadCommand command)
        => Ok(await _mediator.Send(command));
}