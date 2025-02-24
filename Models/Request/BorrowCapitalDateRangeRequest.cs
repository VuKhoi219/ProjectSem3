namespace Project_Sem3.Models.Request;

public class BorrowCapitalDateRangeRequest
{
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public int? UserId { get; set; }
  public StatusBorrowCapital? Status { get; set; }
}
