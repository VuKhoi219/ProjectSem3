using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Project_Sem3.Controllers.ViewController;

public class InsuranceContractController : BaseController
{
  public IActionResult Index()
  {
    return View();
  }
  public IActionResult Detail()
  {
    return View();
  }
}
