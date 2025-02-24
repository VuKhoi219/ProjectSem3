namespace Project_Sem3.Services.CalculateBorrowCapital.MonthlyPaymentAmount;

public class MonthlyPaymentAmount
{
  public decimal CalculateMonthlyPaymentAmount(decimal totalAmount, decimal numberOfMonths)
  {
    return totalAmount / numberOfMonths;
  }
}
