using Microsoft.AspNetCore.Mvc;
using Project_Sem3.Controllers.ViewController;
using Project_Sem3.Models.ViewModel;

public class HomeController : BaseController
{
    public IActionResult Index()
    {
      return View();    }

    public IActionResult AboutUs()
    {
        return View(new Project_Sem3.Models.ViewModel.PageHeaderViewModel
        {
            Title = "About Us",
            BreadcrumbItems = new List<string> { "Home", "About" }
        });
    }

    public IActionResult Service()
    {
      return View(new Project_Sem3.Models.ViewModel.PageHeaderViewModel
      {
        Title = "Insurance Services",
        BreadcrumbItems = new List<string> { "Home", "Services" }
      });
    }

    public IActionResult Support()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View(new Project_Sem3.Models.ViewModel.PageHeaderViewModel
        {
            Title = "Contact Us",
            BreadcrumbItems = new List<string> { "Home", "Contact" }
        });
    }

    public IActionResult PageSuccess()
    {
      return View();
    }


}
