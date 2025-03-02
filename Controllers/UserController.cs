using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Project_Sem3.Data;
using Project_Sem3.Models;

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
      var users = await _context.Users
        .Select(user => new
        {
          Id = user.Id,
          FullName = user.FullName,
          Email = user.Email,
          Phone = user.Phone,
          Gender = user.Gender,
          CitizenIdentificationCard = user.CitizenIdentificationCard,
          DateOfBirth = user.DateOfBirth,
          Status = user.Status,
          RoleId = user.RoleId
        })
        .ToListAsync();

        return Ok(users);
    }

    // ✅ 2. Lấy thông tin người dùng theo ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _context.Users.Where(u => u.Id == id).Select(user => new
        {
          Id = user.Id,
          FullName = user.FullName,
          Email = user.Email,
          Phone = user.Phone,
          Gender = user.Gender,
          CitizenIdentificationCard = user.CitizenIdentificationCard,
          DateOfBirth = user.DateOfBirth,
          Status = user.Status,
          RoleId = user.RoleId
        }).FirstOrDefaultAsync();
        if (user == null)
            return NotFound(new { message = "Người dùng không tồn tại." });
        return Ok(user);
    }

    // ✅ 3. Thêm người dùng mới (Sign Up)
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User request)
    {
// Kiểm tra trùng lặp và thông báo cụ thể
      if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        return BadRequest(new { message = "Email đã tồn tại!" });

      if (!string.IsNullOrWhiteSpace(request.Phone) && await _context.Users.AnyAsync(u => u.Phone == request.Phone))
        return BadRequest(new { message = "Số điện thoại đã tồn tại!" });

      if (!string.IsNullOrWhiteSpace(request.CitizenIdentificationCard) && await _context.Users.AnyAsync(u => u.CitizenIdentificationCard == request.CitizenIdentificationCard))
        return BadRequest(new { message = "CCCD/CMND đã tồn tại!" });

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Phone = request.Phone,
            Gender = request.Gender,
            CitizenIdentificationCard = request.CitizenIdentificationCard,
            DateOfBirth = request.DateOfBirth,
            Status = Status.Active, // Mặc định Active
            RoleId = request.RoleId ?? 2, // Sửa lỗi ở đây
            CreatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, new { message = "Người dùng đã được tạo!", user = user });
    }

    // ✅ 4. Cập nhật thông tin người dùng
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] User request)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "Người dùng không tồn tại." });

        user.FullName = request.FullName;
        user.Email = request.Email;
        user.Phone = request.Phone;
        user.Gender = request.Gender;
        user.CitizenIdentificationCard = request.CitizenIdentificationCard;
        user.DateOfBirth = request.DateOfBirth;
        user.Status = request.Status;
        user.RoleId = request.RoleId;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var userDto = new User
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Gender = user.Gender,
            CitizenIdentificationCard = user.CitizenIdentificationCard,
            DateOfBirth = user.DateOfBirth,
            Status = user.Status,
            RoleId = user.RoleId
        };

        return Ok(new { message = "Cập nhật thành công!", user = userDto });
    }

    // ✅ 5. Xóa người dùng
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "Người dùng không tồn tại." });

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Người dùng đã được xóa!" });
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] UserLoginDto request)
    {
      Console.WriteLine($"Content-Type: {Request.ContentType}");
      Console.WriteLine($"Body: {await new StreamReader(Request.Body).ReadToEndAsync()}");
      if (request == null)
        return BadRequest(new { message = "Dữ liệu đầu vào không hợp lệ!" });

      Console.WriteLine($"Request received: Email={request.Email}, Password={request.Password}");

      var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

      if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        return Unauthorized(new { message = "Email hoặc mật khẩu không đúng!" });

      HttpContext.Session.SetInt32("UserId", user.Id);

      return Ok(new
      {
        message = "Đăng nhập thành công!",
        userId = user.Id,
        fullName = user.FullName,
        email = user.Email,
        roleId = user.RoleId
      });
    }
    public class UserLoginDto
    {
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
