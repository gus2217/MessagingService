using EmailGateway.Models;

namespace EmailGateway.Services;

public interface IEmailService
{
    void SendEmail(Message message);
}