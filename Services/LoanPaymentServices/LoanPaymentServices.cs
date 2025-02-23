using Project_Sem3.Services.LoanPaymentServices.CheckIfOverdue;
using Project_Sem3.Services.LoanPaymentServices.PenaltyPercentage;

namespace Project_Sem3.Services.LoanPaymentServices;

public class LoanPaymentServices
{
  private readonly CalculatePenaltyPercentage _calculatePenaltyPercentage;
  private readonly CalculateCheckIfOverdue _calculateCheckIfOverdue;

  public LoanPaymentServices(CalculateCheckIfOverdue calculateCheckIfOverdue,
    CalculatePenaltyPercentage calculatePenaltyPercentage)
  {
    _calculatePenaltyPercentage = calculatePenaltyPercentage;
    _calculateCheckIfOverdue = calculateCheckIfOverdue;
  }

  public (decimal,decimal) CalculatePaymentAmount(DateTime nowDateTime , DateTime loandateTime , decimal loanAmount , decimal
    monthlyPaymentAmount)
  {
    int checkIfOverdue = _calculateCheckIfOverdue.CheckIfOverdue(nowDateTime , loandateTime);
    int daysInMonth = DateTime.DaysInMonth(nowDateTime.Year, nowDateTime.Month);
    decimal penaltyPercentage = 0;
    if (checkIfOverdue > daysInMonth)
    {
      penaltyPercentage = _calculatePenaltyPercentage.PenaltyPercentage(daysInMonth, checkIfOverdue);
    }
    decimal penaltyAmount = penaltyPercentage * loanAmount / 100;
    Console.WriteLine(penaltyAmount);
    return (monthlyPaymentAmount + penaltyAmount, penaltyPercentage);
  }
}
