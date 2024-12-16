using ShopSolution.ViewModels.Catalog.Products;
using ShopSolution.ViewModels.Catalog.Report;

namespace ShopSolution.BackendApi.Services
{
    public interface IJobService
    {
        Task<DailyReportViewModel> GetDailyReportAsync(DateTime reportDate);
        Task ProcessAndSendDailyReportAsync(DateTime reportDate, string email);

        Task ProcessAndSendMonthlyReportAsync(int month, int year, string email);

        Task UpdateIsFeatured();

    }
}
