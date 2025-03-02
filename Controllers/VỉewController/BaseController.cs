using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Project_Sem3.Models.ViewModel;

namespace Project_Sem3.Controllers.VỉewController;

public class BaseController : Controller
{
// Không gán ViewData trong constructor nữa
  public BaseController()
  {
    // Để trống hoặc chỉ khởi tạo dữ liệu không phụ thuộc HttpContext
  }

  protected NavbarViewModel GetNavbarViewModel()
  {
    var userId = HttpContext.Session.GetInt32("UserId");
    return new NavbarViewModel
    {
      CurrentController = ControllerContext.RouteData.Values["controller"]?.ToString() ?? "Home",
      CurrentAction = ControllerContext.RouteData.Values["action"]?.ToString() ?? "Index",
      Items = new List<NavItem>
      {
        new NavItem { Text = "Home", Controller = "Home", Action = "Index" },
        new NavItem { Text = "About Us", Controller = "Home", Action = "AboutUs" },
        new NavItem { Text = "Insurance Services", Controller = "Home", Action = "Service" },
        new NavItem { Text = "Contact Us", Controller = "Home", Action = "Contact" },
        new NavItem { Text = "Borrow capital", Controller = "BorrowCapital", Action = "Index" },
        new NavItem { Text = "Transaction history", Controller = "Payment", Action = "Index" },
        new NavItem { Text = "List of contracts", Controller = "InsuranceContract", Action = "Index" },
        new NavItem { Text = "Loan Payment History", Controller = "LoanPayment", Action = "Testimonial" }
      },
      IsLoggedIn = userId.HasValue
    };
  }

  // Override OnActionExecuting để gán ViewData trước mỗi action
  public override void OnActionExecuting(ActionExecutingContext context)
  {
    base.OnActionExecuting(context);
    var userId = HttpContext.Session.GetInt32("UserId");
    ViewData["UserId"] = userId; // Truyền UserId vào ViewData
    ViewData["NavbarModel"] = GetNavbarViewModel();
  }
}
