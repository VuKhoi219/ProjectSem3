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

    // 3. Get By UserId - Lấy InsuranceContracts theo UserId, chỉ lấy record chưa xóa mềm
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetInsuranceContractsByUserId(int userId)
    {
        try
        {
            var contracts = await _context.InsuranceContracts
                .Where(c => c.UserId == userId && c.DeleteAt == null)
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

            return Ok(contracts);
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
}
