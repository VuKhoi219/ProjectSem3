namespace Project_Sem3.Helper;

public class CalculateCoefficient
{
    // Hệ số lối sống - Đã dùng int[] từ trước, giữ nguyên
    public decimal LifestyleCoefficient(int[] lifestyleIds)
    {
        decimal totalCoefficient = 0m;

        if (lifestyleIds == null || lifestyleIds.Length == 0)
        {
            return 0.002m; // Giá trị mặc định nếu không có lựa chọn
        }

        foreach (var id in lifestyleIds)
        {
            switch (id)
            {
                case 0: // Hút thuốc
                    totalCoefficient += 0.05m;
                    break;
                case 1: // Uống rượu
                    totalCoefficient += 0.06m;
                    break;
                case 2: // Béo phì
                    totalCoefficient += 0.03m;
                    break;
                case 3: // Không có
                    totalCoefficient += 0.01m;
                    break;
                default:
                    totalCoefficient += 0.02m; // Mặc định
                    break;
            }
        }

        return totalCoefficient;
    }

    // Hệ số tuổi - Không cần int[], giữ nguyên vì chỉ nhận một giá trị tuổi
    public decimal AgeCoefficient(int age)
    {
        if (age < 30)
        {
            return 0.001m;
        }
        else if (age >= 30 && age < 40)
        {
            return 0.002m;
        }
        else if (age >= 40 && age < 50)
        {
            return 0.003m;
        }
        else
        {
            return 0.004m;
        }
    }

    // Hệ số sức khỏe - Chuyển sang int[] để hỗ trợ nhiều trạng thái sức khỏe
    public decimal HealthCoefficient(int[] healthStatusIds)
    {
        decimal totalCoefficient = 0m;

        if (healthStatusIds == null || healthStatusIds.Length == 0)
        {
            return 0.05m; // Giá trị mặc định nếu không có lựa chọn
        }

        foreach (var id in healthStatusIds)
        {
            switch (id)
            {
                case 0: // Khỏe mạnh
                    totalCoefficient += 0.01m;
                    break;
                case 1: // Tiểu đường nhẹ
                    totalCoefficient += 0.02m;
                    break;
                case 2: // Tiểu đường nặng
                    totalCoefficient += 0.05m;
                    break;
                case 3: // Tăng huyết áp
                    totalCoefficient += 0.03m;
                    break;
                case 4: // Béo phì
                    totalCoefficient += 0.02m;
                    break;
                case 5: // Tim mạch
                    totalCoefficient += 0.02m;
                    break;
                case 6: // Hút thuốc
                    totalCoefficient += 0.02m;
                    break;
                default:
                    totalCoefficient += 0.015m; // Mặc định
                    break;
            }
        }

        return totalCoefficient;
    }

    // Hệ số loại xe - Chuyển sang int[] cho loại xe và thương hiệu
    public decimal VehicleCoefficient(int[] vehicleInfo)
    {
        if (vehicleInfo == null || vehicleInfo.Length < 2)
        {
            return 0.01m; // Giá trị mặc định nếu thiếu thông tin
        }

        int vehicleType = vehicleInfo[0]; // 0: Ô tô, 1: Xe máy
        int brandId = vehicleInfo[1];     // ID của thương hiệu

        if (vehicleType == 0) // Ô tô
        {
            switch (brandId)
            {
                case 0: // Ferrari
                case 1: // Lamborghini
                case 2: // Porsche
                    return 0.01m; // Xe thể thao
                case 3: // Toyota
                case 4: // Honda
                    return 0.004m; // Xe sedan
                case 5: // BMW
                case 6: // Mercedes
                    return 0.006m; // Xe SUV
                case 7: // Tesla
                case 8: // Rivian
                    return 0.003m; // Xe điện
                default:
                    return 0.02m; // Mặc định
            }
        }
        else if (vehicleType == 1) // Xe máy
        {
            switch (brandId)
            {
                case 0: // Yamaha
                case 1: // Kawasaki
                case 2: // Ducati
                    return 0.006m; // Xe thể thao
                case 3: // Harley-Davidson
                case 4: // Indian
                case 5: // Honda
                    return 0.004m; // Xe cruiser
                case 6: // Vespa
                case 7: // Piaggio
                    return 0.001m; // Xe scooter
                case 8: // Zero Motorcycles
                case 9: // Harley-Davidson LiveWire
                    return 0.002m; // Xe điện
                default:
                    return 0.001m; // Mặc định
            }
        }

        return 0.01m; // Mặc định cho loại xe khác
    }

    // Hệ số tai nạn - Giữ nguyên vì chỉ cần hai giá trị int
    public decimal AccidentCoefficient(int numberOfAccidents, int yearsWithoutAccident)
    {
        if (numberOfAccidents == 0 && yearsWithoutAccident >= 5)
        {
            return 0.001m;
        }
        if (numberOfAccidents == 1)
        {
            return 0.005m;
        }
        else if (numberOfAccidents == 2)
        {
            return 0.01m;
        }
        else if (numberOfAccidents >= 3)
        {
            return 0.02m;
        }
        return 0.0m;
    }

    // Hệ số thành phố - Chuyển sang int[]
    public decimal LocationCoefficient(int[] cityIds)
    {
        decimal totalCoefficient = 0m;

        if (cityIds == null || cityIds.Length == 0)
        {
            return 0.001m; // Mặc định
        }

        foreach (var id in cityIds)
        {
            switch (id)
            {
                case 0: // Hà Nội
                case 1: // Hồ Chí Minh
                case 2: // Hải Phòng
                case 3: // Cần Thơ
                case 4: // Đà Nẵng
                case 5: // Biên Hòa
                case 6: // Hải Dương
                case 7: // Huế
                case 8: // Thuận An
                case 9: // Thủ Đức
                    totalCoefficient += 0.005m;
                    break;
                default:
                    totalCoefficient += 0.001m;
                    break;
            }
        }

        return totalCoefficient;
    }

    // Hệ số thành phố có thiên tai - Chuyển sang int[]
    public decimal DisasterRiskCoefficient(int[] cityIds)
    {
        decimal totalCoefficient = 0m;

        if (cityIds == null || cityIds.Length == 0)
        {
            return 0.01m; // Mặc định
        }

        foreach (var id in cityIds)
        {
            switch (id)
            {
                case 0: // Lạng Sơn
                case 1: // Cao Bằng
                case 2: // Lào Cai
                case 3: // Yên Bái
                case 4: // Phú Thọ
                case 5: // Bắc Giang
                case 6: // Bắc Kạn
                case 7: // Thái Nguyên
                case 8: // Hoà Bình
                case 9: // Ninh Bình
                case 10: // Thanh Hoá
                    totalCoefficient += 0.05m;
                    break;
                default:
                    totalCoefficient += 0.01m;
                    break;
            }
        }

        return totalCoefficient;
    }

    // Hệ số tuổi tài sản - Giữ nguyên vì chỉ nhận một giá trị int
    public decimal AssetAgeRiskCoefficient(int assetAge)
    {
        if (assetAge <= 0)
        {
            return 0m;
        }
        else if (assetAge <= 50)
        {
            return 0.005m;
        }
        else
        {
            return 0.01m;
        }
    }


    // Hệ số nghề nghiệp - Chuyển sang int[]
    public decimal CareerCoefficient(int[] careerIds)
    {
        decimal totalCoefficient = 0m;

        if (careerIds == null || careerIds.Length == 0)
        {
            return 0.01m; // Mặc định
        }

        foreach (var id in careerIds)
        {
            switch (id)
            {
                case 0: // Cứu hoả
                case 1: // Phi công
                case 2: // Khai thác gỗ
                case 3: // Thu gom rác thải và tái chế
                case 4: // Thợ hàn dưới nước
                case 5: // Công nhân dầu khí
                case 6: // Công nhân xây dựng
                case 7: // Ngư dân vùng biển sâu
                case 8: // Thợ làm sắt thép
                case 9: // Người đấu bò
                case 10: // Thợ mỏ
                case 11: // Làm nông
                case 12: // Sĩ quan cảnh sát
                case 13: // Tài xế xe tải
                case 14: // Diễn viên đóng thế
                    totalCoefficient += 0.03m;
                    break;
                default:
                    totalCoefficient += 0.01m;
                    break;
            }
        }

        return totalCoefficient;
    }

    // Hệ số vật liệu xây dựng - Chuyển sang int[]
    public decimal ConstructionMaterialRiskCoefficient(int[] materialIds)
    {
        decimal totalCoefficient = 0m;

        if (materialIds == null || materialIds.Length == 0)
        {
            return 0.01m; // Mặc định
        }

        foreach (var id in materialIds)
        {
            switch (id)
            {
                case 0: // Gỗ
                    totalCoefficient += 0.03m;
                    break;
                default:
                    totalCoefficient += 0.01m;
                    break;
            }
        }

        return totalCoefficient;
    }

    // Hệ số rủi ro tội phạm - Chuyển sang int[]
    public decimal CrimeRiskCoefficient(int[] cityIds)
    {
        decimal totalCoefficient = 0m;

        if (cityIds == null || cityIds.Length == 0)
        {
            return 0.01m; // Mặc định
        }

        foreach (var id in cityIds)
        {
            switch (id)
            {
                case 0: // Buôn Ma Thuột
                    totalCoefficient += 0.02m;
                    break;
                default:
                    totalCoefficient += 0.01m;
                    break;
            }
        }

        return totalCoefficient;
    }
}
