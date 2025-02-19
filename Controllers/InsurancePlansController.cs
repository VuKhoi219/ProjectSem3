using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Sem3.Data;
using Project_Sem3.Models;

[ApiController]
[Route("api/[controller]")]
public class InsurancePlansController : ControllerBase
{
    private readonly MyDbContext _context;

    public InsurancePlansController(MyDbContext context)
    {
        _context = context;
    }

    // 1. Lấy danh sách gói bảo hiểm
    [HttpGet]
    public async Task<IActionResult> GetAllPlans()
    {
        var plans = await _context.InsurancePlans.Include(p => p.InsuranceContracts).ToListAsync();
        return Ok(plans);
    }

    // 2. Lấy chi tiết gói bảo hiểm
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlanById(int id)
    {
        var plan = await _context.InsurancePlans.Include(p => p.VehicleDetails)
                                               .Include(p => p.LifeDetails)
                                               .Include(p => p.PropertyDetails)
                                               .Include(p => p.HealthDetails)
                                               .FirstOrDefaultAsync(p => p.Id == id);
        if (plan == null) return NotFound();
        return Ok(plan);
    }

    // 3. Thêm mới gói bảo hiểm
    [HttpPost]
    public async Task<IActionResult> CreatePlan([FromBody] InsurancePlan plan)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        plan.CreatedAt = DateTime.UtcNow;
        
        _context.InsurancePlans.Add(plan);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPlanById), new { id = plan.Id }, plan);
    }

    // 4. Cập nhật gói bảo hiểm
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlan(int id, [FromBody] InsurancePlan updatedPlan)
    {
        
        var plan = await _context.InsurancePlans.FindAsync(id);
        if (plan == null) return NotFound();

        plan.Name = updatedPlan.Name;
        plan.Destination = updatedPlan.Destination;
        plan.Type = updatedPlan.Type;
        plan.Status = updatedPlan.Status;
        plan.UpdatedAt = DateTime.UtcNow;

        _context.InsurancePlans.Update(plan);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // 5. Xóa gói bảo hiểm (mềm)
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeletePlan(int id)
    {
        var plan = await _context.InsurancePlans.FindAsync(id);
        if (plan == null) return NotFound();

        plan.DeleteAt = DateTime.UtcNow;
        _context.InsurancePlans.Update(plan);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}