namespace Project_Sem3.Services.CalculateBorrowCapital.MonthlyPaymentAmount;

public class MonthlyPaymentAmount
{
  public decimal CalculateMonthlyPaymentAmount(decimal totalAmount, int NumberOfPayments)
  {
    return decimal.Round( totalAmount / NumberOfPayments , 1);
  }
}
