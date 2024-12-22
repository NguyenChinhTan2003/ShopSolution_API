using ShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.Application.Statistics
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardData();
    }
}