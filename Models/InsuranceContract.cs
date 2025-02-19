using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices.JavaScript;

namespace Project_Sem3.Models;

public class InsuranceContract
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int PlanId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public ContractStatus Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public int? DeleteBy { get; set; }
    
    // Navigation Properties
    public virtual User User { get; set; }
    public virtual User Creator { get; set; }
    public virtual User Updater { get; set; }
    public virtual User Deleter { get; set; }
    public virtual InsurancePlan Plan { get; set; }

    // Relationship
    public virtual ICollection<Payment> Payments { get; set; }
}
public enum ContractStatus
{
    Pending,
    Active,
    Expired,
    Cancelled
}
