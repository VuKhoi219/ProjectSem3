using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Sem3.Data;
using Project_Sem3.Models;
using Project_Sem3.Models.Request;
using Project_Sem3.Services.CalculateBorrowCapital;

namespace Project_Sem3.Controllers;

[ApiController]
[Route("api/borrow-capital")]
public class BorrowCapitalController : ControllerBase
{
    private readonly MyDbContext _context;
    private readonly CalculateBorrowCapitalServices _calculateBorrowCapitalServices;

    public BorrowCapitalController(MyDbContext context, CalculateBorrowCapitalServices calculateBorrowCapitalServices)
    {
        _context = context;
        _calculateBorrowCapitalServices = calculateBorrowCapitalServices;
    }

    // 1. Get All - Lấy tất cả BorrowCapitals với phân trang, chỉ lấy record chưa xóa mềm
    [HttpGet]
    public async Task<IActionResult> GetBorrowCapitals(int page = 1, int pageSize = 10)
    {
        try
        {
            var totalRecords = await _context.BorrowCapitals
                .Where(b => b.DeleteAt == null)
                .CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var listBorrowCapitals = await _context.BorrowCapitals
                .Where(b => b.DeleteAt == null)
                .Select(b => new
                {
                    b.Id,
                    b.UserId,
                    b.LoanAmount,
                    b.LoanDate,
                    b.DueDate,
                    b.Status,
                    CreatedAt = b.CreatedAt.HasValue ? b.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                    UpdatedAt = b.UpdatedAt.HasValue ? b.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
                })
                .OrderBy(b => b.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Data = listBorrowCapitals,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 2. Get By Id - Lấy BorrowCapital theo Id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBorrowCapitalById(int id)
    {
        try
        {
            var borrowCapital = await _context.BorrowCapitals
                .Where(b => b.Id == id && b.DeleteAt == null)
                .Select(b => new
                {
                    b.Id,
                    b.UserId,
                    b.LoanAmount,
                    b.LoanDate,
                    b.DueDate,
                    b.Status,
                    CreatedAt = b.CreatedAt.HasValue ? b.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                    UpdatedAt = b.UpdatedAt.HasValue ? b.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
                })
                .FirstOrDefaultAsync();

            if (borrowCapital == null)
            {
                return NotFound(new { Message = "Borrow capital not found" });
            }

            return Ok(borrowCapital);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 3. Get By UserId - Lấy BorrowCapitals theo UserId, chỉ lấy record chưa xóa mềm
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetBorrowCapitalsByUserId(int userId)
    {
        try
        {
            var borrowCapitals = await _context.BorrowCapitals
                .Where(b => b.UserId == userId && b.DeleteAt == null)
                .Select(b => new
                {
                    b.Id,
                    b.UserId,
                    b.LoanAmount,
                    b.LoanDate,
                    b.DueDate,
                    b.Status,
                    CreatedAt = b.CreatedAt.HasValue ? b.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                    UpdatedAt = b.UpdatedAt.HasValue ? b.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
                })
                .ToListAsync();

            return Ok(borrowCapitals);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 4. Create - Tạo một BorrowCapital mới
    [HttpPost]
    public async Task<IActionResult> CreateBorrowCapital([FromBody] BorrowCapital rq)
    {
        try
        {
            if (rq == null || rq.UserId <= 0 || rq.LoanAmount <= 0 || rq.LoanDate == default || rq.DueDate == default)
            {
                return BadRequest(new { Message = "Invalid borrow capital data" });
            }
            var check = await _context.InsuranceContracts.Where(ict => ict.UserId == rq.UserId).FirstAsync();
            if (check == null)
            {
              return Ok(false);
            }
            rq.Status = StatusBorrowCapital.Pending;
            rq.CreatedAt = DateTime.UtcNow;

            _context.BorrowCapitals.Add(rq);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 5. Update Status - Chỉ cập nhật trạng thái
    [HttpPut("{id}/{newStatusBorrowCapital}")]
    public async Task<IActionResult> EditStatusBorrowCapital(int id, string newStatusBorrowCapital)
    {
        try
        {
            var borrowCapital = await _context.BorrowCapitals
                .Where(b => b.Id == id && b.DeleteAt == null)
                .FirstOrDefaultAsync();
            if (borrowCapital == null)
            {
                return Ok(false);
            }

            switch (newStatusBorrowCapital.ToLower())
            {
                case "pending":
                    borrowCapital.Status = StatusBorrowCapital.Pending;
                    break;
                case "active":
                    borrowCapital.Status = StatusBorrowCapital.Active;
                    break;
                case "overdue":
                    borrowCapital.Status = StatusBorrowCapital.Overdue;
                    break;
                case "closed":
                    borrowCapital.Status = StatusBorrowCapital.Closed;
                    break;
                default:
                    return Ok(false);
            }

            borrowCapital.UpdatedAt = DateTime.UtcNow;
            _context.BorrowCapitals.Update(borrowCapital);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 6. Update - Cập nhật toàn bộ thông tin BorrowCapital
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBorrowCapital(int id, [FromBody] BorrowCapital rq)
    {
        try
        {
            var borrowCapital = await _context.BorrowCapitals
                .Where(b => b.Id == id && b.DeleteAt == null)
                .FirstOrDefaultAsync();
            if (borrowCapital == null)
            {
                return NotFound(new { Message = "Borrow capital not found" });
            }

            borrowCapital.UserId = rq.UserId;
            borrowCapital.LoanAmount = rq.LoanAmount;
            borrowCapital.LoanDate = rq.LoanDate;
            borrowCapital.DueDate = rq.DueDate;
            borrowCapital.Status = rq.Status;
            borrowCapital.UpdatedAt = DateTime.UtcNow;

            _context.BorrowCapitals.Update(borrowCapital);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 7. Delete Soft - Xóa mềm BorrowCapital
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeleteBorrowCapital(int id)
    {
        try
        {
            var borrowCapital = await _context.BorrowCapitals
                .Where(b => b.Id == id && b.DeleteAt == null)
                .FirstOrDefaultAsync();
            if (borrowCapital == null)
            {
                return NotFound(new { Message = "Borrow capital not found" });
            }

            borrowCapital.DeleteAt = DateTime.UtcNow;
            _context.BorrowCapitals.Update(borrowCapital);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 8. Delete Hard - Xóa cứng BorrowCapital
    [HttpDelete("{id}/hard")]
    public async Task<IActionResult> HardDeleteBorrowCapital(int id)
    {
        try
        {
            var borrowCapital = await _context.BorrowCapitals.FirstOrDefaultAsync(b => b.Id == id);
            if (borrowCapital == null)
            {
                return NotFound(new { Message = "Borrow capital not found" });
            }

            _context.BorrowCapitals.Remove(borrowCapital);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 9. Calculate - Tính toán BorrowCapital
    [HttpPost("calculate")]
    public IActionResult CalculateBorrowCapital([FromBody] BorrowCapitalRequest request)
    {
        try
        {
            var totalPaymentAmount = _calculateBorrowCapitalServices.totalPaymentAmount(request.LoanAmount);
            var monthlyPaymentAmount = _calculateBorrowCapitalServices.MonthlyPaymentAmount(
                request.Salaly, request.PercentageSalary, totalPaymentAmount.Item1, request.NumberOfPayments);
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
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 10. Get By Date Range - Lấy BorrowCapitals theo khoảng thời gian, chỉ lấy record chưa xóa mềm
    [HttpPost("by-date")]
    public async Task<IActionResult> GetBorrowCapitalsByDateRange([FromBody] BorrowCapitalDateRangeRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest(new { Message = "Request body is null" });
            }

            var query = _context.BorrowCapitals.Where(b => b.DeleteAt == null);

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
                CreatedAt = b.CreatedAt.HasValue ? b.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
            }).ToListAsync();

            if (!result.Any())
            {
                return Ok(new { Message = "No borrow capitals found for the specified criteria", Data = result });
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }
}
