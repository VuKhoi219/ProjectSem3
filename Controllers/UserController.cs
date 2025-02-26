using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Sem3.Data;
using Project_Sem3.Models;

namespace Project_Sem3.Controllers;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly MyDbContext _context;

    public UserController(MyDbContext context)
    {
        _context = context;
    }

    // ✅ 1. Lấy danh sách tất cả người dùng
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    // ✅ 2. Lấy thông tin người dùng theo ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "Người dùng không tồn tại." });

        return Ok(user);
    }

    // ✅ 3. Thêm người dùng mới (Create)
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        if (_context.Users.Any(u => u.Email == user.Email))
            return BadRequest(new { message = "Email đã tồn tại!" });

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    // ✅ 4. Cập nhật thông tin người dùng (Update)
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "Người dùng không tồn tại." });

        user.FullName = updatedUser.FullName;
        user.Email = updatedUser.Email;
        user.Phone = updatedUser.Phone;
        user.Role = updatedUser.Role;

        await _context.SaveChangesAsync();
        return Ok(user);
    }

    // ✅ 5. Xóa người dùng (Delete)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "Người dùng không tồn tại." });

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Người dùng đã được xóa." });
    }
}
