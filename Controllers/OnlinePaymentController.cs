using Microsoft.AspNetCore.Mvc;
using Project_Sem3.Models.MyBank;
using Project_Sem3.Services;

namespace Project_Sem3.Controllers;
[ApiController]
[Route("api/online-payment/")]
public class OnlinePaymentController : Controller
{
    private readonly OnlinePaymentServices _onlinePaymentServices;

    public OnlinePaymentController(OnlinePaymentServices onlinePaymentServices)
    {
        _onlinePaymentServices = onlinePaymentServices;
    }

    [HttpPost("generate-qr-url")]
    public IActionResult GenerateQrUrl([FromBody] PaymentContent paymentContent)
    {
        // decimal am = 100000.0m;
        // string value = "test";
        var result = _onlinePaymentServices.GenerateQrUrl(paymentContent.Amount, paymentContent.Value);
        return Ok(result);
    }
    
}