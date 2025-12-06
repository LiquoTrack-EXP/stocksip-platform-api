namespace LiquoTrack.StocksipPlatform.API.Authentication.Infrastructure.Email.Gmail.Confirguration;

/**
 *      Configurations for the SMTP server.
 */
public class SmtpSettings
{
    public string Server { get; set; } = null;
    public int Port { get; set; }
    public string SenderName { get; set; } = null;
    public string SenderEmail { get; set; } = null;
    public string UserName { get; set; } = null;
    public string Password { get; set; } = null;
    public bool UseSsl { get; set; } = false;
}