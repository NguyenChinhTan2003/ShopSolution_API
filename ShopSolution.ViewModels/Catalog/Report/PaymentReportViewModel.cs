using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.ViewModels.Catalog.Report
{
    public class PaymentReportViewModel
    {
        public decimal Revenue { get; set; } // Doanh thu theo phương thức thanh toán
        public int TotalOrders { get; set; } // Tổng số đơn hàng
    }
}
