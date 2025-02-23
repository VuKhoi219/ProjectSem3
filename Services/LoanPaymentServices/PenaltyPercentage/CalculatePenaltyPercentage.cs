namespace Project_Sem3.Services.LoanPaymentServices.PenaltyPercentage;

public class CalculatePenaltyPercentage
{
  public decimal PenaltyPercentage(int daysInMonth, int checkIfOverdue)
  {
    int overdueDays = checkIfOverdue - daysInMonth;
    return overdueDays * 0.05m;
  }
}
