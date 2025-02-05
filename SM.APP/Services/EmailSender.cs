namespace SM.APP.Services
{
    using Microsoft.AspNetCore.Identity.UI.Services;
    using System.Threading.Tasks;

    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Simulate email sending (just log the email for now)
            
            return Task.CompletedTask;
        }
    }

}
