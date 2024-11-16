using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ProvEditorNET.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public Task SendEmailAsync(string email, string subject, string message)
    {
        // Visit https://app.sendgrid.com/ for config and more
        // TODO - dotnet secret manager is only for development use KeyVault or something for prod
        var sendGridApiKey = _configuration["Authentication:SendGrid:ApiKey"];
        var client = new SendGridClient(sendGridApiKey);
        var from = new EmailAddress("adtofaust@gmail.com", "ProvEditor");
        var to = new EmailAddress(email, "Hello New User");
        var plainTextContent = message;
        var htmlContent = $"<strong>{message}</strong>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = client.SendEmailAsync(msg);

        return Task.CompletedTask;
    }
}