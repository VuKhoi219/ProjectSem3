using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Sem3.Data;
using Project_Sem3.Models;

namespace Project_Sem3.Controllers;

[ApiController]
[Route("api/insurance-health")]
public class InsuranceHealthDetailController : ControllerBase
{
    private readonly MyDbContext _context;

    public InsuranceHealthDetailController(MyDbContext context)
    {
        _context = context;
    }

    // 1. Create - Tạo một InsuranceHealthDetail mới
    [HttpPost]
    public async Task<IActionResult> CreateInsuranceHealthDetail([FromBody] InsuranceHealthDetail detail)
    {
        try
        {
            // Kiểm tra dữ liệu đầu vào
            if (detail == null || detail.PlanId <= 0 || detail.AnnualPaymentAmount <= 0 ||
                detail.Premium <= 0 || detail.Deductible < 0)
            {
                return BadRequest(new { Message = "Invalid insurance health detail data" });
            }

            detail.CreatedAt = DateTime.UtcNow;

            _context.InsuranceHealthDetails.Add(detail);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 2. Read - Lấy tất cả InsuranceHealthDetails với phân trang, chỉ lấy record chưa xóa mềm
    [HttpGet]
    public async Task<IActionResult> GetAllInsuranceHealthDetails(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? search = null,
    [FromQuery] string? orderColumn = null,
    [FromQuery] string? orderDir = null)
{
    try
    {
        var query = _context.InsuranceHealthDetails.Include(ih => ih.Plan).AsQueryable();

        // Handle search query
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(x =>
                (x.AgeGroup != null && EF.Functions.Like(x.AgeGroup, $"%{search}%")) || // Search AgeGroup
                (x.HospitalNetwork != null && EF.Functions.Like(x.HospitalNetwork, $"%{search}%")) || // Search HospitalNetwork
                (x.RiskFactor.ToString() != null && EF.Functions.Like(x.RiskFactor.ToString(), $"%{search}%")) || // Search RiskFactor
                (x.Region != null && EF.Functions.Like(x.Region, $"%{search}%")) // Search Region
            );
        }

        // Handle sorting by dynamic column and direction
        if (!string.IsNullOrEmpty(orderColumn) && !string.IsNullOrEmpty(orderDir))
        {
            var parameter = Expression.Parameter(typeof(InsuranceHealthDetail), "x");
            var property = Expression.Property(parameter, orderColumn);
            var lambda = Expression.Lambda<Func<InsuranceHealthDetail, object>>(Expression.Convert(property, typeof(object)), parameter);

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
        var pagedInsuranceHealthDetails = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        // Return the response in the desired format
        var result = new
        {
            data = pagedInsuranceHealthDetails.Select(ih => new
            {
                ih.Id,
                ih.AgeGroup,
                ih.HospitalNetwork,
                ih.Duration,
                ih.RiskFactor,
                ih.Region,
            }).ToArray(),
            recordsTotal = await _context.InsuranceHealthDetails.CountAsync(),
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


    // 2.1 Read - Lấy InsuranceHealthDetail theo Id, chỉ lấy record chưa xóa mềm
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInsuranceHealthDetailById(int id)
    {
        try
        {
            var detail = await _context.InsuranceHealthDetails
                .Where(d => d.Id == id && d.DeleteAt == null)
                .Select(d => new
                {
                    d.Id,
                    d.PlanId,
                    d.AnnualPaymentAmount,
                    d.Premium,
                    d.Deductible,
                    d.AgeGroup,
                    d.HospitalNetwork,
                    d.PreExistingConditions,
                    d.Duration,
                    d.RiskFactor,
                    d.Region,
                    CreatedAt = d.CreatedAt.HasValue ? d.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                    UpdatedAt = d.UpdatedAt.HasValue ? d.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
                })
                .FirstOrDefaultAsync();

            if (detail == null)
            {
                return NotFound(new { Message = "Insurance health detail not found" });
            }

            return Ok(detail);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 3. Update - Cập nhật thông tin InsuranceHealthDetail, chỉ cập nhật record chưa xóa mềm
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInsuranceHealthDetail(int id, [FromBody] InsuranceHealthDetail detail)
    {
        try
        {
            var existingDetail = await _context.InsuranceHealthDetails
                .Where(d => d.Id == id && d.DeleteAt == null)
                .FirstOrDefaultAsync();
            if (existingDetail == null)
            {
                return NotFound(new { Message = "Insurance health detail not found" });
            }

            existingDetail.PlanId = detail.PlanId;
            existingDetail.AnnualPaymentAmount = detail.AnnualPaymentAmount;
            existingDetail.Premium = detail.Premium;
            existingDetail.Deductible = detail.Deductible;
            existingDetail.AgeGroup = detail.AgeGroup;
            existingDetail.HospitalNetwork = detail.HospitalNetwork;
            existingDetail.PreExistingConditions = detail.PreExistingConditions;
            existingDetail.Duration = detail.Duration;
            existingDetail.RiskFactor = detail.RiskFactor;
            existingDetail.Region = detail.Region;
            existingDetail.UpdatedAt = DateTime.UtcNow;

            _context.InsuranceHealthDetails.Update(existingDetail);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 4. Delete - Xóa mềm InsuranceHealthDetail
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeleteInsuranceHealthDetail(int id)
    {
        try
        {
            var detail = await _context.InsuranceHealthDetails
                .Where(d => d.Id == id && d.DeleteAt == null)
                .FirstOrDefaultAsync();
            if (detail == null)
            {
                return NotFound(new { Message = "Insurance health detail not found" });
            }

            detail.DeleteAt = DateTime.UtcNow;
            _context.InsuranceHealthDetails.Update(detail);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 5. Delete - Xóa cứng InsuranceHealthDetail
    [HttpDelete("{id}/hard")]
    public async Task<IActionResult> HardDeleteInsuranceHealthDetail(int id)
    {
        try
        {
            var detail = await _context.InsuranceHealthDetails.FirstOrDefaultAsync(d => d.Id == id);
            if (detail == null)
            {
                return NotFound(new { Message = "Insurance health detail not found" });
            }

            _context.InsuranceHealthDetails.Remove(detail);
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
