using Microsoft.AspNetCore.Mvc;
using ShopSolution.ApiIntegration;
using System.Threading.Tasks;

namespace ShopSolution.AdminApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardApiClient _dashboardApiClient;

        public DashboardController(IDashboardApiClient dashboardApiClient)
        {
            _dashboardApiClient = dashboardApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _dashboardApiClient.GetDashboardData();
            return View(viewModel);
        }
    }
}