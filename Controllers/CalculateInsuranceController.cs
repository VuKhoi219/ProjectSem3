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
                var result = _calculateInsuranceServices.LifeInsurance(request.InsuranceAmount, 0.05m, request.Age, request.HealthStatus , request.ContractDuration);
                return Ok(new { TotalCost = result });
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
                var result = _calculateInsuranceServices.HealthInsurance(request.InsuranceAmount, 0.05m, request.Age, request.HealthStatus, request.ContractDuration);
                return Ok(new { TotalCost = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("auto")]
        public IActionResult CalculateAutoInsurance([FromBody] AutoInsuranceRequest request)
        {
            try
            {
                var result = _calculateInsuranceServices.AutoInsurance(request.CarValue,0.05m, request.CarBrand, request.NumberOfAccidents, request.YearsWithoutAccident, request.ContractDuration);
                return Ok(new { TotalCost = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("home")]
        public IActionResult CalculateHomeInsurance([FromBody] HomeInsuranceRequest request)
        {
            try
            {
                var result = _calculateInsuranceServices.HomeInsurance(request.HomeValue, 0.05m, request.City , request.ContractDuration);
                return Ok(new { TotalCost = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
}