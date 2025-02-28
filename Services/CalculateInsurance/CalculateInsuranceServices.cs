namespace Project_Sem3.Helper;

public class CalculateInsuranceServices
{
    public readonly CalculateCoefficient _calculateCoefficient;
    public readonly RiskFactor.RiskFactor _riskFactor;
    public readonly BaseRate.BaseRate _baseRate;

    public CalculateInsuranceServices(CalculateCoefficient calculateCoefficient, RiskFactor.RiskFactor riskFactor, BaseRate.BaseRate baseRate)
    {
        _calculateCoefficient = calculateCoefficient;
        _riskFactor = riskFactor;
        _baseRate = baseRate;
    }

    // Bảo hiểm nhân thọ - Đã dùng int[] cho healthStatusIds và careerIds, chỉ cần sửa điều kiện kiểm tra
    public (decimal, decimal, decimal, decimal, decimal) LifeInsurance(int age, int[] healthStatusIds, int[] careerIds, decimal coverageAmount, int contractDuration)
    {
        if (age <= 0 || healthStatusIds == null || healthStatusIds.Length <= 0 || careerIds == null || careerIds.Length <= 0 || coverageAmount <= 0 || contractDuration <= 0)
        {
            return (0m, 0m, 0m, 0m, 0m);
        }
        decimal ageCoefficient = _calculateCoefficient.AgeCoefficient(age);
        decimal healthCoefficient = _calculateCoefficient.HealthCoefficient(healthStatusIds);
        decimal careerCoefficient = _calculateCoefficient.CareerCoefficient(careerIds);
        decimal riskFactor = _riskFactor.CalculateLifeInsuranceRiskFactor(ageCoefficient, healthCoefficient, careerCoefficient);
        if (riskFactor > 0.1m) riskFactor = 0.1m;
        decimal baseRateLife = _baseRate.BaseRateLife(age);
        decimal annualPaymentAmount = baseRateLife + (riskFactor * coverageAmount);
        decimal deductible = annualPaymentAmount * 0.01m;
        decimal premium = annualPaymentAmount * contractDuration;
        return (annualPaymentAmount, premium, deductible, coverageAmount, riskFactor);
    }

    // Bảo hiểm sức khỏe - Chuyển healthStatus và career sang int[], giữ lifestyleIds
    public (decimal, decimal, decimal, decimal, decimal) HealthInsurance(int age, int[] healthStatusIds, int[] careerIds, int[] lifestyleIds, decimal coverageAmount, int contractDuration)
    {
        if (age <= 0 || healthStatusIds == null || healthStatusIds.Length <= 0 || careerIds == null || careerIds.Length <= 0 || lifestyleIds == null || lifestyleIds.Length <= 0 || coverageAmount <= 0 || contractDuration <= 0)
        {
            return (0m, 0m, 0m, 0m, 0m);
        }

        decimal baseRateHealth = _baseRate.BaseRateHealth();
        decimal ageCoefficient = _calculateCoefficient.AgeCoefficient(age); // Sửa lỗi cú pháp: ageCoefficient
        decimal healthCoefficient = _calculateCoefficient.HealthCoefficient(healthStatusIds);
        decimal careerCoefficient = _calculateCoefficient.CareerCoefficient(careerIds);
        decimal lifestyleCoefficient = _calculateCoefficient.LifestyleCoefficient(lifestyleIds);
        decimal riskFactor = _riskFactor.CalculateHealthInsuranceRiskFactor(ageCoefficient, healthCoefficient, careerCoefficient, lifestyleCoefficient);
        if (riskFactor > 0.1m) riskFactor = 0.1m; // Đặt mức tối đa cho rủi ro cao
        decimal annualPaymentAmount = baseRateHealth + ((riskFactor + healthCoefficient) * coverageAmount);
        decimal deductible = annualPaymentAmount * 0.01m;
        decimal premium = annualPaymentAmount * contractDuration;
        return (annualPaymentAmount, premium, deductible, coverageAmount, riskFactor);
    }

    // Bảo hiểm xe cộ - Chuyển vehicleType, vehicleBrand, city sang int[]
    public (decimal, decimal, decimal, decimal, decimal) VehicleInsurance(int age, int[] vehicleInfo, int[] cityIds, int numberOfAccidents, int yearsWithoutAccident, decimal coverageAmount, int contractDuration)
    {
        if (age <= 0 || vehicleInfo == null || vehicleInfo.Length < 2 || cityIds == null || cityIds.Length <= 0 || numberOfAccidents < 0 || yearsWithoutAccident < 0 || coverageAmount <= 0 || contractDuration <= 0)
        {
            return (0m, 0m, 0m, 0m, 0m);
        }

        decimal baseRateVehicle = _baseRate.BaseRateVehicle();
        decimal ageCoefficient = _calculateCoefficient.AgeCoefficient(age);
        decimal vehicleCoefficient = _calculateCoefficient.VehicleCoefficient(vehicleInfo); // vehicleInfo chứa [vehicleType, brandId]
        decimal locationCoefficient = _calculateCoefficient.LocationCoefficient(cityIds);
        decimal accidentCoefficient = _calculateCoefficient.AccidentCoefficient(numberOfAccidents, yearsWithoutAccident);
        decimal riskFactor = _riskFactor.CalculateVehicleInsuranceRiskFactor(ageCoefficient, vehicleCoefficient, accidentCoefficient, locationCoefficient);
        if (riskFactor < 0.075m)
        {
          riskFactor = 0.075m;
        }
        else
        {
          riskFactor = 0.08m;
        }
        decimal annualPaymentAmount = decimal.Round(baseRateVehicle + (coverageAmount * riskFactor) * 0.9m, 1);
        decimal deductible = decimal.Round((annualPaymentAmount / 0.9m), 1) * 0.1m;
        decimal premium = annualPaymentAmount * contractDuration;
        return (annualPaymentAmount, premium, deductible, coverageAmount, riskFactor);
    }

    // Bảo hiểm tài sản - Chuyển houseType, city, material sang int[]
    public (decimal, decimal, decimal, decimal, decimal) PropertyInsurance(int[] houseTypeIds, int[] cityIds, int assetAge, int[] materialIds, decimal coverageAmount, int contractDuration)
    {
        if (houseTypeIds == null || houseTypeIds.Length <= 0 || cityIds == null || cityIds.Length <= 0 || assetAge < 0 || materialIds == null || materialIds.Length <= 0 || coverageAmount <= 0 || contractDuration <= 0)
        {
            return (0m, 0m, 0m, 0m, 0m);
        }

        decimal baseRateProperty = _baseRate.BaseRateProperty();
        decimal disasterRiskCoefficient = _calculateCoefficient.DisasterRiskCoefficient(cityIds);
        decimal assetAgeRiskCoefficient = _calculateCoefficient.AssetAgeRiskCoefficient(assetAge);
        decimal constructionMaterialRiskCoefficient = _calculateCoefficient.ConstructionMaterialRiskCoefficient(materialIds);
        decimal crimeRiskCoefficient = _calculateCoefficient.CrimeRiskCoefficient(cityIds);
        decimal riskFactor = _riskFactor.CalculatePropertyInsuranceRiskFactor(0.01m, disasterRiskCoefficient, assetAgeRiskCoefficient, constructionMaterialRiskCoefficient, crimeRiskCoefficient);

        decimal deductiblePercentage;
        int houseTypeId = houseTypeIds[0]; // Lấy giá trị đầu tiên trong mảng
        if (houseTypeId == 0) // Căn hộ
        {
            deductiblePercentage = 0.03m;
        }
        else if (houseTypeId == 1) // Thương mại
        {
            deductiblePercentage = 0.05m;
        }
        else // Nhà ở thông thường hoặc khác
        {
            deductiblePercentage = 0.01m;
        }

        decimal annualPaymentAmount = decimal.Round((baseRateProperty + (coverageAmount * riskFactor)) * (1 - deductiblePercentage));
        decimal deductible = decimal.Round((annualPaymentAmount / (1 - deductiblePercentage)), 1) * 0.1m;
        decimal premium = annualPaymentAmount * contractDuration;
        return (annualPaymentAmount, premium, deductible, coverageAmount, riskFactor);
    }
}
