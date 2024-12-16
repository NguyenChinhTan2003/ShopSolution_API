using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.ViewModels.Catalog.Report
{
    public class DailyReportViewModel
    {
        public DateTime ReportDate { get; set; }
        public decimal TotalRevenue { get; set; } // Tổng doanh thu trong ngày
        public int TotalOrders { get; set; }     // Tổng số đơn hàng thành công
        public int TotalProductsSold { get; set; } // Tổng số sản phẩm bán ra
        public List<ProductReportViewModel> ProductReports { get; set; } // Báo cáo chi tiết theo sản phẩm

        public PaymentReportViewModel VNPayReport { get; set; } // Thống kê cho VNPay
        public PaymentReportViewModel CodReport { get; set; }   // Thống kê cho COD

        public List<OrderStatusReportViewModel> OrderStatusReports { get; set; } // Thống kê hóa đơn theo trạng thái
    }
}
