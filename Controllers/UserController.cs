using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
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
    public async Task<IActionResult> GetAllUsers(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? search = null,
    [FromQuery] string? orderColumn = null,
    [FromQuery] string? orderDir = null)
{
    try
    {
        var query = _context.Users.AsQueryable(); // Start with the Users query.

        // Handle search query. Apply the search to relevant fields (adjust this based on your model).
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(x =>
                (x.FullName != null && EF.Functions.Like(x.FullName, $"%{search}%")) || // Search FullName
                (x.Email != null && EF.Functions.Like(x.Email, $"%{search}%")) || // Search Email
                (x.Phone != null && EF.Functions.Like(x.Phone, $"%{search}%")) || // Search Phone
                (x.CitizenIdentificationCard != null && EF.Functions.Like(x.CitizenIdentificationCard, $"%{search}%")) // Search CitizenIdentificationCard
            );
        }

        // Handle sorting by dynamic column and direction
        if (!string.IsNullOrEmpty(orderColumn) && !string.IsNullOrEmpty(orderDir))
        {
            var parameter = Expression.Parameter(typeof(User), "x");
            var property = Expression.Property(parameter, orderColumn);
            var lambda = Expression.Lambda<Func<User, object>>(Expression.Convert(property, typeof(object)), parameter);

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
        var pagedUsers = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        // Return the response in the desired format
        var result = new
        {
            data = pagedUsers.Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.Phone,
                u.CitizenIdentificationCard,
                u.Gender,
                u.Status,
                u.CreatedAt,
                u.UpdatedAt
            }).ToArray(),
            recordsTotal = await _context.Users.CountAsync(),
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
