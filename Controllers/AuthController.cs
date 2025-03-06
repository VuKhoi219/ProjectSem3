using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_Sem3.Areas.Admin.Controllers;

namespace Project_Sem3.Controllers;

public class AuthController : Controller
{
  private readonly SignInManager<IdentityUser> _signInManager;
  // private readonly UserManager<IdentityUser> _userManager;
  // private readonly RoleManager<IdentityRole> _roleManager;

  public AuthController(
    SignInManager<IdentityUser> signInManager
    // UserManager<IdentityUser> _userManager,
    // RoleManager<IdentityRole> _roleManager
  )
  {
    _signInManager = signInManager;
  }

  public IActionResult ForgotPasswordBasic() => View();
  public IActionResult ForgotPasswordCover() => View();

  public IActionResult Login()
  {
    return View();
  }
  [HttpPost]
  public async Task<IActionResult> Login(string email, string password)
  {
    var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
    if (result.Succeeded)
    {
      return RedirectToAction("Index2", "Home", new { area = "Admin" });
    }
    ModelState.AddModelError("error", "Invalid login attempt.");
    return View();
  }

  [HttpGet]
  public async Task<IActionResult> Logout()
  {
    await _signInManager.SignOutAsync();
    return RedirectToAction("Login");
  }

  public IActionResult LoginCover() => View();
  public IActionResult RegisterBasic() => View();
  public IActionResult RegisterCover() => View();
  public IActionResult RegisterMultiSteps() => View();
  public IActionResult ResetPasswordBasic() => View();
  public IActionResult ResetPasswordCover() => View();
  public IActionResult TwoStepsBasic() => View();
  public IActionResult TwoStepsCover() => View();
  public IActionResult VerifyEmailBasic() => View();
  public IActionResult VerifyEmailCover() => View();
}
