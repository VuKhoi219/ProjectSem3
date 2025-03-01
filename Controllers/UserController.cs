using System.ComponentModel.DataAnnotations;
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
            .Select(user => new UserDto
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
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "Người dùng không tồn tại." });

        var userDto = new UserDto
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

        return Ok(userDto);
    }

    // ✅ 3. Thêm người dùng mới (Sign Up)
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest(new { message = "Email đã tồn tại!" });

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Phone = request.Phone,
            Gender = request.Gender,
            CitizenIdentificationCard = request.CitizenIdentificationCard,
            DateOfBirth = request.DateOfBirth,
            Status = request.Status,
            RoleId = request.RoleId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var userDto = new UserDto
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

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, new { message = "Người dùng đã được tạo!", user = userDto });
    }

    // ✅ 4. Cập nhật thông tin người dùng
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto request)
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

        var userDto = new UserDto
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

    // ✅ 6. API Đăng nhập (Sign In)
    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] UserLoginDto request)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

      if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        return Unauthorized(new { message = "Email hoặc mật khẩu không đúng!" });

      // ✅ Lưu userId vào Session
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


    // ✅ 7. DTOs (Định nghĩa trong cùng file)
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Gender Gender { get; set; }
        public string CitizenIdentificationCard { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Status Status { get; set; }
        public int RoleId { get; set; }
    }

    public class UserCreateDto
    {
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [MinLength(6)]
        public string Password { get; set; }

        public string Phone { get; set; }
        public Gender Gender { get; set; }
        public string CitizenIdentificationCard { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Status Status { get; set; }
        public int RoleId { get; set; }
    }

    public class UserUpdateDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Gender Gender { get; set; }
        public string CitizenIdentificationCard { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Status Status { get; set; }
        public int RoleId { get; set; }
    }

    public class UserLoginDto
    {
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
