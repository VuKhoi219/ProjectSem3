namespace Project_Sem3.Models.Request;

public class LifeInsuranceRequest
{
  public int Age { get; set; }
  public int[] HealthStatusIds { get; set; }
  public int[] CareerIds { get; set; } // Đổi từ string Career sang int[] CareerIds
  public decimal CoverageAmount { get; set; }
  public int ContractDuration { get; set; }
}
