using Microsoft.AspNetCore.Mvc;

namespace Project_Sem3.Controllers.ViewController;

public class RequestController : BaseController
{
  public IActionResult Life()
  {
    return View();
  }

  public IActionResult Health()
  {
    return View();
  }

  public IActionResult Property()
  {
    return View();
  }
  public IActionResult Vehicle()
  {
    return View();
  }
}
