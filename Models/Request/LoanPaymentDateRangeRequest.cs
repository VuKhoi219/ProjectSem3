namespace Project_Sem3.Models.Request;

public class LoanPaymentDateRangeRequest
{
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public int? BorrowId { get; set; }
  public bool? Status { get; set; }
}
