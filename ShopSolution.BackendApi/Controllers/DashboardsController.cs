using Microsoft.AspNetCore.Mvc;
using ShopSolution.Application.Statistics;
using System.Threading.Tasks;

namespace ShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardsController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardsController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var dashboardData = await _dashboardService.GetDashboardData();
            return Ok(dashboardData);
        }
    }
}