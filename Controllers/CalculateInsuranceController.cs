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
                var result = _calculateInsuranceServices.LifeInsurance(request.Age,request.HealthStatus,request.Career,request.CoverageAmount,request.ContractDuration);
                return Ok(new { TotalCost = result.Item1 , deductible = result.Item2 ,  coverageAmount = result.Item3 , riskFactor = result.Item4  });
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
                var result = _calculateInsuranceServices.HealthInsurance(request.Age, request.HealthStatus,
                    request.Career, request.Lifestyle, request.CoverageAmount, request.ContractDuration);
                return Ok(new { TotalCost = result.Item1 , deductible = result.Item2 ,  coverageAmount = result.Item3 , riskFactor = result.Item4  });
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
                var result = _calculateInsuranceServices.VehicleInsurance(request.Age, request.VehicleType,
                    request.VehicleBrand, request.City, request.NumberOfAccidents, request.YearsWithoutAccident,
                    request.CoverageAmount, request.ContractDuration);
                return Ok(new { TotalCost = result.Item1 , deductible = result.Item2 ,  coverageAmount = result.Item3 , riskFactor = result.Item4  });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });

            }
        }

        [HttpPost("home")]
        public IActionResult CalculatePropertyInsurance([FromBody] HomeInsuranceRequest request)
        {
            try
            {
                var result = _calculateInsuranceServices.PropertyCoefficient(request.HouseType, request.City,
                    request.AssetAge, request.Material, request.CoverageAmount, request.ContractDuration);
                return Ok(new { TotalCost = result.Item1 , deductible = result.Item2 ,  coverageAmount = result.Item3 , riskFactor = result.Item4  });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
}