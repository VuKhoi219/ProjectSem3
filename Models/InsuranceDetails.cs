using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Sem3.Models;

public abstract class InsuranceDetails
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PlanId { get; set; }

    [Required]
    public decimal Premium { get; set; }

    [Required]
    public decimal CoverageAmount { get; set; }

    [Required]
    public decimal Deductible { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public int? DeleteBy { get; set; }
    
    public virtual InsurancePlan Plan { get; set; }
    public virtual User Creator { get; set; }
    public virtual User Updater { get; set; }
    public virtual User Deleter { get; set; }
}
