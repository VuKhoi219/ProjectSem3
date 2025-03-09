using Microsoft.AspNetCore.Mvc;

namespace Project_Sem3.Controllers.ViewController;

public class LoanPaymentController : BaseController
{
  public IActionResult Create()
  {
    return View();
  }
}
