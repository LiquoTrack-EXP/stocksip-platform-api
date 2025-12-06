namespace LiquoTrack.StocksipPlatform.API.Authentication.Application.Internal.OutboundServices.Email;

/**
 *      Service interface for sending emails
 */
public interface IEmailService
{
    /// <summary>
    ///     Method to send an email.
    /// </summary>
    /// <param name="to">
    ///     The email address of the recipient.
    /// </param>
    /// <param name="subject">
    ///     The subject of the email.
    /// </param>
    /// <param name="body">
    ///     Body of the email.
    /// </param>
    /// <returns>
    ///     A string representing the ID of the send email.
    /// </returns>
    Task SendEmail(string to, string subject, string body);
}