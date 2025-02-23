using Bogus.DataSets;

namespace Project_Sem3.Services.LoanPaymentServices.CheckIfOverdue;

public class CalculateCheckIfOverdue
{
  public int CheckIfOverdue(DateTime nowDateTime, DateTime loanDateTime)
  {
    DateTime onlyNowDate = nowDateTime.Date;
    DateTime onlyLoanDate = loanDateTime.Date;
    return (onlyNowDate - onlyLoanDate).Days;
  }
}
