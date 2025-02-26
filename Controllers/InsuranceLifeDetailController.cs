using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Sem3.Data;
using Project_Sem3.Models;

namespace Project_Sem3.Controllers;

[ApiController]
[Route("api/insurance-life")]
public class InsuranceLifeDetailController : ControllerBase
{
    private readonly MyDbContext _context;

    public InsuranceLifeDetailController(MyDbContext context)
    {
        _context = context;
    }

    // 1. Create - Tạo một InsuranceLifeDetail mới
    [HttpPost]
    public async Task<IActionResult> CreateInsuranceLifeDetail([FromBody] InsuranceLifeDetail detail)
    {
        try
        {
            // Kiểm tra dữ liệu đầu vào
            if (detail == null || detail.PlanId <= 0 || detail.AnnualPaymentAmount <= 0 ||
                detail.Premium <= 0 || detail.CoverageAmount <= 0 || detail.Deductible < 0)
            {
                return BadRequest(new { Message = "Invalid insurance life detail data" });
            }

            detail.CreatedAt = DateTime.UtcNow;

            _context.InsuranceLifeDetails.Add(detail);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 2. Read - Lấy tất cả InsuranceLifeDetails với phân trang, chỉ lấy record chưa xóa mềm
    [HttpGet]
    public async Task<IActionResult> GetAllInsuranceLifeDetails(int page = 1, int pageSize = 10)
    {
        try
        {
            var totalRecords = await _context.InsuranceLifeDetails
                .Where(d => d.DeleteAt == null)
                .CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var details = await _context.InsuranceLifeDetails
                .Where(d => d.DeleteAt == null)
                .Select(d => new
                {
                    d.Id,
                    d.PlanId,
                    d.AnnualPaymentAmount,
                    d.Premium,
                    d.CoverageAmount,
                    d.Deductible,
                    d.TermYears,
                    d.AgeGroup,
                    d.Beneficiaries,
                    d.Duration,
                    d.RiskFactor,
                    d.Region,
                    CreatedAt = d.CreatedAt.HasValue ? d.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                    UpdatedAt = d.UpdatedAt.HasValue ? d.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
                })
                .OrderBy(d => d.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Data = details,
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

    // 2.1 Read - Lấy InsuranceLifeDetail theo Id, chỉ lấy record chưa xóa mềm
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInsuranceLifeDetailById(int id)
    {
        try
        {
            var detail = await _context.InsuranceLifeDetails
                .Where(d => d.Id == id && d.DeleteAt == null)
                .Select(d => new
                {
                    d.Id,
                    d.PlanId,
                    d.AnnualPaymentAmount,
                    d.Premium,
                    d.CoverageAmount,
                    d.Deductible,
                    d.TermYears,
                    d.AgeGroup,
                    d.Beneficiaries,
                    d.Duration,
                    d.RiskFactor,
                    d.Region,
                    CreatedAt = d.CreatedAt.HasValue ? d.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                    UpdatedAt = d.UpdatedAt.HasValue ? d.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
                })
                .FirstOrDefaultAsync();

            if (detail == null)
            {
                return NotFound(new { Message = "Insurance life detail not found" });
            }

            return Ok(detail);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 3. Update - Cập nhật thông tin InsuranceLifeDetail, chỉ cập nhật record chưa xóa mềm
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInsuranceLifeDetail(int id, [FromBody] InsuranceLifeDetail detail)
    {
        try
        {
            var existingDetail = await _context.InsuranceLifeDetails
                .Where(d => d.Id == id && d.DeleteAt == null)
                .FirstOrDefaultAsync();
            if (existingDetail == null)
            {
                return NotFound(new { Message = "Insurance life detail not found" });
            }

            existingDetail.PlanId = detail.PlanId;
            existingDetail.AnnualPaymentAmount = detail.AnnualPaymentAmount;
            existingDetail.Premium = detail.Premium;
            existingDetail.CoverageAmount = detail.CoverageAmount;
            existingDetail.Deductible = detail.Deductible;
            existingDetail.TermYears = detail.TermYears;
            existingDetail.AgeGroup = detail.AgeGroup;
            existingDetail.Beneficiaries = detail.Beneficiaries;
            existingDetail.Duration = detail.Duration;
            existingDetail.RiskFactor = detail.RiskFactor;
            existingDetail.Region = detail.Region;
            existingDetail.UpdatedAt = DateTime.UtcNow;

            _context.InsuranceLifeDetails.Update(existingDetail);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 4. Delete - Xóa mềm InsuranceLifeDetail
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeleteInsuranceLifeDetail(int id)
    {
        try
        {
            var detail = await _context.InsuranceLifeDetails
                .Where(d => d.Id == id && d.DeleteAt == null)
                .FirstOrDefaultAsync();
            if (detail == null)
            {
                return NotFound(new { Message = "Insurance life detail not found" });
            }

            detail.DeleteAt = DateTime.UtcNow;
            _context.InsuranceLifeDetails.Update(detail);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 5. Delete - Xóa cứng InsuranceLifeDetail
    [HttpDelete("{id}/hard")]
    public async Task<IActionResult> HardDeleteInsuranceLifeDetail(int id)
    {
        try
        {
            var detail = await _context.InsuranceLifeDetails.FirstOrDefaultAsync(d => d.Id == id);
            if (detail == null)
            {
                return NotFound(new { Message = "Insurance life detail not found" });
            }

            _context.InsuranceLifeDetails.Remove(detail);
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
