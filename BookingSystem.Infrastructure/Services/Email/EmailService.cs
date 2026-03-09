using BookingSystem.Application.Interfaces.Email;
using BookingSystem.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BookingSystem.Infrastructure.Services.Email;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
    {
        _smtpSettings = smtpSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string toName, string subject, string body)
    {
        var attempt = 0;
        var lastException = default(Exception);

        while(attempt < _smtpSettings.MaxRetryAttempts)
        {
            try
            {
                attempt++;
                _logger.LogInformation("Sending email to {Email}, attempt {Attempt}", toEmail, attempt);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = body };

                using var client = new SmtpClient();
                await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
                return;
            }
            catch (Exception ex)
            {
                lastException = ex;
                _logger.LogWarning("Email attempt {Attempt} failed for {Email}: {Error}", attempt, toEmail, ex.Message);

                if (attempt < _smtpSettings.MaxRetryAttempts)
                    await Task.Delay(TimeSpan.FromSeconds(_smtpSettings.RetryDelaySeconds));

            }
        }

        _logger.LogError("All {MaxAttempts} email attempts failed for {Email}", _smtpSettings.MaxRetryAttempts, toEmail);
        throw new InvalidOperationException($"Failed to send email after {_smtpSettings.MaxRetryAttempts} attempts", lastException);
    }

}
