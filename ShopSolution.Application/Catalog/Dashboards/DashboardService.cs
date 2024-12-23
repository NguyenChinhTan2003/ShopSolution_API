using Microsoft.EntityFrameworkCore;
using ShopSolution.Data.EF;
using ShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.Application.Statistics
{
    public class DashboardService : IDashboardService
    {
        private readonly ShopDBContext _context;

        public DashboardService(ShopDBContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardData()
        {
            var today = DateTime.Today;
            var thisMonth = new DateTime(today.Year, today.Month, 1);

            var totalOrders = await _context.Orders.CountAsync();
            var totalRevenue = await _context.OrderDetails.SumAsync(od => od.Quantity * od.Price);

            var revenueToday = await _context.OrderDetails
                .Where(od => od.Order.OrderDate.Date == today)
                .SumAsync(od => od.Quantity * od.Price + (decimal)od.Order.shippingCost);

            var revenueThisMonth = await _context.OrderDetails
                .Where(od => od.Order.OrderDate >= thisMonth)
                .SumAsync(od => od.Quantity * od.Price + (decimal)od.Order.shippingCost);

            // Lấy doanh thu theo ngày trong 7 ngày gần nhất
            var revenueData = new List<decimal>();
            var revenueLabels = new List<string>();
            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.Today.AddDays(-i);
                var revenue = await _context.OrderDetails
                    .Where(od => od.Order.OrderDate.Date == date)
                    .SumAsync(od => od.Quantity * od.Price );
                revenueData.Add(revenue);
                revenueLabels.Add(date.ToString("dd/MM"));
            }

            // Lấy doanh thu theo tháng trong 12 tháng gần nhất
            var revenueDataByMonth = new List<decimal>();
            var revenueLabelsByMonth = new List<string>();
            for (int i = 11; i >= 0; i--)
            {
                var month = DateTime.Today.AddMonths(-i);
                var firstDayOfMonth = new DateTime(month.Year, month.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                var revenue = await _context.OrderDetails
                    .Where(od => od.Order.OrderDate.Date >= firstDayOfMonth && od.Order.OrderDate.Date <= lastDayOfMonth)
                    .SumAsync(od => od.Quantity * od.Price);

                revenueDataByMonth.Add(revenue);
                revenueLabelsByMonth.Add(month.ToString("MM/yyyy"));
            }

            return new DashboardViewModel
            {
                // ... other data
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                RevenueToday = revenueToday,
                RevenueThisMonth = revenueThisMonth,
                RevenueData = revenueData,
                RevenueLabels = revenueLabels,
                RevenueDataByMonth = revenueDataByMonth,
                RevenueLabelsByMonth = revenueLabelsByMonth
            };
        }
    }
}