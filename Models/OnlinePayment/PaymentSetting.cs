namespace Project_Sem3.Models.MyBank;

public class PaymentSetting
{
    //tên ngân hàng
    public string BankId { get; set;}
    //số tài khoản
    public string AccountNo { get; set;}
    public string QrUrlTemplate { get; set; }
}