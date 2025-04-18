using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using PustokTask.Settings;
using PustokTask.ViewModels;

namespace PustokTask.Services;

public class EmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public void SendEmail(string to, string subject, string body)
    {
        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(_emailSettings.FromEmail));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        smtp.Connect(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
        smtp.Authenticate(_emailSettings.FromEmail, _emailSettings.SmtpPass);
        smtp.Send(email);
        smtp.Disconnect(true);
    }
}
