using System;
using Microsoft.AspNetCore.Mvc;
using Project_Sem3.Helper;
using Project_Sem3.Models.Request;

namespace Project_Sem3.Controllers;

[ApiController]
[Route("api/insurance/calculate")]
public class CalculateInsuranceController : Controller
{
    private readonly CalculateInsuranceServices _calculateInsuranceServices;

    public CalculateInsuranceController(CalculateInsuranceServices calculateInsuranceServices)
    {
        _calculateInsuranceServices = calculateInsuranceServices;
    }

    [HttpPost("life")]
    public IActionResult CalculateLifeInsurance([FromBody] LifeInsuranceRequest request)
    {
        try
        {
            var result = _calculateInsuranceServices.LifeInsurance(
                request.Age,
                request.HealthStatusIds, // Đã là int[]
                request.CareerIds,      // Đổi từ Career sang CareerIds
                request.CoverageAmount,
                request.ContractDuration
            );
            return Ok(new
            {
                annualPaymentAmount = result.Item1,
                premium = result.Item2,
                deductible = result.Item3,
                coverageAmount = result.Item4,
                riskFactor = result.Item5
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("health")]
    public IActionResult CalculateHealthInsurance([FromBody] HealthInsuranceRequest request)
    {
        try
        {
            var result = _calculateInsuranceServices.HealthInsurance(
                request.Age,
                request.HealthStatusIds, // Đã là int[]
                request.CareerIds,       // Đổi từ Career sang CareerIds
                request.LifestyleIds,    // Đã là int[]
                request.CoverageAmount,
                request.ContractDuration
            );
            return Ok(new
            {
                annualPaymentAmount = result.Item1,
                premium = result.Item2,
                deductible = result.Item3,
                coverageAmount = result.Item4,
                riskFactor = result.Item5
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("vehicle")]
    public IActionResult CalculateVehicleInsurance([FromBody] VehicleInsuranceRequest request)
    {
        try
        {
            var result = _calculateInsuranceServices.VehicleInsurance(
                request.Age,
                request.VehicleInfo,     // Đổi từ VehicleType và VehicleBrand sang int[] VehicleInfo
                request.CityIds,         // Đổi từ City sang CityIds
                request.NumberOfAccidents,
                request.YearsWithoutAccident,
                request.CoverageAmount,
                request.ContractDuration
            );
            return Ok(new
            {
                annualPaymentAmount = result.Item1,
                premium = result.Item2,
                deductible = result.Item3,
                coverageAmount = result.Item4,
                riskFactor = result.Item5
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("property")]
    public IActionResult CalculatePropertyInsurance([FromBody] HomeInsuranceRequest request)
    {
        try
        {
            var result = _calculateInsuranceServices.PropertyInsurance( // Sửa tên phương thức từ PropertyCoefficient sang PropertyInsurance
                request.HouseTypeIds,    // Đổi từ HouseType sang HouseTypeIds
                request.CityIds,         // Đổi từ City sang CityIds
                request.AssetAge,
                request.MaterialIds,     // Đổi từ Material sang MaterialIds
                request.CoverageAmount,
                request.ContractDuration
            );
            return Ok(new
            {
                annualPaymentAmount = result.Item1,
                premium = result.Item2,
                deductible = result.Item3,
                coverageAmount = result.Item4,
                riskFactor = result.Item5
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
