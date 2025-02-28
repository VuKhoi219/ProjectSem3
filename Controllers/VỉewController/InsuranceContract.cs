using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Project_Sem3.Controllers.VỉewController;

public class InsuranceContract : Controller
{
  public IActionResult Index()
  {
    return View();
  }
  public IActionResult Detail(int id)
  {
    return View(id);
  }
}
