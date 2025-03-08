using Project_Sem3.Models.MailContent;

namespace Project_Sem3.Services.SendMail;

public interface ISendMailService
{
    Task<bool> SendMail(MailContent mailContent);

    Task<bool> SendEmailAsync(string email, string subject, string htmlMessage);
}
