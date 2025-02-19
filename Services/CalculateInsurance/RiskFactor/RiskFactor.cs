namespace Project_Sem3.Helper.RiskFactor;

public class RiskFactor
{
// Hàm tính RiskFactor cho Bảo hiểm xe (Auto Insurance)
    public decimal CalculateVehicleInsuranceRiskFactor(decimal ageCoefficient, decimal vehicleCoefficient, decimal accidentCoefficient,decimal locationCoefficient)
    {
        return ageCoefficient + vehicleCoefficient + locationCoefficient + accidentCoefficient;
    }

// Hàm tính RiskFactor cho Bảo hiểm nhân thọ (Life Insurance)
    public decimal CalculateLifeInsuranceRiskFactor(decimal ageCoefficient, decimal healthCoefficient, decimal careerCoefficient)
    {
        return ageCoefficient + healthCoefficient + careerCoefficient;
    }

// Hàm tính RiskFactor cho Bảo hiểm nhà (Property Insurance)
    public decimal CalculatePropertyInsuranceRiskFactor( decimal baseRick ,decimal disasterRiskCoefficient
        ,decimal assetAgeRiskCoefficient,   
        decimal constructionMaterialRiskCoefficient ,
        decimal crimeRiskCoefficient )
    {
        return baseRick + disasterRiskCoefficient + assetAgeRiskCoefficient + constructionMaterialRiskCoefficient + crimeRiskCoefficient;
    }

// Hàm tính RiskFactor cho Bảo hiểm y tế (Health Insurance)
    public decimal CalculateHealthInsuranceRiskFactor(decimal ageCoefficient, decimal healthCoefficient, decimal careerCoefficient , decimal lifestyleCoefficient)
    {
        return ageCoefficient + healthCoefficient + careerCoefficient + lifestyleCoefficient;
    }

}