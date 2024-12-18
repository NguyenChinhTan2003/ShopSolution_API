using ShopSolution.WebApp.Library;
using ShopSolution.WebApp.Models.VnPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.ApiIntegration.VnPay
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} - {model.Address} - Email: {model.Email} - SĐT: {model.Phone}");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;
        }


        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            var orderInfo = response.OrderDescription;

            // Khởi tạo các biến
            string address = "";
            string email = "";
            string phone = "";

            // Tách thông tin bằng dấu "- "
            var parts = orderInfo?.Split(" - ");

            if (parts?.Length > 0)
            {
                // Lấy địa chỉ từ phần tử thứ hai đến cuối (trước email và sdt nếu có)
                address = string.Join(" - ", parts.Skip(1).Take(parts.Length - 3)).Trim();

                // Lấy email
                var emailPart = parts.LastOrDefault(p => p.StartsWith("Email: "));
                email = emailPart?.Replace("Email: ", "").Trim() ?? "";


                // Lấy số điện thoại
                var phonePart = parts.LastOrDefault(p => p.StartsWith("SĐT: "));
                phone = phonePart?.Replace("SĐT: ", "").Trim() ?? "";
            }

            response.Address = address;
            response.Email = email;
            response.Phone = phone;


            return response;
        }

    }
}
