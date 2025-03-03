using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Sem3.Data;
using Project_Sem3.Models;

namespace Project_Sem3.Controllers;

[ApiController]
[Route("api/insurance-contracts")]
public class InsuranceContractsController : ControllerBase
{
    private readonly MyDbContext _context;

    public InsuranceContractsController(MyDbContext context)
    {
        _context = context;
    }

    // 1. Get All - Lấy tất cả InsuranceContracts với phân trang, chỉ lấy record chưa xóa mềm
    [HttpGet]
    public async Task<IActionResult> GetAllInsuranceContracts(int page = 1, int pageSize = 10)
    {
        try
        {
            var totalRecords = await _context.InsuranceContracts
                .Where(c => c.DeleteAt == null)
                .CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var contracts = await _context.InsuranceContracts
                .Where(c => c.DeleteAt == null)
                .Select(c => new
                {
                    c.Id,
                    c.UserId,
                    c.PlanId,
                    c.StartDate,
                    c.EndDate,
                    c.Status,
                    CreatedAt = c.CreatedAt.HasValue ? c.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                    UpdatedAt = c.UpdatedAt.HasValue ? c.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
                })
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Data = contracts,
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

    // 2. Get By Id - Lấy InsuranceContract theo Id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInsuranceContractById(int id)
    {
        try
        {
            var contract = await _context.InsuranceContracts
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.UserId,
                    c.PlanId,
                    c.StartDate,
                    c.EndDate,
                    c.Status,
                    CreatedAt = c.CreatedAt.HasValue ? c.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                    UpdatedAt = c.UpdatedAt.HasValue ? c.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                    DeleteAt = c.DeleteAt.HasValue ? c.DeleteAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
                })
                .FirstOrDefaultAsync();

            if (contract == null)
            {
                return NotFound(new { Message = "Insurance contract not found" });
            }

            return Ok(contract);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

// 3. Get By UserId - Lấy InsuranceContracts theo UserId, chỉ lấy record chưa xóa mềm, có phân trang
[HttpGet("user/{userId}")]
public async Task<IActionResult> GetInsuranceContractsByUserId(int userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
{
    try
    {
        // Đảm bảo pageNumber và pageSize hợp lệ
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        // Lấy tổng số bản ghi thỏa mãn điều kiện
        var totalRecords = await _context.InsuranceContracts
            .CountAsync(c => c.UserId == userId && c.DeleteAt == null);

        // Tính tổng số trang
        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        // Lấy dữ liệu với phân trang
        var contracts = await _context.InsuranceContracts
            .Where(c => c.UserId == userId && c.DeleteAt == null)
            .OrderBy(c => c.CreatedAt) // Sắp xếp theo CreatedAt (tùy chọn, bạn có thể thay đổi tiêu chí sắp xếp)
            .Skip((pageNumber - 1) * pageSize) // Bỏ qua các bản ghi của các trang trước
            .Take(pageSize) // Lấy số bản ghi theo pageSize
            .Select(c => new
            {
                c.Id,
                c.UserId,
                c.PlanId,
                c.StartDate,
                c.EndDate,
                c.Status,
                CreatedAt = c.CreatedAt.HasValue ? c.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                UpdatedAt = c.UpdatedAt.HasValue ? c.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
            })
            .ToListAsync();

        // Trả về dữ liệu kèm thông tin phân trang
        var result = new
        {
            TotalRecords = totalRecords,
            TotalPages = totalPages,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            Data = contracts
        };

        return Ok(result);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
    }
}
    // 4. Create - Tạo một InsuranceContract mới
    [HttpPost]
    public async Task<IActionResult> CreateInsuranceContract([FromBody] InsuranceContract contract)
    {
        try
        {
            if (contract == null || contract.UserId <= 0 || contract.PlanId <= 0 ||
                contract.StartDate == default || contract.EndDate == default)
            {
                return BadRequest(new { Message = "Invalid insurance contract data" });
            }
            contract.CreatedAt = DateTime.UtcNow;
            contract.CreatedBy = contract.UserId;
            contract.Status = ContractStatus.Pending; // Mặc định là Pending
            _context.InsuranceContracts.Add(contract);
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
    [HttpPut("{id}/{newStatus}")]
    public async Task<IActionResult> EditContractStatus(int id, string newStatus)
    {
        try
        {
            var contract = await _context.InsuranceContracts.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (contract == null)
            {
                return Ok(false);
            }

            switch (newStatus.ToLower())
            {
                case "pending":
                    contract.Status = ContractStatus.Pending;
                    break;
                case "active":
                    contract.Status = ContractStatus.Active;
                    break;
                case "expired":
                    contract.Status = ContractStatus.Expired;
                    break;
                case "cancelled":
                    contract.Status = ContractStatus.Cancelled;
                    break;
                default:
                    return Ok(false);
            }

            contract.UpdatedAt = DateTime.UtcNow;
            _context.InsuranceContracts.Update(contract);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 6. Update - Cập nhật toàn bộ thông tin InsuranceContract
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInsuranceContract(int id, [FromBody] InsuranceContract contract)
    {
        try
        {
            var existingContract = await _context.InsuranceContracts.FirstOrDefaultAsync(c => c.Id == id);
            if (existingContract == null)
            {
                return NotFound(new { Message = "Insurance contract not found" });
            }

            existingContract.UserId = contract.UserId;
            existingContract.PlanId = contract.PlanId;
            existingContract.StartDate = contract.StartDate;
            existingContract.EndDate = contract.EndDate;
            existingContract.Status = contract.Status;
            existingContract.UpdatedAt = DateTime.UtcNow;

            _context.InsuranceContracts.Update(existingContract);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 7. Delete Soft - Xóa mềm InsuranceContract
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeleteInsuranceContract(int id)
    {
        try
        {
            var contract = await _context.InsuranceContracts.FirstOrDefaultAsync(c => c.Id == id);
            if (contract == null)
            {
                return NotFound(new { Message = "Insurance contract not found" });
            }

            contract.DeleteAt = DateTime.UtcNow;
            _context.InsuranceContracts.Update(contract);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 8. Delete Hard - Xóa cứng InsuranceContract
    [HttpDelete("{id}/hard")]
    public async Task<IActionResult> HardDeleteInsuranceContract(int id)
    {
        try
        {
            var contract = await _context.InsuranceContracts.FirstOrDefaultAsync(c => c.Id == id);
            if (contract == null)
            {
                return NotFound(new { Message = "Insurance contract not found" });
            }

            _context.InsuranceContracts.Remove(contract);
            await _context.SaveChangesAsync();

            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }
[HttpGet("contract-detail/{contractId}/{planId}")]
public async Task<IActionResult> GetContractDetail(int contractId, int planId)
{
    try
    {
        var contract = await _context.InsuranceContracts
            .AsNoTracking() // Tăng hiệu suất cho read-only
            .Where(c => c.Id == contractId)
            .Select(c => new
            {
                Id = c.Id,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Status = c.Status,
                InsurancePlan = c.PlanId == planId
                    ? _context.InsurancePlans
                        .Where(l => l.Id == planId)
                        .Select(p => new
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description,
                            Status = p.Status,
                            CoverageAmount = p.CoverageAmount,
                            LifeDetail = p.Type == InsuranceType.Life
                                ? _context.InsuranceLifeDetails
                                    .Where(l => l.PlanId == p.Id && l.Id == c.DetailId)
                                    .FirstOrDefault()
                                : null,
                            HealthDetail = p.Type == InsuranceType.Health
                                ? _context.InsuranceHealthDetails
                                    .Where(h => h.PlanId == p.Id && h.Id == c.DetailId)
                                    .FirstOrDefault()
                                : null,
                            VehicleDetail = p.Type == InsuranceType.Vehicle
                                ? _context.InsuranceVehicleDetails
                                    .Where(v => v.PlanId == p.Id && v.Id == c.DetailId)
                                    .FirstOrDefault()
                                : null,
                            PropertyDetail = p.Type == InsuranceType.Property
                                ? _context.InsurancePropertyDetails
                                    .Where(pr => pr.PlanId == p.Id && pr.Id == c.DetailId)
                                    .FirstOrDefault()
                                : null
                        })
                        .FirstOrDefault() // Thêm FirstOrDefault() để vật thể hóa InsurancePlan
                    : null
            })
            .FirstOrDefaultAsync();

        if (contract == null)
        {
            return NotFound(new { Message = $"Contract with ID {contractId} not found or has been deleted" });
        }

        if (contract.InsurancePlan == null)
        {
            return NotFound(new { Message = $"Plan with ID {planId} not found for contract {contractId}" });
        }

        return Ok(new { Data = contract });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
    }
}
}
