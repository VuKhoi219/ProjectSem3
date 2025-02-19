using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Project_Sem3.Helper;

public class Helpers
{
    public string ToTitleCase(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());
    }
    // kiểm tra có phải có phải sđt của vn không
    public bool IsValidVietnamesePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            return false; // Chuỗi rỗng không hợp lệ
        }

        // Biểu thức chính quy kiểm tra số điện thoại di động 10 số đầu 0 và các đầu số phổ biến
        // Lưu ý: Biểu thức này có thể cần được cập nhật để bao gồm đầy đủ các đầu số mới và số cố định nếu cần.
        string pattern = @"^(0[3|5|7|8|9])([0-9]{8})$";

        return Regex.IsMatch(phoneNumber, pattern);
    }
    // kiểm tra có phải email không
    public  bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return false;
        }
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
    }
    // kiểm tra xem có ký tự đặc biệt không ,có true , không false
    public  bool ContainsSpecialCharacters(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return false;
        }
        return Regex.IsMatch(str, @"[^a-zA-Z0-9\s]");
    }
    // chuển oj thành json
    public  string ConvertToJson(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
    
    
    
    //  bcrypt password
    public string HashPassword(string p)
    {
        return BCrypt.Net.BCrypt.HashPassword(p);
    }

    public bool VerifyPassword(string p, string hpwd)
    {
        return BCrypt.Net.BCrypt.Verify(p, hpwd);
    }
}

