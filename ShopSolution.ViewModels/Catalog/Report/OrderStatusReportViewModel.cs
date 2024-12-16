using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.ViewModels.Catalog.Report
{
    public class OrderStatusReportViewModel
    {
        public string Status { get; set; } // Trạng thái đơn hàng (Confirmed, Success, InProgress, Canceled)
        public int OrderCount { get; set; } // Số lượng hóa đơn ở trạng thái này
    }
}
