using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Project_Sem3.Models.MailContent;

namespace Project_Sem3.Services.SendMail;

public class SendMailService : ISendMailService
{
    private readonly MailSettings _mailSettings;
    private readonly ILogger<SendMailService> _logger;
    public SendMailService (IOptions<MailSettings> mailSettings, ILogger<SendMailService> logger) {
        _mailSettings = mailSettings.Value;
        _logger = logger;
        logger.LogInformation("Create SendMailService");
    }
    public async Task<bool> SendMail (MailContent mailContent)
    {
        bool check = false;
        var email = new MimeMessage ();
        email.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
        email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
        email.To.Add (MailboxAddress.Parse (mailContent.To));
        email.Subject = mailContent.Subject;
        
        var builder = new BodyBuilder();
        builder.HtmlBody = mailContent.Body;
        email.Body = builder.ToMessageBody ();

        // dùng SmtpClient của MailKit
        using var smtp = new MailKit.Net.Smtp.SmtpClient();

        try
        {
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            check = true;
        }
        catch (Exception ex)
        {
            // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
            System.IO.Directory.CreateDirectory("mailssave");
            var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
            await email.WriteToAsync(emailsavefile);

            _logger.LogInformation("Lỗi gửi mail, lưu tại - " + emailsavefile);
            _logger.LogError(ex.Message);
        }

        smtp.Disconnect (true);

        _logger.LogInformation("send mail to " + mailContent.To);
        
        return check;
    }
    public async Task SendEmailAsync(string email, string subject, string htmlMessage) {
        await SendMail(new MailContent() {
            To = email,
            Subject = subject,
            Body = htmlMessage
        });
    }
}