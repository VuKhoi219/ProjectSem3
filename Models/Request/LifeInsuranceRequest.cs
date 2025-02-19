namespace Project_Sem3.Models.Request;

public class LifeInsuranceRequest
{
    public decimal InsuranceAmount { get; set; }
    public int Age { get; set; }
    public string HealthStatus { get; set; }
    public int ContractDuration { get; set; }

}