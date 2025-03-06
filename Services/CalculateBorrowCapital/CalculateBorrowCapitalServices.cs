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

    decimal total = decimal.Round(loanAmount * (1m + _interestRateSetting.InterestRate / 100));
    return (total,_interestRateSetting.InterestRate);
  }
  public (decimal, bool) MonthlyPaymentAmount(decimal salary, decimal percentageOfSalary, decimal totalPaymentAmount,
    int NumberOfPayments)
  {
    decimal fixedPaymentAmount =
      _monthlyPaymentAmount.CalculateMonthlyPaymentAmount(totalPaymentAmount, NumberOfPayments);
    decimal userProposedAmount = decimal.Round( salary * (percentageOfSalary / 100),1);
    if (fixedPaymentAmount > userProposedAmount)
    {
      return (fixedPaymentAmount, false);
    }
    return (userProposedAmount, true);
  }
}
