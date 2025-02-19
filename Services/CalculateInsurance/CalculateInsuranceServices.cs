namespace Project_Sem3.Helper;

public class CalculateInsuranceServices
{
    public readonly CalculateCoefficient _calculateCoefficient;
    public readonly RiskFactor.RiskFactor _riskFactor;
    public readonly BaseRate.BaseRate _baseRate;
    public CalculateInsuranceServices(CalculateCoefficient calculateCoefficient , RiskFactor.RiskFactor riskFactor , BaseRate.BaseRate baseRate)
    {
        _calculateCoefficient = calculateCoefficient;
        _riskFactor = riskFactor;
        _baseRate = baseRate;
    }
    
    public (decimal,decimal,decimal,decimal,decimal) LifeInsurance(int age, string healthStatus, string career ,decimal coverageAmount , int contractDuration)
    {
        if (age <=  0 || healthStatus.Length <=0 || career.Length <= 0 || coverageAmount <=0 || contractDuration <=0 )
        {
            return (0m , 0m,0m , 0m,0m);
        }
        decimal ageCoefficient = _calculateCoefficient.ageCoefficient(age);
        decimal healthCoefficient = _calculateCoefficient.healthCoefficient(healthStatus);
        decimal careerCoefficient = _calculateCoefficient.careerCoefficient(career);
        decimal riskFactor  =
            _riskFactor.CalculateLifeInsuranceRiskFactor(ageCoefficient, healthCoefficient, careerCoefficient);
        decimal baseRateLife = _baseRate.BaseRateLife(age);
        decimal annualPaymentAmount = baseRateLife + (riskFactor  * coverageAmount);
        decimal deductible = annualPaymentAmount * 0.01m;
        decimal premium = annualPaymentAmount * contractDuration;
        return (annualPaymentAmount ,premium,deductible , coverageAmount * contractDuration ,riskFactor);       }
    
    public (decimal,decimal,decimal,decimal,decimal)HealthInsurance(int age, string healthStatus, string career,string lifestyle ,decimal coverageAmount , int contractDuration)
    {
        if (age <=  0 || healthStatus.Length <=0 || career.Length <= 0 || lifestyle.Length <= 0 || coverageAmount <=0 || contractDuration <=0 )
        {
            return (0m , 0m,0m , 0m,0m);
        }
        decimal baseRateHealth = _baseRate.BaseRateHealth();
        decimal ageCoefficient = _calculateCoefficient.ageCoefficient(age);
        decimal healthCoefficient = _calculateCoefficient.healthCoefficient(healthStatus);
        decimal careerCoefficient = _calculateCoefficient.careerCoefficient(career);
        decimal lifestyleCoefficient = _calculateCoefficient.lifestyleCoefficient(lifestyle);
        decimal riskFactor = _riskFactor.CalculateHealthInsuranceRiskFactor(ageCoefficient,healthCoefficient,careerCoefficient,lifestyleCoefficient);

        decimal annualPaymentAmount = baseRateHealth + ((riskFactor + healthCoefficient) * coverageAmount);
        decimal deductible = annualPaymentAmount * 0.01m;
        decimal premium = annualPaymentAmount * contractDuration;
        return (annualPaymentAmount ,premium,deductible , coverageAmount * contractDuration ,riskFactor);      
    }
    
    public (decimal, decimal, decimal, decimal,decimal) VehicleInsurance(int age ,string vehicleType ,
        string vehicleBrand ,string city ,
        int numberOfAccidents , int yearsWithoutAccident,
        decimal coverageAmount , int contractDuration)
    {
        if (age <= 0 || vehicleType.Length <= 0 || vehicleBrand.Length <= 0 || city.Length <= 0
            || numberOfAccidents <= 0 || yearsWithoutAccident <= 0 || coverageAmount <= 0 || contractDuration <= 0)
        {
            return (0m , 0m,0m , 0m,0m);
        }
        decimal baseRateVehicle = _baseRate.BaseRateVehicle();
        decimal ageCoefficient = _calculateCoefficient.ageCoefficient(age);
        decimal vehicleCoefficient = _calculateCoefficient.vehicleCoefficient(vehicleType, vehicleBrand);
        decimal locationCoefficient = _calculateCoefficient.locationCoefficient(city);
        decimal accidentCoefficient =
            _calculateCoefficient.accidentCoefficient(numberOfAccidents, yearsWithoutAccident);
        decimal riskFactor = 
            _riskFactor.CalculateVehicleInsuranceRiskFactor(ageCoefficient , vehicleCoefficient, accidentCoefficient
                ,locationCoefficient);
        // premium làm tròn đến số lớn hơn
        decimal annualPaymentAmount
            = decimal.Round(baseRateVehicle + (coverageAmount * riskFactor) * 0.9m , 1 ); 
        decimal deductible =decimal.Round((annualPaymentAmount / 0.9m),1) * 0.1m;
        decimal premium = annualPaymentAmount * contractDuration;
        return (annualPaymentAmount ,premium,deductible , coverageAmount * contractDuration ,riskFactor);        
    }
    
    // decimal homeInsuranceCost = HomeInsurance(2000000000, 0.005m, 1.2m, 1.5m);
    // Console.WriteLine($"Phí bảo hiểm nhà ở: {homeInsuranceCost:N0} VND");
    public (decimal, decimal, decimal, decimal , decimal) PropertyCoefficient(string houseType ,string city ,
        int assetAge , string material ,decimal coverageAmount , int contractDuration)
    {
        decimal baseRateProperty = _baseRate.BaseRateProperty();
        decimal disasterRiskCoefficient = _calculateCoefficient.disasterRiskCoefficient(city);
        decimal assetAgeRiskCoefficient = _calculateCoefficient.assetAgeRiskCoefficient(assetAge);
        decimal constructionMaterialRiskCoefficient =
            _calculateCoefficient.constructionMaterialRiskCoefficient(material);
        decimal crimeRiskCoefficient = _calculateCoefficient.crimeRiskCoefficient(city);
        decimal riskFactor = _riskFactor.CalculatePropertyInsuranceRiskFactor(0.01m, disasterRiskCoefficient,
            assetAgeRiskCoefficient, constructionMaterialRiskCoefficient, crimeRiskCoefficient);
        decimal deductiblePercentage;
        if (houseType.ToLower().Trim().Equals("Căn hộ"))
        {
            deductiblePercentage = 0.03m;
        }
        else if(houseType.ToLower().Trim().Equals("Thương mại"))
        {
            deductiblePercentage = 0.05m;
        }
        else
        {
            deductiblePercentage = 0.01m;
        }

        decimal annualPaymentAmount = decimal.Round((baseRateProperty + (coverageAmount * riskFactor)) * (1 - deductiblePercentage));
        decimal deductible =decimal.Round((annualPaymentAmount /(1 - deductiblePercentage)),1) * 0.1m;
        decimal premium = annualPaymentAmount * contractDuration;
        return (annualPaymentAmount ,premium,deductible , coverageAmount * contractDuration ,riskFactor);       
    }
}