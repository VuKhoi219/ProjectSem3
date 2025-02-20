using Microsoft.Extensions.Options;
using Project_Sem3.Models.MyBank;

namespace Project_Sem3.Services;

public class OnlinePaymentServices
{
    private readonly PaymentSetting _paymentSetting;

    public OnlinePaymentServices(IOptions<PaymentSetting> paymentSetting)
    {
        _paymentSetting = paymentSetting.Value;
    }

    public string GenerateQrUrl(decimal amount, string value)
    {
        string qrUrl = string.Format(
            _paymentSetting.QrUrlTemplate,
            _paymentSetting.BankId,
            _paymentSetting.AccountNo,
            amount,
            Uri.EscapeDataString(value)  // Đảm bảo mã hóa các ký tự đặc biệt trong productName
        );
        return qrUrl;
    }
}