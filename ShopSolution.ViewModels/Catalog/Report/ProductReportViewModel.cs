using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.ViewModels.Catalog.Report
{
    public class ProductReportViewModel
    {
        public string ProductName { get; set; }  // Tên sản phẩm
        public int QuantitySold { get; set; }   // Số lượng đã bán
        public decimal Revenue { get; set; }    // Doanh thu từ sản phẩm này
        public int StockRemaining { get; set; } // Tồn kho còn lại
    }
}
