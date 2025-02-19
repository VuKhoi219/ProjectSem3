using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Sem3.Models;

public class BorrowCapital
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public decimal LoanAmount { get; set; }

    [MaxLength(10)]
    public string Currency { get; set; } = "VND";  // Default value is 'VND'

    [Required]
    public decimal InterestRate { get; set; }

    [MaxLength(255)]
    public string LoanPurpose { get; set; }

    public DateTime LoanDate { get; set; }

    [Required]
    public decimal RepaymentAmount { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    public PaymentSchedule PaymentSchedule { get; set; }

    [MaxLength(100)]
    public string BankTransactionId { get; set; }

    public string BankStatus { get; set; }

    [Required]
    public string Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public int? DeleteBy { get; set; }
    
    public virtual User User { get; set; }
    public virtual User Creator { get; set; }
    public virtual User Updater { get; set; }
    public virtual User Deleter { get; set; }

}

public enum PaymentSchedule
{
    Monthly,
    Quarterly,
    Yearly
}
