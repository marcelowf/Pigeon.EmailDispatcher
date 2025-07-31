using Microsoft.Extensions.Configuration;
using MimeKit;
using Pigeon.EmailDispatcher.Service.Interfaces;
using Pigeon.EmailDispatcher.Service.Models;
using MimeContentType = MimeKit.ContentType;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Pigeon.EmailDispatcher.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly string _templatePath;

        public EmailService(IConfiguration config)
        {
            _config = config;
            _templatePath = Path.Combine(AppContext.BaseDirectory, "Services", "EmailTemplates", "EmailLayout.html");
        }

        public async Task SendEmailAsync(EmailModel queueMessage)
        {
            var fullHtml = await WrapWithTemplateAsync(queueMessage.Message ?? string.Empty);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("M.W.F. Applications", _config["EmailSettings:FromEmail"]));
            foreach (var recipient in queueMessage.ToRecipients)
                message.To.Add(MailboxAddress.Parse(recipient));

            if (queueMessage.CcRecipients != null)
            {
                foreach (var cc in queueMessage.CcRecipients)
                    message.Cc.Add(MailboxAddress.Parse(cc));
            }

            if (queueMessage.BccRecipients != null)
            {
                foreach (var bcc in queueMessage.BccRecipients)
                    message.Bcc.Add(MailboxAddress.Parse(bcc));
            }

            message.Subject = queueMessage.Subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = fullHtml };

            if (queueMessage.Attachments != null)
            {
                foreach (var attachment in queueMessage.Attachments)
                {
                    var fileBytes = Convert.FromBase64String(attachment.ContentBase64);
                    bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, MimeContentType.Parse(attachment.ContentType));
                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            client.Timeout = 10000;

            await client.ConnectAsync(
                _config["EmailSettings:SmtpHost"],
                int.Parse(_config["EmailSettings:SmtpPort"]),
                MailKit.Security.SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _config["EmailSettings:FromEmail"],
                _config["EmailSettings:Password"]);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        private async Task<string> WrapWithTemplateAsync(string content)
        {
            if (!File.Exists(_templatePath))
                return content;

            var template = await File.ReadAllTextAsync(_templatePath);
            return template.Replace("{{BODY_CONTENT}}", content);
        }
    }
}
