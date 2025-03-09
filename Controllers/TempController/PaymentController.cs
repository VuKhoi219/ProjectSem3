using Microsoft.AspNetCore.Mvc;

namespace Project_Sem3.Controllers.ViewController;

public class PaymentController : BaseController
{
  public IActionResult Index()
  {
    return View();
  }

  public IActionResult Create()
  {
    return View();
  }
  public IActionResult Detail(int id)
  {
    return View(id);
  }
}
