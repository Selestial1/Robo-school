using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace RoboSchool.Services;

public class EmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendApplicationConfirmationAsync(
        string recipientEmail,
        string recipientName,
        int applicationId,
        CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled)
        {
            _logger.LogWarning("Отправка e-mail отключена в настройках (Email:Enabled = false).");
            return;
        }

        if (string.IsNullOrWhiteSpace(_settings.Host) ||
            string.IsNullOrWhiteSpace(_settings.FromEmail))
        {
            _logger.LogWarning("SMTP не настроен: укажите Email:Host и Email:FromEmail в appsettings.json.");
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        message.To.Add(new MailboxAddress(recipientName, recipientEmail));
        message.Subject = "Ваша заявка принята — ROBO.SCHOOL";

        var htmlBody = $"""
            <p>Здравствуйте, {System.Net.WebUtility.HtmlEncode(recipientName)}!</p>
            <p>Мы получили вашу заявку №{applicationId} на сайте <strong>ROBO.SCHOOL</strong>.</p>
            <p>Наш менеджер свяжется с вами в ближайшие несколько дней по указанным контактным данным.</p>
            <p>Если у вас возникнут вопросы, звоните: <a href="tel:+78000001122">+7 800 000 11 22</a>.</p>
            <p>С уважением,<br>команда ROBO.SCHOOL</p>
            """;

        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();
        var secureSocketOptions = _settings.UseSsl
            ? SecureSocketOptions.StartTls
            : SecureSocketOptions.None;

        await client.ConnectAsync(_settings.Host, _settings.Port, secureSocketOptions, cancellationToken);

        if (!string.IsNullOrWhiteSpace(_settings.Username))
            await client.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);

        _logger.LogInformation("Письмо-подтверждение отправлено на {Email} (заявка №{Id}).", recipientEmail, applicationId);
    }
}
