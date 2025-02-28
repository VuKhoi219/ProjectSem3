using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Sem3.Areas.Admin.Controllers;
using Project_Sem3.Data; // Giả sử đây là namespace chứa DbContext
using Project_Sem3.Models; // Giả sử đây là namespace chứa Payment và PaymentStatus
using System.Linq.Dynamic.Core;


namespace Project_Sem3.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly MyDbContext _context;

    public PaymentController(MyDbContext context)
    {
        _context = context;
    }

    // 1. Create - Tạo một Payment mới
    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] Payment payment)
    {
        try
        {
            if (payment == null || payment.UserId <= 0 || payment.ContractId <= 0 || payment.Amount <= 0)
            {
                return BadRequest(new { Message = "Invalid payment data" });
            }

            payment.CreatedAt = DateTime.UtcNow;
            payment.Status = PaymentStatus.Pending;

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 2. Read - Lấy tất cả Payments với phân trang, chỉ lấy record chưa xóa mềm
    [HttpGet]
    public async Task<IActionResult> GetAllPayments( [FromQuery] int page = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string? search = null,
      [FromQuery] string? orderColumn = null,
      [FromQuery] string? orderDir = null)
    {
      try
      {
        // var totalRecords = await _context.Payments
        //   .Where(p => p.DeleteAt == null)
        //   .CountAsync();
        // var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        //
        // var payments = await _context.Payments
        //   .Where(p => p.DeleteAt == null)
        //   .Select(p => new
        //   {
        //     p.Id,
        //     p.UserId,
        //     p.ContractId,
        //     p.Amount,
        //     PaymentDate = p.PaymentDate.ToString("yyyy-MM-dd HH:mm:ss"),
        //     p.Status,
        //     p.ImageUrl,
        //     CreatedAt = p.CreatedAt.HasValue ? p.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
        //     UpdatedAt = p.UpdatedAt.HasValue ? p.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
        //   })
        //   .OrderBy(p => p.Id)
        //   .Skip((page - 1) * pageSize)
        //   .Take(pageSize)
        //   .ToListAsync();
        //
        // return Ok(new
        // {
        //   data = payments, // Chú ý thay đổi từ Data sang data
        //   recordsTotal = totalRecords, // Tổng số bản ghi
        //   recordsFiltered = totalRecords, // Số bản ghi sau khi lọc
        //   draw = Request.Query["draw"].ToString() // Trả về giá trị draw (trường bắt buộc của DataTable)
        // });
        var query = _context.Payments.AsQueryable(); // móc từ db.
        if (!string.IsNullOrEmpty(search))
        {
          // search theo keyword
          query = query.Where(x => x.User.Email.Contains(search, StringComparison.OrdinalIgnoreCase));
        }
        if (!string.IsNullOrEmpty(orderColumn) && !string.IsNullOrEmpty(orderDir))
        {
          // sort by field
          query = query.OrderBy($"{orderColumn} {orderDir}");
        }
        var totalCount = query.Count();
        var pagedProducts = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var result = new
        {
          data = pagedProducts.ToArray(),
          recordsTotal = _context.Payments.Count(),
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


    // 2.1 Read - Lấy Payment theo Id, chỉ lấy record chưa xóa mềm
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        try
        {
            var payment = await _context.Payments
                .Where(p => p.Id == id && p.DeleteAt == null)
                .Select(p => new
                {
                    p.Id,
                    p.UserId,
                    p.ContractId,
                    p.Amount,
                    PaymentDate = p.PaymentDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    p.Status,
                    p.ImageUrl,
                    CreatedAt = p.CreatedAt.HasValue ? p.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                    UpdatedAt = p.UpdatedAt.HasValue ? p.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
                })
                .FirstOrDefaultAsync();

            if (payment == null)
            {
                return NotFound(new { Message = "Payment not found" });
            }

            return Ok(payment);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 2.2 Read - Lấy Payments theo UserId, chỉ lấy record chưa xóa mềm
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetPaymentsByUserId(int userId , int page = 1, int pageSize = 10)
    {
        try
        {
          var totalRecords = await _context.Payments
            .Where(p => p.DeleteAt == null)
            .CountAsync();
          var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            var payments = await _context.Payments
                .Where(p => p.UserId == userId && p.DeleteAt == null)
                .Select(p => new
                {
                    p.Id,
                    p.UserId,
                    p.ContractId,
                    p.Amount,
                    PaymentDate = p.PaymentDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    p.Status,
                    p.ImageUrl,
                    CreatedAt = p.CreatedAt.HasValue ? p.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                    UpdatedAt = p.UpdatedAt.HasValue ? p.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
                })                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
              Data = payments,
              TotalRecords = totalRecords,
              TotalPages = totalPages,
              CurrentPage = page,
              PageSize = pageSize
            });        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 3. Update Status - Cập nhật trạng thái Payment, chỉ xử lý record chưa xóa mềm
    [HttpPut("{id}/{newStatus}")]
    public async Task<IActionResult> UpdatePaymentStatus(int id, string newStatus)
    {
        try
        {
            var payment = await _context.Payments
                .Where(p => p.Id == id && p.DeleteAt == null)
                .FirstOrDefaultAsync();
            if (payment == null)
            {
                return Ok(false);
            }

            switch (newStatus.ToLower())
            {
                case "pending":
                    payment.Status = PaymentStatus.Pending;
                    break;
                case "completed":
                    payment.Status = PaymentStatus.Completed;
                    break;
                case "failed":
                    payment.Status = PaymentStatus.Failed;
                    break;
                default:
                    return Ok(false);
            }

            payment.UpdatedAt = DateTime.UtcNow;
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 3.1 Update - Cập nhật toàn bộ thông tin Payment, chỉ xử lý record chưa xóa mềm
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePayment(int id, [FromBody] Payment payment)
    {
        try
        {
            var existingPayment = await _context.Payments
                .Where(p => p.Id == id && p.DeleteAt == null)
                .FirstOrDefaultAsync();
            if (existingPayment == null)
            {
                return NotFound(new { Message = "Payment not found" });
            }

            existingPayment.UserId = payment.UserId;
            existingPayment.ContractId = payment.ContractId;
            existingPayment.Amount = payment.Amount;
            existingPayment.PaymentDate = payment.PaymentDate;
            existingPayment.Status = payment.Status;
            existingPayment.ImageUrl = payment.ImageUrl;
            existingPayment.UpdatedAt = DateTime.UtcNow;

            _context.Payments.Update(existingPayment);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 4. Delete - Xóa mềm Payment
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeletePayment(int id)
    {
        try
        {
            var payment = await _context.Payments
                .Where(p => p.Id == id && p.DeleteAt == null)
                .FirstOrDefaultAsync();
            if (payment == null)
            {
                return NotFound(new { Message = "Payment not found" });
            }

            payment.DeleteAt = DateTime.UtcNow;
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 5. Delete - Xóa cứng Payment
    [HttpDelete("{id}/hard")]
    public async Task<IActionResult> HardDeletePayment(int id)
    {
        try
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);
            if (payment == null)
            {
                return NotFound(new { Message = "Payment not found" });
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }
}
