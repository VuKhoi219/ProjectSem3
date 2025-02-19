using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Sem3.Models;

public class Payment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int ContractId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    [Required]
    public PaymentStatus Status { get; set; }

    [MaxLength(255)]
    public string ImageUrl { get; set; }

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
    
    public virtual InsuranceContract Contract { get; set; }
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed
}
