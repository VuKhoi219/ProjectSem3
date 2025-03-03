using System.Linq.Expressions;
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
                detail.Premium <= 0|| detail.Deductible < 0)
            {
                return BadRequest(new { Message = "Invalid insurance life detail data" });
            }

            detail.CreatedAt = DateTime.UtcNow;

            _context.InsuranceLifeDetails.Add(detail);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, DetailId = detail.Id });        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 2. Read - Lấy tất cả InsuranceLifeDetails với phân trang, chỉ lấy record chưa xóa mềm
    [HttpGet]
    public async Task<IActionResult> GetAllInsuranceLifeDetails(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? search = null,
    [FromQuery] string? orderColumn = null,
    [FromQuery] string? orderDir = null)
{
    try
    {
        var query = _context.InsuranceLifeDetails.Include(il => il.Plan).AsQueryable();

        // Handle search query
        if (!string.IsNullOrEmpty(search))
        {
          query = query.Where(x =>
            (x.AgeGroup != null && EF.Functions.Like(x.AgeGroup, $"%{search}%")) || // Search AgeGroup
            (x.Beneficiaries != null && EF.Functions.Like(x.Beneficiaries, $"%{search}%")) || // Search Beneficiaries
            (x.Duration.ToString() != null &&
             EF.Functions.Like(x.Duration.ToString(), $"%{search}%")) || // Search Duration
            (x.RiskFactor.ToString() != null &&
             EF.Functions.Like(x.RiskFactor.ToString(), $"%{search}%")) || // Search RiskFactor
            (x.Region != null && EF.Functions.Like(x.Region, $"%{search}%"))); // Search Region
        }

        // Handle sorting by dynamic column and direction
        if (!string.IsNullOrEmpty(orderColumn) && !string.IsNullOrEmpty(orderDir))
        {
            var parameter = Expression.Parameter(typeof(InsuranceLifeDetail), "x");
            var property = Expression.Property(parameter, orderColumn);
            var lambda = Expression.Lambda<Func<InsuranceLifeDetail, object>>(Expression.Convert(property, typeof(object)), parameter);

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
        var pagedInsuranceLifeDetails = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        // Return the response in the desired format
        var result = new
        {
            data = pagedInsuranceLifeDetails.Select(il => new
            {
                il.Id,
                il.AgeGroup,
                il.Beneficiaries,
                il.Duration,
                il.RiskFactor,
                il.Region,
            }).ToArray(),
            recordsTotal = await _context.InsuranceLifeDetails.CountAsync(),
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
            // existingDetail.Premium = detail.Premium;
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
