namespace Project_Sem3.Models.Request;

public class LoanPaymentRequest
{
  public DateTime NowDateTime { get; set; }
  public DateTime LoanDateTime { get; set; }
  public decimal LoanAmount { get; set; }
  public decimal MonthlyPaymentAmount { get; set; }
}
