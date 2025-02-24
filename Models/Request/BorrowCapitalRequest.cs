namespace Project_Sem3.Models.Request;

public class BorrowCapitalRequest
{
  public decimal LoanAmount { get; set; }
  public decimal Salaly { get; set;}
  public decimal PercentageSalary { get; set; }
  public int NumberOfPayments { get; set;}

  public DateTime LoanDate { get; set; }
}
