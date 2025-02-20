using Microsoft.AspNetCore.Mvc;
using Project_Sem3.Models.MailContent;
using Project_Sem3.Services.SendMail;

namespace Project_Sem3.Controllers;

[ApiController]
[Route("api/send-email/")]
public class SendEmail : Controller
{
    private readonly SendMailService _sendMailService;
    
    [HttpPost("send")]
    public async Task<IActionResult> SendtoEmail()
    {
        MailContent content = new MailContent {
            To = "vukhoi353@gmail.com",
            Subject = "Kiểm tra thử",
            Body = "<p><strong>Xin chào xuanthulab.net</strong></p>"
        };
        await _sendMailService.SendMail(content);
        return Ok("ok");
    }
}