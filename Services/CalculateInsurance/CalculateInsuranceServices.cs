namespace Project_Sem3.Helper;

public class CalculateInsuranceServices
{
    public readonly CalculateCoefficient _CalculateCoefficient;

    public CalculateInsuranceServices(CalculateCoefficient calculateCoefficient)
    {
        _CalculateCoefficient = calculateCoefficient;
    }
    // ví dụ  : 
    // decimal insuranceAmount = 500000000; // 500 triệu VND
    // decimal basicFee = 0.005m; // 0.5%
    // decimal ageCoefficient = 1.2m; // 40 tuổi
    // decimal healthCoefficient = 1.3m; // Tiểu đường
    // decimal insuranceCost = LifeInsurance(insuranceAmount, basicFee, ageCoefficient, healthCoefficient);
    public decimal LifeInsurance(decimal insuranceAmount, decimal basicFee,int age,
        string healthStatus , int contractDuration)
    {
        if (insuranceAmount <= 0 || basicFee <= 0 || age <= 0 || healthStatus.Length <= 0)
        {
            throw new ArgumentException("Các giá trị đầu vào phải lớn hơn 0.");
        }
        decimal ageCoefficient = _CalculateCoefficient.ageCoefficient(age);
        decimal healthCoefficient = _CalculateCoefficient.healthCoefficient(healthStatus);
        decimal contractCoefficient = _CalculateCoefficient.contractCoefficient(contractDuration);
        return insuranceAmount * basicFee * ageCoefficient * healthCoefficient * contractCoefficient;
    }
    
    // decimal healthInsuranceCost = HealthInsurance(100000000, 0.002m, 1.1m, 1.2m);
    // Console.WriteLine($"Phí bảo hiểm y tế: {healthInsuranceCost:N0} VND");
    public decimal HealthInsurance(decimal insuranceAmount, decimal baseRate,int age,
        string healthStatus ,int contractDuration)
    {
        if (insuranceAmount <= 0 || baseRate <= 0 || age <= 0 || healthStatus.Length <= 0)
        {
            throw new ArgumentException("Các giá trị đầu vào phải lớn hơn 0.");
        }
        decimal ageCoefficient = _CalculateCoefficient.ageCoefficient(age);
        decimal healthCoefficient = _CalculateCoefficient.healthCoefficient(healthStatus);
        decimal contractCoefficient = _CalculateCoefficient.contractCoefficient(contractDuration);
        return insuranceAmount * baseRate * ageCoefficient * healthCoefficient * contractCoefficient;
    }
    
    // decimal autoInsuranceCost = AutoInsurance(500000000, 0.01m, 1.05m, 1.2m);
    // Console.WriteLine($"Phí bảo hiểm xe cơ giới: {autoInsuranceCost:N0} VND");
    public decimal AutoInsurance(decimal carValue, decimal baseRate , string carBarand, int numberOfAccidents
        , int yearsWithoutAccident , int contractDuration)
    {
        if (carValue <= 0 || baseRate <= 0 || carBarand.Length <= 0 || numberOfAccidents < 0|| yearsWithoutAccident <0)
        {
            throw new ArgumentException("Các giá trị đầu vào phải lớn hơn 0.");
        }

        decimal carTypeCoefficient = _CalculateCoefficient.carTypeCoefficient(carBarand);
        decimal accidentCoefficient =
            _CalculateCoefficient.accidentCoefficient(numberOfAccidents, yearsWithoutAccident);
        decimal contractCoefficient = _CalculateCoefficient.contractCoefficient(contractDuration);

        return carValue * baseRate * carTypeCoefficient * accidentCoefficient * contractCoefficient;
    }
    
    // decimal homeInsuranceCost = HomeInsurance(2000000000, 0.005m, 1.2m, 1.5m);
    // Console.WriteLine($"Phí bảo hiểm nhà ở: {homeInsuranceCost:N0} VND");
    public decimal HomeInsurance(decimal homeValue, decimal baseRate, string city,int contractDuration)
    {
        if (homeValue <= 0 || baseRate <= 0 || city.Length <= 0)
        {
            throw new ArgumentException("Các giá trị đầu vào phải lớn hơn 0.");
        }

        decimal locationCoefficient = _CalculateCoefficient.locationCoefficient(city);
        decimal disasterRiskCoefficient = _CalculateCoefficient.disasterRiskCoefficient(city);
        decimal contractCoefficient = _CalculateCoefficient.contractCoefficient(contractDuration);

        return homeValue * baseRate * locationCoefficient * disasterRiskCoefficient * contractCoefficient;
    }
}