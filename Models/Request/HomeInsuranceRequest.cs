namespace Project_Sem3.Models.Request;

public class HomeInsuranceRequest
{
  public int[] HouseTypeIds { get; set; } // Đổi từ string HouseType sang int[] HouseTypeIds
  public int[] CityIds { get; set; }      // Đổi từ string City sang int[] CityIds
  public int AssetAge { get; set; }
  public int[] MaterialIds { get; set; }  // Đổi từ string Material sang int[] MaterialIds
  public decimal CoverageAmount { get; set; }
  public int ContractDuration { get; set; }
}
