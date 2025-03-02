namespace Project_Sem3.Models.ViewModel;

public class NavbarViewModel
{
  public string CurrentController { get; set; }
  public string CurrentAction { get; set; }
  public List<NavItem> Items { get; set; } = new List<NavItem>();
  public bool IsLoggedIn { get; set; }
}

public class NavItem
{
  public string Text { get; set; }
  public string Controller { get; set; }
  public string Action { get; set; }
}
