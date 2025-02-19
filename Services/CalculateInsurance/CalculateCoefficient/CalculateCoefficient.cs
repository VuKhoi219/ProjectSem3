namespace Project_Sem3.Helper;

public class CalculateCoefficient
{
    // hệ số lối sống 
    public decimal lifestyleCoefficient(string Lifestyle )
    {
        switch (Lifestyle)
        {
            case "Hút thuốc":
                return 0.08m;
            case "Uống rượu":
                return 0.06m;
            case "Béo phì" :
                return 0.03m;
            case "Không có":
                return 0.01m;
            default:
                return 0.02m;
        }
    } 
    // hệ số tuổi
    public decimal ageCoefficient(int age)
    {
        if (age < 30 )
        {
            return 0.01m;
        }
        else if(age>= 30 && age < 40)
        {
            return 0.02m;
        }
        else if(age >=40 && age<50)
        {
            return 0.03m;
        }
        else
        {
            return 0.04m;
        }
    }
    // hệ số sức khoẻ
    public decimal healthCoefficient(string healthStatus)
    {
        switch (healthStatus.ToLower().Trim())
        {
            case "khỏe mạnh":
                return 0.01m;
            case "tiểu đường nhẹ":
                return 0.02m;
            case "tiểu đường nặng":
                return 0.05m;
            case "tăng huyết áp":
                return 0.03m;
            case "béo phì":
                return 0.02m;
            case "tim mạch":
                return 0.02m;
            case "hút thuốc":
                return 0.02m;
            default:
                return 0.015m; // Nếu không xác định được trạng thái sức khỏe
        }
    }
    // hệ số loại xe 
    public decimal vehicleCoefficient(string vehicle,string typeBrand)
    {
        if (vehicle.Trim().ToLower() == "ô tô")
        {
            List<string> sportsCars = new List<string> { "Ferrari", "Lamborghini", "Porsche" };
            List<string> sedans = new List<string> { "Toyota", "Honda" };
            List<string> SUV = new List<string> { "BMW", "Mercedes" };
            List<string> tram = new List<string> { "Tesla", "Rivian" };

            if (sportsCars.Contains(typeBrand.ToLower().Trim()))
            {
                return 0.1m; // Xe thể thao (được tăng giá trị vì hiệu suất cao)
            }
            else if (sedans.Contains(typeBrand.Trim().ToLower()))
            {
                return 0.04m; // Xe sedan (tăng lên vì chúng thường phổ biến và tiện nghi)
            }
            else if (SUV.Contains(typeBrand.Trim().ToLower()))
            {
                return 0.06m; // Xe SUV (tăng lên vì SUV thường lớn và đa dụng hơn)
            }
            else if (tram.Contains(typeBrand.Trim().ToLower()))
            {
                return 0.03m; // Xe điện (tăng vì giá trị cao của xe điện, nhất là Tesla)
            }
            else
            {
                return 0.02m; // Nếu không tìm thấy, trả về hệ số mặc định
            }
        }
        else if(vehicle.Trim().ToLower() == "xe máy")
        {
            List<string> sportBikes = new List<string> { "Yamaha", "Kawasaki", "Ducati" };
            List<string> cruisers = new List<string> { "Harley-Davidson", "Indian", "Honda" };
            List<string> scooters = new List<string> { "Vespa", "Honda", "Piaggio" };
            List<string> electricBikes = new List<string> { "Zero Motorcycles", "Harley-Davidson LiveWire", "Energica" };
            if (sportBikes.Contains(typeBrand.Trim().ToLower()))
            {
                return 0.06m; // Xe thể thao
            }
            else if (cruisers.Contains(typeBrand.Trim().ToLower()))
            {
                return 0.04m; // Xe cruiser
            }
            else if (scooters.Contains(typeBrand.Trim().ToLower()))
            {
                return 0.01m; // Xe scooter
            }
            else if (electricBikes.Contains(typeBrand.Trim().ToLower()))
            {
                return 0.02m; // Xe điện
            }
            else
            {
                return 0.01m; // Nếu không tìm thấy, trả về hệ số mặc định
            }
        }
        else
        {
            return 0.01m;
        }
    }
    
    // hêj số tai nạn 
    public decimal accidentCoefficient(int numberOfAccidents, int yearsWithoutAccident)
    {
        // Nếu người lái xe không có tai nạn nào trong 5 năm
        if (numberOfAccidents == 0 && yearsWithoutAccident >= 5)
        {
            return 0.01m; // Hệ số cơ bản nếu không có tai nạn trong 5 năm
        }

        // Nếu có tai nạn, tính hệ số theo số lượng tai nạn
        if (numberOfAccidents == 1)
        {
            return 0.02m; // Một vụ tai nạn trong vòng 3 năm
        }
        else if (numberOfAccidents == 2)
        {
            return 0.05m; // Hai vụ tai nạn trong vòng 3 năm
        }
        else if (numberOfAccidents >= 3)
        {
            return 0.02m; // Ba vụ tai nạn trở lên trong vòng 3 năm
        }
    
        return 0.0m; // Nếu không có tai nạn, trả về hệ số cơ bản
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
            return 0.05m;
        }
        return 0.01m;
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
            return 0.05m;
        }
        return 0.01m;
    }
    
    public decimal assetAgeRiskCoefficient(int assetAge)
    {
        if (assetAge <= 0)
        {
            return 0m;
        }
        else if (assetAge <= 50)
        {
            return 0.05m;
        }
        else
        {
            return 0.1m;
        }
    }
    // hệ số nghề nghiệp 
    public decimal careerCoefficient(string career)
    {
        List<string> careers = new List<string>
        {
            "Cứu hoả",
            "Phi công",
            "Khai thác gỗ",
            "Thu gom rác thải và tái chế",
            "Thợ hàn dưới nước",
            "Công nhân dầu khí",
            "Công nhân xây dựng",
            "Ngư dân vùng biển sâu",
            " Thợ làm sắt thép",
            "Người đấu bò",
            "Thợ mỏ",
            "Làm nông",
            "Thợ mỏ",
            "Sĩ quan cảnh sát",
            "Tài xế xe tải",
            "Diễn viên đóng thế",
        };
        if (careers.Contains(career.ToLower().Trim()))
        {
            return 0.05m;
        }

        return 0.01m;
    }

    public decimal constructionMaterialRiskCoefficient(string material)
    {
        string t = "Gỗ";
        if (t.Equals(material.Trim().ToLower()))
        {
            return 0.03m;
        }

        return 0.01m;
    }

    public decimal crimeRiskCoefficient(string city)
    {
        string t = "Buôn Ma Thuật";
        if (t.Equals(city.Trim().ToLower()))
        {
            return 0.02m;
        }

        return 0.01m;
    }
}