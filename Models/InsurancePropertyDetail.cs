using System.ComponentModel.DataAnnotations;

namespace Project_Sem3.Models;

public class InsurancePropertyDetail : InsuranceDetails
{
    [MaxLength(255)]
    public string PropertyType { get; set; }

    [MaxLength(255)]
    public string Location { get; set; }

    public int Duration { get; set; }

    public decimal RiskFactor { get; set; }

    [MaxLength(100)]
    public string Region { get; set; }
}
