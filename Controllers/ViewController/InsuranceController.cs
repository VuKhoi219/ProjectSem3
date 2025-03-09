using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Project_Sem3.Controllers.ViewController;

public class InsuranceController : BaseController
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
