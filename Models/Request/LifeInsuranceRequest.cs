namespace Project_Sem3.Models.Request;

public class LifeInsuranceRequest
{
    // Số tiền bảo hiểm chi trả
    public decimal CoverageAmount { get; set; }

    // Tuổi của người tham gia bảo hiểm
    public int Age { get; set; }

    // Tình trạng sức khỏe của người tham gia bảo hiểm
    public string HealthStatus { get; set; }

    // Nghề nghiệp của người tham gia bảo hiểm
    public string Career { get; set; }

    // Thời gian hợp đồng bảo hiểm (Thaáng)
    public int ContractDuration { get; set; }

}