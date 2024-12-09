using ShopSolution.WebApp.Models.VnPay;

namespace ShopSolution.WebApp.Service
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message); //hàm gửi email
    }
}
