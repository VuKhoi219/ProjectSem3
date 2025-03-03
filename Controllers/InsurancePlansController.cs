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
    public async Task<IActionResult> GetAllPlans(int page = 1, int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
            return BadRequest(new { Message = "Page and pageSize must be greater than 0" });

        try
        {
            var query = _context.InsurancePlans.Where(p => p.DeleteAt == null);
            int totalRecords = await query.CountAsync();
            if (totalRecords == 0)
                return Ok(new { Message = "No insurance plans found", Data = new List<object>() });

            var plans = await query
                .Include(p => p.InsuranceContracts)
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Type,
                    p.Status,
                    p.CoverageAmount // Thêm trường này
                })
                .ToListAsync();

            return Ok(new
            {
                Data = plans,
                Pagination = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                    CurrentPage = page,
                    PageSize = pageSize
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
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
