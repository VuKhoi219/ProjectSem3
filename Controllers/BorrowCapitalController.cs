using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Project_Sem3.Data;
using Project_Sem3.Models;
using Project_Sem3.Models.Request;
using Project_Sem3.Services.CalculateBorrowCapital;

namespace Project_Sem3.Controllers;

[ApiController]
[Route("api/borrow-capital")]
public class BorrowCapitalController : Controller
{
  public readonly MyDbContext _context;
  public readonly CalculateBorrowCapitalServices _calculateBorrowCapitalServices;

  public BorrowCapitalController(MyDbContext context , CalculateBorrowCapitalServices calculateBorrowCapitalServices)
  {
    _context = context;
    _calculateBorrowCapitalServices = calculateBorrowCapitalServices;
  }
// crud vay vốn
  [HttpGet]
  public async Task<IActionResult> GetBorrowCapitals()
  {
    try
    {
      var listBorrowCapitals = await _context.BorrowCapitals.Select(s => new
      {
        s.CreatedAt,
        s.Id,
        s.Status
      }).ToListAsync();
      return Ok(listBorrowCapitals);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }
  }
  [HttpGet("{id}")]
  public async Task<IActionResult> GetBorrowCapitalById(int id)
  {
    try
    {
      var borrowCapital = await _context.BorrowCapitals.Where(b => b.Id == id).FirstAsync();
      return Ok(borrowCapital);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }
  }

  [HttpGet("user/{userId}")]
  public async Task<IActionResult> GetBorrowCapitalsByUserId(int userId)
  {
    try
    {
      var borrowCapital = await _context.BorrowCapitals.Where(b => b.UserId == userId).ToListAsync();
      return Ok(borrowCapital);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }
  }

  [HttpPost]
  public async Task<IActionResult> CreateBorrowCapital([FromBody] BorrowCapital rq)
  {
    try
    {
      rq.Status = StatusBorrowCapital.Pending;
      _context.BorrowCapitals.Add(rq);
      await _context.SaveChangesAsync();
      return Ok(true);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }
  }

  [HttpPut("{id}/{newStatusBorrowCapital}")]
  public async Task<IActionResult> EditStatusBorrowCapital(int id , string newStatusBorrowCapital)
  {
    try
    {
      var borrowCapital = await _context.BorrowCapitals.Where(b => b.Id == id).FirstAsync();
      if (borrowCapital == null) return Ok(false);

      switch (newStatusBorrowCapital)
      {
        case "Pending":
          borrowCapital.Status = StatusBorrowCapital.Pending;
          break;
        case "Active":
          borrowCapital.Status = StatusBorrowCapital.Active;
          break;
        case "Overdue" :
          borrowCapital.Status = StatusBorrowCapital.Overdue;
          break;
        case "Closed" :
          borrowCapital.Status = StatusBorrowCapital.Closed;
          break;
        default:
          return Ok(false);
      }
      borrowCapital.UpdatedAt = DateTime.Now;
      _context.BorrowCapitals.Update(borrowCapital);
      await _context.SaveChangesAsync();
      return Ok(true);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }
  }

  [HttpPost("calculate")]
  public IActionResult CalculateBorrowCapital([FromBody] BorrowCapitalRequest request)
  {
    try
    {
      var totalPaymentAmount = _calculateBorrowCapitalServices.totalPaymentAmount(request.LoanAmount);
      var monthlyPaymentAmount = _calculateBorrowCapitalServices.MonthlyPaymentAmount(request.Salaly, request.PercentageSalary,
        totalPaymentAmount.Item1, request.NumberOfPayments);
      var dueDate = request.LoanDate.AddMonths(request.NumberOfPayments);
      return Ok(new
      {
        TotalAmount = totalPaymentAmount.Item1,
        TotalInterest = totalPaymentAmount.Item2,
        MonthlyAmount = monthlyPaymentAmount.Item1,
        BoolMonthlyAmount = monthlyPaymentAmount.Item2,
        DueDate = dueDate
      });

    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }
  }

  [HttpPost("by-date")]
  public async Task<IActionResult> GetBorrowCapitalsByDateRange([FromBody] BorrowCapitalDateRangeRequest request)
  {
    try
    {
      if (request == null)
      {
        return BadRequest(new { Message = "Request body is null" });
      }

      var query = _context.BorrowCapitals.AsQueryable();

      if (request.StartDate.HasValue)
        query = query.Where(b => b.LoanDate >= request.StartDate.Value);
      if (request.EndDate.HasValue)
        query = query.Where(b => b.LoanDate <= request.EndDate.Value);
      if (request.UserId.HasValue)
        query = query.Where(b => b.UserId == request.UserId.Value);
      if (request.Status.HasValue)
        query = query.Where(b => b.Status == request.Status.Value);

      var result = await query.Select(b => new
      {
        b.Id,
        b.UserId,
        b.LoanAmount,
        b.LoanDate,
        b.DueDate,
        b.Status,
        b.CreatedAt
      }).ToListAsync();

      if (!result.Any())
      {
        return Ok(new { Message = "No borrow capitals found for the specified criteria", Data = result });
      }

      return Ok(result);
    }
    catch (Exception e)
    {
      return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
    }
  }
}
