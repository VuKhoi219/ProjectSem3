using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Sem3.Data;
using Project_Sem3.Models;

namespace Project_Sem3.Controllers;

[ApiController]
[Route("api/insurance-vehicle")]
public class InsuranceVehicleDetailsController : ControllerBase
{
    private readonly MyDbContext _context;

    public InsuranceVehicleDetailsController(MyDbContext context)
    {
        _context = context;
    }

    // 1. Create - Tạo một InsuranceVehicleDetail mới
    [HttpPost]
    public async Task<IActionResult> CreateInsuranceVehicleDetail([FromBody] InsuranceVehicleDetail detail)
    {
        try
        {
            // Kiểm tra dữ liệu đầu vào
            if (detail == null || detail.PlanId <= 0 || detail.AnnualPaymentAmount <= 0 ||
                detail.Premium <= 0|| detail.Deductible < 0)
            {
                return BadRequest(new { Message = "Invalid insurance vehicle detail data" });
            }

            detail.CreatedAt = DateTime.UtcNow;

            _context.InsuranceVehicleDetails.Add(detail);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 2. Read - Lấy tất cả InsuranceVehicleDetails với phân trang, chỉ lấy record chưa xóa mềm
    [HttpGet]
    public async Task<IActionResult> GetAllInsuranceVehicleDetails(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? search = null,
    [FromQuery] string? orderColumn = null,
    [FromQuery] string? orderDir = null)
{
    try
    {
        var query = _context.InsuranceVehicleDetails.Include(ivd => ivd.Plan).AsQueryable();

        // Handle search query
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(x =>
                (x.VehicleType != null && EF.Functions.Like(x.VehicleType, $"%{search}%")) || // Search VehicleType
                (x.VehicleModel != null && EF.Functions.Like(x.VehicleModel, $"%{search}%")) || // Search VehicleModel
                (x.VehicleYear.ToString() != null && EF.Functions.Like(x.VehicleYear.ToString(), $"%{search}%")) || // Search VehicleYear
                (x.Region != null && EF.Functions.Like(x.Region, $"%{search}%")) || // Search Region
                (x.Plan != null && EF.Functions.Like(x.Plan.ToString(), $"%{search}%")) // Search Plan Name
            );
        }

        // Handle sorting by dynamic column and direction
        if (!string.IsNullOrEmpty(orderColumn) && !string.IsNullOrEmpty(orderDir))
        {
            var parameter = Expression.Parameter(typeof(InsuranceVehicleDetail), "x");
            var property = Expression.Property(parameter, orderColumn);
            var lambda = Expression.Lambda<Func<InsuranceVehicleDetail, object>>(Expression.Convert(property, typeof(object)), parameter);

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
        var pagedInsuranceVehicleDetails = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        // Return the response in the desired format
        var result = new
        {
            data = pagedInsuranceVehicleDetails.Select(ivd => new
            {
                ivd.Id,
                ivd.VehicleType,
                ivd.VehicleModel,
                ivd.VehicleYear,
                ivd.Duration,
                ivd.RiskFactor,
                ivd.Region,
            }).ToArray(),
            recordsTotal = await _context.InsuranceVehicleDetails.CountAsync(),
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


    // 2.1 Read - Lấy InsuranceVehicleDetail theo Id, chỉ lấy record chưa xóa mềm
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInsuranceVehicleDetailById(int id)
    {
        try
        {
            var detail = await _context.InsuranceVehicleDetails
                .Where(d => d.Id == id && d.DeleteAt == null)
                .Select(d => new
                {
                    d.Id,
                    d.PlanId,
                    d.AnnualPaymentAmount,
                    d.Premium,
                    d.Deductible,
                    d.VehicleType,
                    d.VehicleModel,
                    d.VehicleYear,
                    d.Duration,
                    d.RiskFactor,
                    d.Region,
                    CreatedAt = d.CreatedAt.HasValue ? d.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                    UpdatedAt = d.UpdatedAt.HasValue ? d.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
                })
                .FirstOrDefaultAsync();

            if (detail == null)
            {
                return NotFound(new { Message = "Insurance vehicle detail not found" });
            }

            return Ok(detail);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 3. Update - Cập nhật thông tin InsuranceVehicleDetail, chỉ cập nhật record chưa xóa mềm
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInsuranceVehicleDetail(int id, [FromBody] InsuranceVehicleDetail detail)
    {
        try
        {
            var existingDetail = await _context.InsuranceVehicleDetails
                .Where(d => d.Id == id && d.DeleteAt == null)
                .FirstOrDefaultAsync();
            if (existingDetail == null)
            {
                return NotFound(new { Message = "Insurance vehicle detail not found" });
            }

            existingDetail.PlanId = detail.PlanId;
            existingDetail.AnnualPaymentAmount = detail.AnnualPaymentAmount;
            existingDetail.Premium = detail.Premium;
            existingDetail.Deductible = detail.Deductible;
            existingDetail.VehicleType = detail.VehicleType;
            existingDetail.VehicleModel = detail.VehicleModel;
            existingDetail.VehicleYear = detail.VehicleYear;
            existingDetail.Duration = detail.Duration;
            existingDetail.RiskFactor = detail.RiskFactor;
            existingDetail.Region = detail.Region;
            existingDetail.UpdatedAt = DateTime.UtcNow;

            _context.InsuranceVehicleDetails.Update(existingDetail);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 4. Delete - Xóa mềm InsuranceVehicleDetail
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeleteInsuranceVehicleDetail(int id)
    {
        try
        {
            var detail = await _context.InsuranceVehicleDetails
                .Where(d => d.Id == id && d.DeleteAt == null)
                .FirstOrDefaultAsync();
            if (detail == null)
            {
                return NotFound(new { Message = "Insurance vehicle detail not found" });
            }

            detail.DeleteAt = DateTime.UtcNow;
            _context.InsuranceVehicleDetails.Update(detail);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 5. Delete - Xóa cứng InsuranceVehicleDetail
    [HttpDelete("{id}/hard")]
    public async Task<IActionResult> HardDeleteInsuranceVehicleDetail(int id)
    {
        try
        {
            var detail = await _context.InsuranceVehicleDetails.FirstOrDefaultAsync(d => d.Id == id);
            if (detail == null)
            {
                return NotFound(new { Message = "Insurance vehicle detail not found" });
            }

            _context.InsuranceVehicleDetails.Remove(detail);
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
