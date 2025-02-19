using System.ComponentModel.DataAnnotations;

namespace Project_Sem3.Models;

public class InsuranceLifeDetail : InsuranceDetails
{
    public int TermYears { get; set; }

    [MaxLength(50)]
    public string AgeGroup { get; set; }

    public string Beneficiaries { get; set; }

    public int Duration { get; set; }

    public decimal RiskFactor { get; set; }

    [MaxLength(100)]
    public string Region { get; set; }
}