using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Sem3.Areas.Admin.Controllers;
using Project_Sem3.Data; // Giả sử đây là namespace chứa DbContext
using Project_Sem3.Models; // Giả sử đây là namespace chứa Payment và PaymentStatus

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
            // Kiểm tra dữ liệu đầu vào
            if (payment == null || payment.UserId <= 0 || payment.ContractId <= 0 || payment.Amount <= 0)
            {
                return BadRequest(new { Message = "Invalid payment data" });
            }

            // Gán thời gian tạo
            payment.CreatedAt = DateTime.UtcNow;
            payment.Status = PaymentStatus.Pending; // Mặc định là Pending khi tạo mới

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Payment created successfully", PaymentId = payment.Id });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 2. Read - Lấy tất cả Payments
    [HttpGet]
    public async Task<IActionResult> GetAllPayments()
    {
        try
        {
            var payments = await _context.Payments
                .Select(p => new
                {
                    p.Id,
                    p.UserId,
                    p.ContractId,
                    p.Amount,
                    p.PaymentDate,
                    p.Status,
                    p.ImageUrl,
                })
                .ToListAsync();

            return Ok(payments);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 2.1 Read - Lấy Payment theo Id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        try
        {
            var payment = await _context.Payments
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.UserId,
                    p.ContractId,
                    p.Amount,
                    p.PaymentDate,
                    p.Status,
                    p.ImageUrl,
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
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 2.2 Read - Lấy Payments theo UserId
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetPaymentsByUserId(int userId)
    {
        try
        {
            var payments = await _context.Payments
                .Where(p => p.UserId == userId)
                .Select(p => new
                {
                    p.Id,
                    p.UserId,
                    p.ContractId,
                    p.Amount,
                    p.PaymentDate,
                    p.Status,
                    p.ImageUrl
                })
                .ToListAsync();

            return Ok(payments);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 3. Update - Cập nhật thông tin Payment
    [HttpPut("{id}/{newStatus}")]
    public async Task<IActionResult> UpdatePayment(int id,string newStatus)
    {
        try
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);
            if (payment == null)
            {
                return NotFound(new { Message = "Payment not found" });
            }

            switch (newStatus)
            {
              case "Pending":
                payment.Status = PaymentStatus.Pending;
                break;
              case "Completed":
                payment.Status = PaymentStatus.Completed;
                break;
              case "Failed":
                payment.Status = PaymentStatus.Failed;
                break;
              default:
                return Ok(null);
                break;
            }
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Payment updated successfully" });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

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

            return Ok(new { Message = "Payment permanently deleted" });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }
}
