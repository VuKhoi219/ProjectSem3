using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Sem3.Data; // Giả sử đây là namespace chứa DbContext
using Project_Sem3.Models; // Giả sử đây là namespace chứa Notification

namespace Project_Sem3.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly MyDbContext _context;

    public NotificationsController(MyDbContext context)
    {
        _context = context;
    }

    // 1. Create - Tạo một Notification mới
    [HttpPost]
    public async Task<IActionResult> CreateNotification([FromBody] Notification notification)
    {
        try
        {
            // Kiểm tra dữ liệu đầu vào
            if (notification == null || notification.UserId <= 0 || string.IsNullOrEmpty(notification.Message))
            {
                return BadRequest(new { Message = "Invalid notification data" });
            }

            // Gán thời gian tạo và trạng thái mặc định
            notification.CreatedAt = DateTime.UtcNow;
            notification.IsRead = false; // Mặc định là chưa đọc

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Notification created successfully", NotificationId = notification.Id });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 2. Read - Lấy tất cả Notifications với phân trang, chỉ lấy record chưa xóa mềm
    [HttpGet]
    public async Task<IActionResult> GetAllNotifications(int page, int pageSize)
    {
        try
        {
            var totalRecords = await _context.Notifications
                .Where(n => n.DeleteAt == null)
                .CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var notifications = await _context.Notifications
                .Where(n => n.DeleteAt == null) // Chỉ lấy những record chưa bị xóa mềm
                .Select(n => new
                {
                    n.Id,
                    n.UserId,
                    n.Message,
                    n.IsRead,
                    CreatedAt = n.CreatedAt.HasValue ? n.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                    UpdatedAt = n.UpdatedAt.HasValue ? n.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
                })
                .OrderBy(n => n.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Data = notifications,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize
            });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 2.1 Read - Lấy Notification theo Id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotificationById(int id)
    {
        try
        {
            var notification = await _context.Notifications
                .Where(n => n.Id == id)
                .Select(n => new
                {
                    n.Id,
                    n.UserId,
                    n.Message,
                    n.IsRead,
                    CreatedAt = n.CreatedAt.HasValue ? n.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                    UpdatedAt = n.UpdatedAt.HasValue ? n.UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                    DeleteAt = n.DeleteAt.HasValue ? n.DeleteAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
                })
                .FirstOrDefaultAsync();

            if (notification == null)
            {
                return NotFound(new { Message = "Notification not found" });
            }

            return Ok(notification);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 3. Update - Cập nhật thông tin Notification
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNotification(int id, [FromBody] Notification notification)
    {
        try
        {
            var existingNotification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
            if (existingNotification == null)
            {
                return NotFound(new { Message = "Notification not found" });
            }

            // Cập nhật các trường cần thiết
            existingNotification.UserId = notification.UserId;
            existingNotification.Message = notification.Message;
            existingNotification.IsRead = notification.IsRead;
            existingNotification.UpdatedAt = DateTime.UtcNow;

            _context.Notifications.Update(existingNotification);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Notification updated successfully" });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 4. Delete - Xóa mềm Notification
    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeleteNotification(int id)
    {
        try
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
            if (notification == null)
            {
                return NotFound(new { Message = "Notification not found" });
            }

            notification.DeleteAt = DateTime.UtcNow;
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Notification soft deleted successfully" });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }

    // 4.1 Delete - Xóa cứng Notification
    [HttpDelete("{id}/hard")]
    public async Task<IActionResult> HardDeleteNotification(int id)
    {
        try
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
            if (notification == null)
            {
                return NotFound(new { Message = "Notification not found" });
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Notification permanently deleted" });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "An error occurred", Error = e.Message });
        }
    }
}
