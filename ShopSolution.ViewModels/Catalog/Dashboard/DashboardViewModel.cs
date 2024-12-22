using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.ViewModels.Common
{
    public class DashboardViewModel
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal RevenueToday { get; set; }
        public decimal RevenueThisMonth { get; set; }

        public List<decimal> RevenueData { get; set; } // Dữ liệu doanh thu
        public List<string> RevenueLabels { get; set; } // Nhãn thời gian (ví dụ: ngày, tháng)
        public List<decimal> RevenueDataByMonth { get; set; } // Dữ liệu doanh thu theo tháng
        public List<string> RevenueLabelsByMonth { get; set; } // Nhãn thời gian theo tháng
    }
}