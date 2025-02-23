using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Sem3.Data;
using Project_Sem3.Models;
using Project_Sem3.Models.Request;
using Project_Sem3.Services.LoanPaymentServices;

namespace Project_Sem3.Controllers;

[ApiController]
[Route("api/loan-payment")]
public class LoanPaymentController : Controller
{
  public readonly MyDbContext _context;
  public readonly LoanPaymentServices _loanPaymentServices;

  public LoanPaymentController(MyDbContext myDbContext , LoanPaymentServices loanPaymentServices)
  {
    _context = myDbContext;
    _loanPaymentServices = loanPaymentServices;
  }

  [HttpGet]
  public async Task<IActionResult> GetLoanPayments()
  {
    try
    {
      var result = await _context.LoanPayments.Select(lp => new
      {
        lp.Id,
        lp.BorrowId,
        lp.PaymentAmount,
        lp.PaymentDate,
      }).ToListAsync();
      return Ok(result);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetLoanPaymentById(int id)
  {
    try
    {
      var borrowCapital = await _context.LoanPayments.Where(b => b.Id == id).FirstAsync();
      return Ok(borrowCapital);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }
  }

  [HttpGet("borrow/{borrowId}")]
  public async Task<IActionResult> GetLoanPaymentsByUserId(int borrowId)
  {
    try
    {
      var borrowCapital = await _context.LoanPayments.Where(b => b.BorrowId == borrowId).ToListAsync();
      return Ok(borrowCapital);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }
  }

  [HttpPost]
  public async Task<IActionResult> CreateLoanPayment([FromBody] LoanPayment rq)
  {
    try
    {
      var borrowExists = await _context.BorrowCapitals.AnyAsync(b => b.Id == rq.BorrowId);
      if (!borrowExists)
      {
        return BadRequest(new { Message = "Invalid BorrowId" });
      }
      rq.Status = false;
      _context.LoanPayments.Add(rq);
      await _context.SaveChangesAsync();
      return Ok(true);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }
  }

  [HttpPut("{id}/{newStatus}")]
  public async Task<IActionResult> EditStatusLoanPayment(int id , string newStatus)
  {
    try
    {
      var loanPayment = await _context.LoanPayments.Where(lp => lp.Id == id).FirstAsync();
      if (loanPayment == null) return Ok(false);
      switch (newStatus)
      {
        case "false":
          loanPayment.Status = false;
          break;
        // thánh toán thành công
        case "true":
          loanPayment.Status = true;
          break;
        default:
          return Ok(false);
      }
      _context.LoanPayments.Update(loanPayment);
      await _context.SaveChangesAsync();
      return Ok(true);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }
  }

  [HttpPost("calculate-payment")]
  public IActionResult CalculatePayment([FromBody] LoanPaymentRequest request)
  {
    if (request == null)
    {
      return BadRequest(new { Message = "Request body is null" });
    }
    try
    {
      var result = _loanPaymentServices.CalculatePaymentAmount(
        request.NowDateTime,
        request.LoanDateTime,
        request.LoanAmount,
        request.MonthlyPaymentAmount);
      return Ok(new {PaymentAmount = result.Item1 , PenaltyPercentage = result.Item2});
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { Message = "An error occurred", Error = ex.Message });
    }
  }

  [HttpGet("total-loan-payment/{borrowId}")]
  public async Task<IActionResult> GetTotalPaymentByBorrowId(int borrowId)
  {
    try
    {
      var borrow = await _context.BorrowCapitals
        .Where(b => b.Id == borrowId)
        .Select(b => b.RepaymentAmount)
        .FirstOrDefaultAsync();

      var totalLoanPayment = await _context.LoanPayments
        .Where(lp => lp.BorrowId == borrowId && lp.Status == true)
        .SumAsync(lp => lp.PaymentAmount);
      var remainingAmount = borrow - totalLoanPayment;
      return Ok(new { BorrowId = borrowId, RepaymentAmount = borrow, TotalLoanPayment = totalLoanPayment , RemainingAmount = remainingAmount });
    }
    catch (Exception e)
    {
      return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
    }
  }

  [HttpPost("by-date")]
  public async Task<IActionResult> GetLoanPaymentsByDateRange([FromBody] LoanPaymentDateRangeRequest request)
  {
    try
    {
      if (request == null)
      {
        return BadRequest(new { Message = "Request body is null" });
      }

      var query = _context.LoanPayments.AsQueryable();

      if (request.StartDate.HasValue)
        query = query.Where(lp => lp.PaymentDate >= request.StartDate.Value);
      if (request.EndDate.HasValue)
        query = query.Where(lp => lp.PaymentDate <= request.EndDate.Value);
      if (request.BorrowId.HasValue)
        query = query.Where(lp => lp.BorrowId == request.BorrowId.Value);
      if (request.Status.HasValue)
        query = query.Where(lp => lp.Status == request.Status.Value);

      var result = await query.Select(lp => new
      {
        lp.Id,
        lp.BorrowId,
        lp.PaymentAmount,
        lp.PaymentDate,
        lp.Status
      }).ToListAsync();

      if (!result.Any())
      {
        return Ok(new { Message = "No loan payments found for the specified criteria", Data = result });
      }

      return Ok(result);
    }
    catch (Exception e)
    {
      return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
    }
  }
}
