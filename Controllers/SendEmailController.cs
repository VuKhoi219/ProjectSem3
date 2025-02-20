using Microsoft.AspNetCore.Mvc;
using Project_Sem3.Models.MailContent;
using Project_Sem3.Services.SendMail;

namespace Project_Sem3.Controllers;

[ApiController]
[Route("api/send-email/")]
public class SendEmail : Controller
{
    private readonly ISendMailService _sendMailService;

    public SendEmail(ISendMailService sendMailService)
    {
        _sendMailService = sendMailService;
    }
    
    [HttpPost("send")]
    public async Task<IActionResult> SendtoEmail([FromBody] MailContent mailContent)
    {
        // vi dụ
        // MailContent content = new MailContent {
        //     To = "ellyproe@gmail.com",
        //     Subject = "Kiểm tra thử",
        //     Body = "<p><strong>Xin chào xuanthulab.net</strong></p>"
        // };
        bool result =   await _sendMailService.SendMail(mailContent);
        return Ok(result);
    }
}