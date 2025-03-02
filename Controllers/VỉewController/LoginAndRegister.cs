using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Project_Sem3.Controllers.VỉewController;

public class LoginAndRegister : BaseController
{
  public IActionResult Login()
  {
    return View();
  }

  public IActionResult Register()
  {
    return View();
  }
}
