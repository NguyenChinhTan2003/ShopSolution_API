using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.ViewModels.Catalog.Report
{
    public class MonthlyReportViewModel
    {
        public int Month { get; set; }        // Tháng của báo cáo
        public int Year { get; set; }         // Năm của báo cáo
        public decimal TotalRevenue { get; set; } // Tổng doanh thu trong tháng
        public int TotalOrders { get; set; }     // Tổng số đơn hàng thành công
        public int TotalProductsSold { get; set; } // Tổng số sản phẩm bán ra
        public List<ProductReportViewModel> ProductReports { get; set; } // Báo cáo chi tiết theo sản phẩm
        public PaymentReportViewModel VNPayReport { get; set; }          // Thống kê VNPay
        public PaymentReportViewModel CodReport { get; set; }            // Thống kê COD
        public List<OrderStatusReportViewModel> OrderStatusReports { get; set; } // Báo cáo theo trạng thái đơn hàng
    }

}
