using Microsoft.AspNetCore.Mvc;

namespace Project_Sem3.Areas.Admin.Controllers;

public class HomeController : AdminBaseController
{
    public IActionResult Index2()
    {
        return View();
    }
}
