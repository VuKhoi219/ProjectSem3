using Microsoft.Extensions.Options;
using Project_Sem3.Models.InterestRate;
using Sprache;

namespace Project_Sem3.Services.CalculateBorrowCapital;

public class CalculateBorrowCapitalServices
{
  private readonly MonthlyPaymentAmount.MonthlyPaymentAmount _monthlyPaymentAmount;
  private readonly InterestRateSetting _interestRateSetting;
  public CalculateBorrowCapitalServices(MonthlyPaymentAmount.MonthlyPaymentAmount monthlyPaymentAmount,
    IOptions<InterestRateSetting> interestRateSetting)
  {
    _monthlyPaymentAmount = monthlyPaymentAmount;
    _interestRateSetting = interestRateSetting.Value;
  }

  public (decimal,decimal) totalPaymentAmount(decimal loanAmount)
  {
    if (_interestRateSetting == null)
    {
      throw new InvalidOperationException("Interest rate settings are not initialized.");
    }
    return (loanAmount * (1m + _interestRateSetting.InterestRate / 100),_interestRateSetting.InterestRate);
  }
  public (decimal, bool) MonthlyPaymentAmount(decimal salaly, decimal percentageOfSalary, decimal totalPaymentAmount,
    int numberOfMonthly)
  {
    decimal fixedPaymentAmount =
      _monthlyPaymentAmount.CalculateMonthlyPaymentAmount(totalPaymentAmount, numberOfMonthly);
    decimal userProposedAmount = salaly * (percentageOfSalary / 100);
    if (fixedPaymentAmount > userProposedAmount)
    {
      return (fixedPaymentAmount, false);
    }
    return (userProposedAmount, true);
  }
}
