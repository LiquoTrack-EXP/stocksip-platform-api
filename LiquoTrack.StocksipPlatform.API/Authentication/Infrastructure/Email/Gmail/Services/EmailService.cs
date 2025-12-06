using System.Net;
using System.Net.Mail;
using LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Email;
using LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Email.Gmail.Confirguration;
using Microsoft.Extensions.Options;

namespace LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Email.Gmail.Services;

/// <summary>
///     Service that sends emails using Gmail.
/// </summary>
/// <param name="smtp">
///     The SMTP settings.
/// </param>
public class EmailService(IOptions<SmtpSettings> smtp) : IEmailService
{
    /// <summary>
    ///     The SMTP settings.
    /// </summary>
    private readonly SmtpSettings _smtp = smtp.Value;
    
    /// <summary>
    ///     Method to send an email.
    /// </summary>
    /// <param name="to">
    ///     Email address of the recipient.
    /// </param>
    /// <param name="subject">
    ///     Subject of the email.
    /// </param>
    /// <param name="body">
    ///     Body of the email.
    /// </param>
    public async Task SendEmail(string to, string subject, string body)
    {
        var message = new MailMessage();
        message.From = new MailAddress(_smtp.SenderEmail, _smtp.SenderName);
        message.To.Add(to);
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = true;

        using var client = new SmtpClient(_smtp.Server, _smtp.Port)
        {
            Credentials = new NetworkCredential(_smtp.UserName, _smtp.Password),
            EnableSsl = _smtp.UseSsl
        };
        
        await client.SendMailAsync(message);
    }
}