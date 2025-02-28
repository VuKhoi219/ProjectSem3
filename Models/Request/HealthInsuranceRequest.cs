namespace Project_Sem3.Models.Request;

public class HealthInsuranceRequest
{
  public int Age { get; set; }
  public int[] HealthStatusIds { get; set; }
  public int[] CareerIds { get; set; } // Đổi từ string Career sang int[] CareerIds
  public int[] LifestyleIds { get; set; }
  public decimal CoverageAmount { get; set; }
  public int ContractDuration { get; set; }
}
