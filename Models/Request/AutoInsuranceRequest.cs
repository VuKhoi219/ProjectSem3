namespace Project_Sem3.Models.Request;

public class AutoInsuranceRequest
{
    public decimal CarValue { get; set; }
    public string CarBrand { get; set; }
    public int NumberOfAccidents { get; set; }
    public int YearsWithoutAccident { get; set; }
    public int ContractDuration { get; set; }

}