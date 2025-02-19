using System.ComponentModel.DataAnnotations;

namespace Project_Sem3.Models;

public class InsuranceVehicleDetail : InsuranceDetails
{
    [MaxLength(225)]
    public string VehicleType { get; set; }

    [MaxLength(255)]
    public string VehicleModel { get; set; }

    public int VehicleYear { get; set; }

    public int Duration { get; set; }

    public decimal RiskFactor { get; set; }

    [MaxLength(100)]
    public string Region { get; set; }
    
}