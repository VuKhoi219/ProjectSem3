using System.ComponentModel.DataAnnotations;

namespace Project_Sem3.Models;

public class InsuranceHealthDetail : InsuranceDetails
{
    [MaxLength(50)]
    public string AgeGroup { get; set; }

    public string HospitalNetwork { get; set; }

    public string PreExistingConditions { get; set; }

    public int Duration { get; set; }

    public decimal RiskFactor { get; set; }

    [MaxLength(100)]
    public string Region { get; set; }
}