namespace Project_Sem3.Models.Request;

public class HealthInsuranceRequest
{
    public int Age { get; set; }
    public string HealthStatus { get; set; }
    public string Career { get; set; }
    public string Lifestyle { get; set; }
    public decimal CoverageAmount { get; set; }
    public int ContractDuration { get; set; }

}