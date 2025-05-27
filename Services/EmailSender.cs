using System.Net.Mail;
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
        // NOTE - _configuration service tries to fetch config/keys from both secret manager and appsettings.json 
        // string link = $"{_configuration.GetValue<string>("Host:Dev")}/api/v1/auth/confirmemailcustom?email={email}&token={confirmEmailTokenEncoded}";
        var sendGridApiKey = _configuration["Authentication:SendGrid:ApiKey"];
        Console.WriteLine($"SendGrid: {sendGridApiKey}");
        var client = new SendGridClient(sendGridApiKey);
        var from = new EmailAddress("adtofaust@gmail.com", "ProvEditor");
        // ProvEditor must be only accessible by handful of people so every new user must be approved by admin (here me)
        // we can also create roles instead -> this way everyone can register but by default will have only visibility access to endpoint, no possibility to add/edit etc
        // var to = new EmailAddress(email, "New User Approval needed");
        var to = new EmailAddress("adtofaust@gmail.com", "New User Approval needed");
        var plainTextContent = message;
        var htmlContent = $"<strong>{message}</strong>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = client.SendEmailAsync(msg);
        
        return Task.CompletedTask;
    }
}