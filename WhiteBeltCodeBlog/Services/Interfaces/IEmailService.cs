using Microsoft.AspNetCore.Identity.UI.Services;
using WhiteBeltCodeBlog.Models;

namespace WhiteBeltCodeBlog.Services.Interfaces
{
    public interface IEmailService : IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
