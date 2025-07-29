using Pigeon.EmailDispatcher.Service.Models;

namespace Pigeon.EmailDispatcher.Service.Interfaces
{
public interface IEmailService
    {
        Task SendEmailAsync(EmailModel QueueMessage);
    }
}