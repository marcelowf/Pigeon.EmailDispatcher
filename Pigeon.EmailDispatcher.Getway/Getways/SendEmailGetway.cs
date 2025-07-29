using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Pigeon.EmailDispatcher.Service.Interfaces;
using Pigeon.EmailDispatcher.Service.Models;

namespace Pigeon.EmailDispatcher.Getway.Getways
{
    public class SendEmailGetway
    {
        private readonly ILogger<SendEmailGetway> _logger;
        private readonly IEmailService _emailService;

        public SendEmailGetway(ILogger<SendEmailGetway> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        [Function(nameof(SendEmailGetway))]
        public async Task Run([ServiceBusTrigger("%EmailQueueName%", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message received with ID: {id}", message.MessageId);

            var content = message.Body.ToString();
            List<EmailModel>? models = null;

            try
            {
                models = JsonSerializer.Deserialize<List<EmailModel>>(content);


                if (models == null)
                {
                    _logger.LogError("Deserialization returned null.");
                    await messageActions.AbandonMessageAsync(message);
                    return;
                }

                foreach (var emailModel in models)
                {
                    await _emailService.SendEmailAsync(emailModel);
                }

                await messageActions.CompleteMessageAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message.");
                await messageActions.AbandonMessageAsync(message);
            }
        }
    }
}