using System.Linq.Expressions;
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
  public async Task<IActionResult> GetAllLoanPayments(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? search = null,
    [FromQuery] string? orderColumn = null,
    [FromQuery] string? orderDir = null)
{
    try
    {
        var query = _context.LoanPayments.Include(lp => lp.BorrowCapital).ThenInclude(b => b.User).AsQueryable();

        // Handle search query
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(x =>
                (x.PaymentAmount.ToString() != null && EF.Functions.Like(x.PaymentAmount.ToString(), $"%{search}%")) || // Search PaymentAmount
                (x.OverdueDays.ToString() != null && EF.Functions.Like(x.OverdueDays.ToString(), $"%{search}%")) || // Search OverdueDays
                (x.PenaltyInterest.ToString() != null && EF.Functions.Like(x.PenaltyInterest.ToString(), $"%{search}%")) || // Search PenaltyInterest
                (x.Status.ToString() != null && EF.Functions.Like(x.Status.ToString(), $"%{search}%")) || // Search Status
                (x.BorrowCapital != null && EF.Functions.Like(x.BorrowCapital.ToString(), $"%{search}%")) // Search BorrowId
            );
        }

        // Handle sorting by dynamic column and direction
        if (!string.IsNullOrEmpty(orderColumn) && !string.IsNullOrEmpty(orderDir))
        {
            var parameter = Expression.Parameter(typeof(LoanPayment), "x");
            var property = Expression.Property(parameter, orderColumn);
            var lambda = Expression.Lambda<Func<LoanPayment, object>>(Expression.Convert(property, typeof(object)), parameter);

            if (orderDir.ToLower() == "asc")
            {
                query = query.OrderBy(lambda);
            }
            else if (orderDir.ToLower() == "desc")
            {
                query = query.OrderByDescending(lambda);
            }
        }

        // Get the total count for pagination
        var totalCount = await query.CountAsync();

        // Paginate the results
        var pagedLoanPayments = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        // Return the response in the desired format
        var result = new
        {
            data = pagedLoanPayments.Select(lp => new
            {
                lp.Id,
                lp.PaymentAmount,
                lp.PaymentDate,
                lp.OverdueDays,
                lp.PenaltyInterest,
            }).ToArray(),
            recordsTotal = await _context.LoanPayments.CountAsync(),
            recordsFiltered = totalCount,
            page = page,
            pageSize = pageSize
        };

        return Ok(result);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
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
  public async Task<IActionResult> GetLoanPaymentsByBorrowId(int borrowId)
  {
    try
    {
      var borrowCapital = await _context.LoanPayments.Where(b => b.BorrowId == borrowId).ToListAsync();
      return Ok(borrowCapital);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
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
      return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
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
      return Ok(new {PaymentAmount = result.Item1 , PenaltyPercentage = result.Item2 , OverdueDays = CalculateOverdueDays(request.NowDateTime, request.LoanDateTime)});
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { Message = "An error occurred", Error = ex.Message });
    }
  }
  private int CalculateOverdueDays(DateTime nowDateTime, DateTime loanDateTime)
  {
    var now = nowDateTime;
    var loanDate = loanDateTime;
    var daysDiff = (now - loanDate).Days;
    return Math.Max(0, daysDiff - 30); // Giả sử kỳ hạn 30 ngày
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
