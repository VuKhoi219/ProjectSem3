namespace Project_Sem3.Models.Request;

public class VehicleInsuranceRequest
{
  public int Age { get; set; }
  public int[] VehicleInfo { get; set; } // Thay string VehicleType và VehicleBrand bằng int[] VehicleInfo
  public int[] CityIds { get; set; }     // Đổi từ string City sang int[] CityIds
  public int NumberOfAccidents { get; set; }
  public int YearsWithoutAccident { get; set; }
  public decimal CoverageAmount { get; set; }
  public int ContractDuration { get; set; }
}
