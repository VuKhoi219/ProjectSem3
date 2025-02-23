using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Sem3.Models;

public class LoanPayment
{
  [Key]
  public int Id { get; set; }

  [Required]
  public int BorrowId { get; set; }

  [Required]
  public decimal PaymentAmount { get; set; }

  [Required]
  public DateTime PaymentDate { get; set; } = DateTime.Now;
  [Required]
  public string PaymentImage { get; set; }

  [Required]
  public int OverdueDays { get; set; }

  [Required]
  public decimal PenaltyInterest { get; set; }

  [Required] public bool Status { get; set; } = false;

  public DateTime? CreatedAt { get; set; } = DateTime.Now;

  [Required]
  public int? CreatedBy { get; set; }

  // Navigation properties
  public BorrowCapital? BorrowCapital { get; set; }
  public User? Creator { get; set; }
}
