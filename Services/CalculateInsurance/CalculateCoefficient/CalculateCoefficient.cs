namespace Project_Sem3.Helper;

public class CalculateCoefficient
{
    // hệ số tuổi
    public decimal ageCoefficient(int age)
    {
        if (age < 30 )
        {
            return 1.0m;
        }
        else if(age>= 30 && age < 40)
        {
            return 1.2m;
        }
        else if(age >=40 && age<50)
        {
            return 1.3m;
        }
        else
        {
            return 1.4m;
        }
    }
    // hệ số sức khoẻ
    public decimal healthCoefficient(string healthStatus)
    {
        switch (healthStatus.ToLower().Trim())
        {
            case "khỏe mạnh":
                return 1.0m;
            case "tiểu đường nhẹ":
                return 1.2m;
            case "tiểu đường nặng":
                return 1.5m;
            case "tăng huyết áp":
                return 1.3m;
            case "béo phì":
                return 1.2m;
            case "tim mạch":
                return 1.5m;
            case "hút thuốc":
                return 1.4m;
            default:
                return 1.1m; // Nếu không xác định được trạng thái sức khỏe
        }
    }
    // hệ số loại xe 
    public decimal carTypeCoefficient(string carBrand)
    {
        List<string> sportsCars = new List<string> { "Ferrari", "Lamborghini", "Porsche" };
        List<string> sedans = new List<string> { "Toyota", "Honda" };
        List<string> SUV = new List<string> { "BMW", "Mercedes" };
        List<string> tram = new List<string> { "Tesla", "Rivian" };
        
        if (sportsCars.Contains(carBrand.ToLower().Trim()))
        {
            return 1.5m; // Xe thể thao
        }
        else if (sedans.Contains(carBrand.Trim().ToLower()))
        {
            return 1.2m; // Xe sedan
        }
        else if (SUV.Contains(carBrand.Trim().ToLower()))
        {
            return 1.3m; // Xe SUV
        }
        else if (tram.Contains(carBrand.Trim().ToLower()))
        {
            return 1.1m; // Xe điện
        }
        else
        {
            return 1.0m; // Nếu không tìm thấy, trả về hệ số mặc định
        }

    }
    
    // hêj số tai nạn 
    public decimal accidentCoefficient(int numberOfAccidents, int yearsWithoutAccident)
    {
        // Nếu người lái xe không có tai nạn nào trong 5 năm
        if (numberOfAccidents == 0 && yearsWithoutAccident >= 5)
        {
            return 1.0m; // Hệ số cơ bản nếu không có tai nạn trong 5 năm
        }

        // Nếu có tai nạn, tính hệ số theo số lượng tai nạn
        if (numberOfAccidents == 1)
        {
            return 1.2m; // Một vụ tai nạn trong vòng 3 năm
        }
        else if (numberOfAccidents == 2)
        {
            return 1.5m; // Hai vụ tai nạn trong vòng 3 năm
        }
        else if (numberOfAccidents >= 3)
        {
            return 2.0m; // Ba vụ tai nạn trở lên trong vòng 3 năm
        }
    
        return 1.0m; // Nếu không có tai nạn, trả về hệ số cơ bản
    }
    // hệ số thành phoos  
    public decimal locationCoefficient(string city)
    {
        List<string> majorCities = new List<string>
        {
            "Hà Nội",
            "Hồ Chí Minh",
            "Hải Phòng",
            "Cần Thơ",
            "Đà Nẵng",
            "Biên Hòa",
            "Hải Dương",
            "Huế",
            "Thuận An",
            "Thủ Đức"
        };
        if (majorCities.Contains(city.Trim().ToLower()))
        {
            return 1.5m;
        }
        return 1.0m;
    }
    // hệ số thành phố có thiên tai 
    public decimal disasterRiskCoefficient(string city)
    {
        List<string> disasterCity = new List<string>
        {
            "Lạng Sơn",
            "Cao Bằng",
            "Lào Cai",
            "Yên Bái",
            "Phú Thọ",
            "Bắc Giang",
            "Bắc Kạn",
            "Thái Nguyên",
            "Hoà Bình",
            "Ninh Bình",
            "Thanh Hoá"
        };
        if (disasterCity.Contains(city))
        {
            return 1.5m;
        }
        return 1.0m;
    }
    
    // tính hệ số hợp đồng ( thời hạn hợp đồng )
    public decimal contractCoefficient(int contractDuration)
    {
        if (contractDuration >= 10)
        {
            return 0.9m * contractDuration;
        }else if (contractDuration >= 5 && contractDuration < 10)
        {
            return 0.95m * contractDuration;
        }
        return 1.0m * contractDuration;
    }
}