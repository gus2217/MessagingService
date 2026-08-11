// Controllers/EmailController.cs
using EmailGateway.Models;
using EmailGateway.Services;
using MessagingService.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace MessagingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IEmailService emailService, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Sends an email to one or more recipients.
        /// </summary>
        /// <param name="request">Email details.</param>
        /// <returns>Status of the send operation.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SendEmail([FromBody] EmailRequest request)
        {
            // Validate input
            if (request.To == null || !request.To.Any())
                return BadRequest("At least one recipient is required.");

            if (string.IsNullOrWhiteSpace(request.Subject))
                return BadRequest("Subject is required.");

            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Content is required.");

            try
            {
                // Map DTO to your existing Message model
                var message = new Message(request.To, request.Subject, request.Content);
                _emailService.SendEmail(message);

                _logger.LogInformation("Email sent successfully to {Recipients}", string.Join(", ", request.To));
                return Ok(new { success = true, message = "Email sent successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipients}", string.Join(", ", request.To));
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { success = false, error = "An error occurred while sending the email." });
            }
        }
    }
}