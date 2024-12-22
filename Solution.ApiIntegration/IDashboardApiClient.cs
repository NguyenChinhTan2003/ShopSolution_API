using ShopSolution.ViewModels.Common;
using System.Threading.Tasks;

namespace ShopSolution.ApiIntegration
{
    public interface IDashboardApiClient
    {
        Task<DashboardViewModel> GetDashboardData();
    }
}