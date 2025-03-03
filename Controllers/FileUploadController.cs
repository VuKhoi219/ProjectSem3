using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
namespace Project_Sem3.Controllers;

[ApiController]
[Route("api/upload")]
public class FileUploadController : ControllerBase
{
private readonly IWebHostEnvironment _environment;

    public FileUploadController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Please select a file to upload" });
            }

            // Kiểm tra kích thước file (ví dụ: tối đa 10MB)
            if (file.Length > 10 * 1024 * 1024)
            {
                return BadRequest(new { message = "File size exceeds 10MB limit" });
            }

            // Kiểm tra loại file (ví dụ: chỉ cho phép ảnh)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { message = "Invalid file type. Allowed types: jpg, png, pdf" });
            }

            // Tạo đường dẫn lưu trữ
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Tạo tên file unique để tránh trùng lặp
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Lưu file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Trả về phản hồi thành công
            return Ok(new
            {
                message = "File uploaded successfully",
                fileName = uniqueFileName,
                filePath = $"/uploads/{uniqueFileName}",
                size = file.Length
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error uploading file", error = ex.Message });
        }
    }
}
