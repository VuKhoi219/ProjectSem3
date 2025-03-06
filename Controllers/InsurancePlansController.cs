using System.Linq.Expressions;
using Bogus.DataSets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Sem3.Data;
using Project_Sem3.Models;

[ApiController]
[Route("api/insurance-plans")]
public class InsurancePlansController : ControllerBase
{
    private readonly MyDbContext _context;

    public InsurancePlansController(MyDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // 1. Lấy danh sách gói bảo hiểm với phân trang
    [HttpGet]
    public async Task<IActionResult> GetAllInsurancePlans(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? search = null,
    [FromQuery] string? orderColumn = null,
    [FromQuery] string? orderDir = null)
{
    try
    {
        // Starting the query, including the related insurance contracts and details
        var query = _context.InsurancePlans.AsQueryable();

        // Handle search query (for Name, Description, Type, and Status)
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(x =>
                (x.Name != null && EF.Functions.Like(x.Name, $"%{search}%")) || // Search Name
                (x.Description != null && EF.Functions.Like(x.Description, $"%{search}%")) || // Search Description
                (x.Type.ToString() != null && EF.Functions.Like(x.Type.ToString(), $"%{search}%")) // Search Type
            );
        }

        // Handle sorting by dynamic column and direction
        if (!string.IsNullOrEmpty(orderColumn) && !string.IsNullOrEmpty(orderDir))
        {
            var parameter = Expression.Parameter(typeof(InsurancePlan), "x");
            var property = Expression.Property(parameter, orderColumn);
            var lambda = Expression.Lambda<Func<InsurancePlan, object>>(Expression.Convert(property, typeof(object)), parameter);

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
        var pagedInsurancePlans = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        // Return the response in the desired format
        var result = new
        {
            data = pagedInsurancePlans.Select(ip => new
            {
                ip.Id,
                ip.Name,
                ip.Description,
                Type = ip.Type.ToString(),
                ip.CoverageAmount,
                ip.UpdatedAt
            }).ToArray(),
            recordsTotal = await _context.InsurancePlans.CountAsync(),
            recordsFiltered = totalCount,
            page = page,
            pageSize = pageSize
        };

        return Ok(result);
    }
    catch (Exception e)
    {
        // Log the error (optional)
        Console.WriteLine(e);
        return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
    }
}


    // 2. Lấy chi tiết gói bảo hiểm theo Id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlanById(int id )
    {
        if (id <= 0)
            return BadRequest(new { Message = "Invalid plan ID" });

        try
        {
            var plan = await _context.InsurancePlans
                .Where(p => p.Id == id && p.DeleteAt == null)
                .Include(p => p.InsuranceContracts)
                .Include(p => p.Creator)
                .Include(p => p.Updater)
                .Include(p => p.Deleter)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Status,
                    p.CoverageAmount, // Thêm trường này
                })
                .FirstOrDefaultAsync();

            if (plan == null)
                return NotFound(new { Message = $"Insurance plan with ID {id} not found or has been deleted" });

            return Ok(new { Data = plan });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
        }
    }

    [HttpGet("{id}/{detailId}")]
    public async Task<IActionResult> GetPlanAndDetail(int id , int detailId)
    {
      if (id <= 0)
        return BadRequest(new { Message = "Invalid plan ID" });
      try
      {
        var plan = await _context.InsurancePlans
          .Where(p => p.Id == id && p.DeleteAt == null)
          .Select(p => new
          {
            p.Id,
            p.Name,
            p.Description,
            p.Status,
            p.CoverageAmount, // Thêm trường này
            LifeDetail = p.Type == InsuranceType.Life ? _context.InsuranceLifeDetails.Where(l => l.PlanId == p.Id && l.Id == detailId).FirstOrDefault() : null,
            HealthDetail = p.Type == InsuranceType.Health ? _context.InsuranceHealthDetails.Where(h => h.PlanId == p.Id && h.Id == detailId).FirstOrDefault():null,
            VehicleDetail = p.Type == InsuranceType.Vehicle ? _context.InsuranceVehicleDetails.Where(v => v.PlanId == p.Id && v.Id == detailId).FirstOrDefault():null,
            PropertyDetail = p.Type == InsuranceType.Property ? _context.InsurancePropertyDetails.Where(pr => pr.PlanId == p.Id && pr.Id == detailId).FirstOrDefault() : null,
          })
          .FirstOrDefaultAsync();

        if (plan == null)
          return NotFound(new { Message = $"Insurance plan with ID {id} not found or has been deleted" });

        return Ok(new { Data = plan });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
      }
    }
    // 3. Thêm mới gói bảo hiểm
    [HttpPost]
    public async Task<IActionResult> CreatePlan([FromBody] InsurancePlan plan)
    {
        if (plan == null || string.IsNullOrWhiteSpace(plan.Name) || plan.CoverageAmount <= 0)
            return BadRequest(new { Message = "Invalid insurance plan data. Name and CoverageAmount are required" });

        if (!ModelState.IsValid)
            return BadRequest(new { Message = "Validation failed", Errors = ModelState });

        try
        {
            plan.Status = InsuranceStatus.Inactive;
            plan.CreatedAt = DateTime.UtcNow;
            plan.DeleteAt = null;
            plan.CreatedBy = 1; // Thay bằng logic lấy user ID thực tế từ authentication

            _context.InsurancePlans.Add(plan);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlanById), new { id = plan.Id }, new { Message = "Insurance plan created successfully", Data = plan });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
        }
    }

    // 4. Cập nhật gói bảo hiểm
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlan(int id, [FromBody] InsurancePlan updatedPlan)
    {
        if (id <= 0 || updatedPlan == null)
            return BadRequest(new { Message = "Invalid ID or insurance plan data" });

        if (!ModelState.IsValid)
            return BadRequest(new { Message = "Validation failed", Errors = ModelState });

        try
        {
            var plan = await _context.InsurancePlans
                .FirstOrDefaultAsync(p => p.Id == id && p.DeleteAt == null);

            if (plan == null)
                return NotFound(new { Message = $"Insurance plan with ID {id} not found or has been deleted" });

            plan.Name = updatedPlan.Name?.Trim() ?? plan.Name;
            plan.Description = updatedPlan.Description?.Trim() ?? plan.Description;
            plan.Type = updatedPlan.Type;
            plan.Status = updatedPlan.Status;
            plan.CoverageAmount = updatedPlan.CoverageAmount > 0 ? updatedPlan.CoverageAmount : plan.CoverageAmount; // Thêm trường này
            plan.UpdatedAt = DateTime.UtcNow;
            plan.UpdatedBy = 1; // Thay bằng logic lấy user ID thực tế từ authentication

            _context.InsurancePlans.Update(plan);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Insurance plan {id} updated successfully", Data = plan });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
        }
    }

    // 5. Xóa mềm gói bảo hiểm
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeletePlan(int id)
    {
        if (id <= 0)
            return BadRequest(new { Message = "Invalid plan ID" });

        try
        {
            var plan = await _context.InsurancePlans
                .FirstOrDefaultAsync(p => p.Id == id && p.DeleteAt == null);

            if (plan == null)
                return NotFound(new { Message = $"Insurance plan with ID {id} not found or has been deleted" });

            plan.DeleteAt = DateTime.UtcNow;
            plan.Status = InsuranceStatus.Inactive;
            plan.DeleteBy = 1; // Thay bằng logic lấy user ID thực tế từ authentication

            _context.InsurancePlans.Update(plan);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Insurance plan {id} has been soft deleted" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
        }
    }

    // 6. Xóa cứng gói bảo hiểm
    [HttpDelete("{id}/hard")]
    public async Task<IActionResult> HardDeletePlan(int id)
    {
        if (id <= 0)
            return BadRequest(new { Message = "Invalid plan ID" });

        try
        {
            var plan = await _context.InsurancePlans.FirstOrDefaultAsync(p => p.Id == id);
            if (plan == null)
                return NotFound(new { Message = $"Insurance plan with ID {id} not found" });

            _context.InsurancePlans.Remove(plan);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Insurance plan {id} has been permanently deleted" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
        }
    }
    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetInsurancePlans(string type)
    {
      if (string.IsNullOrWhiteSpace(type))
        return BadRequest(new { Message = "Invalid type" });

      try
      {
        // Chuyển đổi type từ string sang InsuranceType enum
        if (!Enum.TryParse<InsuranceType>(type, true, out var insuranceType))
          return BadRequest(new { Message = $"Invalid insurance type: {type}. Must be one of: Life, Health, Vehicle, Property" });

        var plans = await _context.InsurancePlans
          .Where(p => p.Type == insuranceType && p.Status == InsuranceStatus.Active && p.DeleteAt == null)
          .Include(p => p.InsuranceContracts)
          .Include(p => p.Creator)
          .Include(p => p.Updater)
          .Include(p => p.Deleter)
          .Select(p => new
          {
            p.Id,
            p.Name,
            p.Description,
            p.Type,
            p.Status,
            p.CoverageAmount
          })
          .ToListAsync();
        if (!plans.Any())
          return NotFound(new { Message = $"No active insurance plans found for type {type}" });
        return Ok(new { Data = plans });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
      }
    }

}
