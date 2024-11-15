using Microsoft.AspNetCore.Identity.UI.Services;

namespace ProvEditorNET.Services;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        // TODO - implement sending email
        // https://medium.com/@shahidabbas9696/how-to-send-email-in-asp-net-core-web-api-949b49493f19
        
        return Task.CompletedTask;
    }
}