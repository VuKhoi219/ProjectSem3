namespace Project_Sem3.Models.Request;

public class HomeInsuranceRequest
{
    public string HouseType { get; set; }
    public string City { get; set; }
    public int AssetAge { get; set; }
    public string Material { get; set; }
    public decimal CoverageAmount { get; set;}
    public int ContractDuration { get; set; }

}