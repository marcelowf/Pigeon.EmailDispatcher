using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pigeon.EmailDispatcher.Service.Interfaces;
using Pigeon.EmailDispatcher.Service.Services;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddScoped<IEmailService, EmailService>();

#region Azure Key Vault
builder.Configuration.AddAzureKeyVault(new Uri("https://kv-pigeon-qa.vault.azure.net/"), new DefaultAzureCredential());
#endregion

builder.Build().Run();
