namespace Project_Sem3.Models.Request;

public class VehicleInsuranceRequest
{
    public decimal CarValue { get; set; }
    public int Age { get; set; }
    public string VehicleType { get; set; }
    public string VehicleBrand { get; set;}
    public string City { get; set; }
    public int NumberOfAccidents { get; set; }
    public int YearsWithoutAccident { get; set;}
    public decimal CoverageAmount { get; set;}
    public int ContractDuration { get; set;}
}