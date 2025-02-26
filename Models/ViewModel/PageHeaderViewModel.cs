namespace Project_Sem3.Models.ViewModel;

public class PageHeaderViewModel
{
  public string Title { get; set; }
  public List<string> BreadcrumbItems { get; set; } = new List<string>();
}
