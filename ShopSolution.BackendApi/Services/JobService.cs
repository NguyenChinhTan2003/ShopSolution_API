using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ShopSolution.Application.Common;
using ShopSolution.Data.EF;
using ShopSolution.Data.Enums;
using ShopSolution.Utilities.Exceptions;
using ShopSolution.ViewModels.Catalog.Products;
using ShopSolution.ViewModels.Catalog.Report;
using System.Text;
using System.Text.RegularExpressions;

namespace ShopSolution.BackendApi.Services
{
    public class JobService : IJobService
    {
        private readonly ShopDBContext _context;

        public JobService(ShopDBContext context)
        {
            _context = context;
        }

        // Báo cáo theo từng ngày
        public async Task<DailyReportViewModel> GetDailyReportAsync(DateTime reportDate)
        {
            // Lấy danh sách đơn hàng trong ngày
            var orders = await _context.Orders
                .Where(o => o.OrderDate.Date == reportDate.Date)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.ProductTranslations)
                .ToListAsync();

            // Tổng doanh thu và sản phẩm bán ra
            var successfulOrders = orders
                .Where(o => o.Status == OrderStatus.Success || o.Status == OrderStatus.Confirmed)
                .ToList();

            var totalRevenue = successfulOrders
                .SelectMany(o => o.OrderDetails)
                .Sum(od => od.Price * od.Quantity);

            var totalProductsSold = successfulOrders
                .SelectMany(o => o.OrderDetails)
                .Sum(od => od.Quantity);

            var totalOrders = successfulOrders.Count;


            // Báo cáo chi tiết sản phẩm
            var productReports = successfulOrders
                .SelectMany(o => o.OrderDetails)
                .GroupBy(od => od.ProductId)
                .Select(g => new ProductReportViewModel
                {
                    ProductName = g.FirstOrDefault()?.Product?.ProductTranslations
                                        .FirstOrDefault(pt => pt.LanguageId == "vi")?.Name ?? "Unknown",
                    QuantitySold = g.Sum(od => od.Quantity),
                    Revenue = g.Sum(od => od.Price * od.Quantity),
                    StockRemaining = g.FirstOrDefault()?.Product?.Stock ?? 0
                })
                .ToList();

            // Thống kê VNPay
            var vnPayOrders = successfulOrders.Where(o => o.PaymentMethod.StartsWith("VnPay"));
            var vnPayRevenue = vnPayOrders
                .SelectMany(o => o.OrderDetails)
                .Sum(od => od.Price * od.Quantity);

            var vnPayReport = new PaymentReportViewModel
            {
                Revenue = vnPayRevenue,
                TotalOrders = vnPayOrders.Count()
            };

            // Thống kê COD
            var codOrders = successfulOrders.Where(o => o.PaymentMethod == "COD");
            var codRevenue = codOrders
                .SelectMany(o => o.OrderDetails)
                .Sum(od => od.Price * od.Quantity);

            var codReport = new PaymentReportViewModel
            {
                Revenue = codRevenue,
                TotalOrders = codOrders.Count()
            };

            // Thống kê số lượng hóa đơn theo trạng thái
            var orderStatusReports = orders
                .GroupBy(o => o.Status)
                .Select(g => new OrderStatusReportViewModel
                {
                    Status = g.Key.ToString(),
                    OrderCount = g.Count()
                })
                .ToList();

            // Tạo ViewModel
            return new DailyReportViewModel
            {
                ReportDate = reportDate,
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalProductsSold = totalProductsSold,
                ProductReports = productReports,
                VNPayReport = vnPayReport,
                CodReport = codReport,
                OrderStatusReports = orderStatusReports
            };
        }



        public async Task ProcessAndSendDailyReportAsync(DateTime reportDate, string email)
        {
            // Gọi hàm lấy báo cáo
            var report = await GetDailyReportAsync(reportDate);

            // Tạo nội dung email từ báo cáo
            var emailContent = GenerateDailyReportHtml(report);

            // Gửi email đến địa chỉ được chỉ định
            var emailSender = new EmailSender(_context.GetService<IConfiguration>());
            await emailSender.SendEmailAsync(email, $"Báo cáo ngày {reportDate:dd/MM/yyyy}", emailContent);
        }


        private string GenerateDailyReportHtml(DailyReportViewModel report)
        {
            var sb = new StringBuilder();

            sb.Append($"<h1>Báo cáo cuối ngày - {report.ReportDate:dd/MM/yyyy}</h1>");
            sb.Append($"<p><strong>Tổng doanh thu:</strong> {report.TotalRevenue.ToString("#,##0 VNĐ")}</p>");
            sb.Append($"<p><strong>Tổng số đơn hàng:</strong> {report.TotalOrders}</p>");
            sb.Append($"<p><strong>Tổng số sản phẩm bán ra:</strong> {report.TotalProductsSold}</p>");

            // Thống kê VNPay
            sb.Append("<h2>Thống kê phương thức thanh toán - VNPay</h2>");
            sb.Append($"<p><strong>Doanh thu:</strong> {report.VNPayReport.Revenue.ToString("#,##0 VNĐ")}</p>");
            sb.Append($"<p><strong>Số đơn hàng:</strong> {report.VNPayReport.TotalOrders}</p>");

            // Thống kê COD
            sb.Append("<h2>Thống kê phương thức thanh toán - COD</h2>");
            sb.Append($"<p><strong>Doanh thu:</strong> {report.CodReport.Revenue.ToString("#,##0 VNĐ")}</p>");
            sb.Append($"<p><strong>Số đơn hàng:</strong> {report.CodReport.TotalOrders}</p>");

            // Thống kê hóa đơn theo trạng thái
            sb.Append("<h2>Thống kê hóa đơn theo trạng thái</h2>");
            sb.Append("<table border='1' style='border-collapse:collapse;width:100%;text-align:left;'>");
            sb.Append("<thead>");
            sb.Append("<tr><th>Trạng thái</th><th>Số lượng đơn hàng</th></tr>");
            sb.Append("</thead><tbody>");

            if (report.OrderStatusReports.Any())
            {
                foreach (var statusReport in report.OrderStatusReports)
                {
                    sb.Append($"<tr><td>{statusReport.Status}</td><td>{statusReport.OrderCount}</td></tr>");
                }
            }
            else
            {
                sb.Append("<tr><td colspan='2'>Không có dữ liệu.</td></tr>");
            }

            sb.Append("</tbody></table>");

            // Chi tiết sản phẩm
            sb.Append("<h2>Chi tiết sản phẩm</h2>");
            sb.Append("<table border='1' style='border-collapse:collapse;width:100%;text-align:left;'>");
            sb.Append("<thead>");
            sb.Append("<tr><th>#</th><th>Sản phẩm</th><th>Số lượng bán</th><th>Doanh thu</th><th>Tồn kho</th></tr>");
            sb.Append("</thead><tbody>");

            if (report.ProductReports.Any())
            {
                int index = 1;
                foreach (var product in report.ProductReports)
                {
                    sb.Append($"<tr><td>{index++}</td><td>{product.ProductName}</td><td>{product.QuantitySold}</td><td>{product.Revenue.ToString("#,##0 VNĐ")}</td><td>{product.StockRemaining}</td></tr>");
                }
            }
            else
            {
                sb.Append("<tr><td colspan='5'>Không có dữ liệu.</td></tr>");
            }

            sb.Append("</tbody></table>");

            return sb.ToString();
        }

        // Báo cáo theo từng tháng
        public async Task<MonthlyReportViewModel> GetMonthlyReportAsync(int month, int year)
        {
            // Xác định khoảng thời gian
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // Lấy danh sách đơn hàng trong khoảng thời gian
            var orders = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.ProductTranslations)
                .ToListAsync();

            // Tổng doanh thu và sản phẩm bán ra
            var successfulOrders = orders
                .Where(o => o.Status == OrderStatus.Success || o.Status == OrderStatus.Confirmed)
                .ToList();

            var totalRevenue = successfulOrders
                .SelectMany(o => o.OrderDetails)
                .Sum(od => od.Price * od.Quantity);

            var totalProductsSold = successfulOrders
                .SelectMany(o => o.OrderDetails)
                .Sum(od => od.Quantity);

            var totalOrders = successfulOrders.Count;


            // Báo cáo chi tiết sản phẩm
            var productReports = successfulOrders
                .SelectMany(o => o.OrderDetails)
                .GroupBy(od => od.ProductId)
                .Select(g => new ProductReportViewModel
                {
                    ProductName = g.FirstOrDefault()?.Product?.ProductTranslations
                                        .FirstOrDefault(pt => pt.LanguageId == "vi")?.Name ?? "Unknown",
                    QuantitySold = g.Sum(od => od.Quantity),
                    Revenue = g.Sum(od => od.Price * od.Quantity),
                    StockRemaining = g.FirstOrDefault()?.Product?.Stock ?? 0
                })
                .ToList();

            // Thống kê VNPay
            var vnPayOrders = successfulOrders.Where(o => o.PaymentMethod.StartsWith("VnPay"));
            var vnPayRevenue = vnPayOrders
                .SelectMany(o => o.OrderDetails)
                .Sum(od => od.Price * od.Quantity);

            var vnPayReport = new PaymentReportViewModel
            {
                Revenue = vnPayRevenue,
                TotalOrders = vnPayOrders.Count()
            };

            // Thống kê COD
            var codOrders = successfulOrders.Where(o => o.PaymentMethod == "COD");
            var codRevenue = codOrders
                .SelectMany(o => o.OrderDetails)
                .Sum(od => od.Price * od.Quantity);

            var codReport = new PaymentReportViewModel
            {
                Revenue = codRevenue,
                TotalOrders = codOrders.Count()
            };

            // Thống kê số lượng hóa đơn theo trạng thái
            var orderStatusReports = orders
                .GroupBy(o => o.Status)
                .Select(g => new OrderStatusReportViewModel
                {
                    Status = g.Key.ToString(),
                    OrderCount = g.Count()
                })
                .ToList();

            // Tạo ViewModel
            return new MonthlyReportViewModel
            {
                Month = month,
                Year = year,
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalProductsSold = totalProductsSold,
                ProductReports = productReports,
                VNPayReport = vnPayReport,
                CodReport = codReport,
                OrderStatusReports = orderStatusReports
            };
        }

        public async Task ProcessAndSendMonthlyReportAsync(int month, int year, string email)
        {
            // Gọi hàm lấy báo cáo tháng
            var report = await GetMonthlyReportAsync(month, year);

            // Tạo nội dung email từ báo cáo
            var emailContent = GenerateMonthlyReportHtml(report);

            // Gửi email
            var emailSender = new EmailSender(_context.GetService<IConfiguration>());
            await emailSender.SendEmailAsync(email, $"Báo cáo tháng {month:00}/{year}", emailContent);
        }

        private string GenerateMonthlyReportHtml(MonthlyReportViewModel report)
        {
            var sb = new StringBuilder();

            sb.Append($"<h1>Báo cáo tháng {report.Month:00}/{report.Year}</h1>");
            sb.Append($"<p><strong>Tổng doanh thu:</strong> {report.TotalRevenue.ToString("#,##0 VNĐ")}</p>");
            sb.Append($"<p><strong>Tổng số đơn hàng:</strong> {report.TotalOrders}</p>");
            sb.Append($"<p><strong>Tổng số sản phẩm bán ra:</strong> {report.TotalProductsSold}</p>");

            // Thống kê VNPay
            sb.Append("<h2>Thống kê phương thức thanh toán - VNPay</h2>");
            sb.Append($"<p><strong>Doanh thu:</strong> {report.VNPayReport.Revenue.ToString("#,##0 VNĐ")}</p>");
            sb.Append($"<p><strong>Số đơn hàng:</strong> {report.VNPayReport.TotalOrders}</p>");

            // Thống kê COD
            sb.Append("<h2>Thống kê phương thức thanh toán - COD</h2>");
            sb.Append($"<p><strong>Doanh thu:</strong> {report.CodReport.Revenue.ToString("#,##0 VNĐ")}</p>");
            sb.Append($"<p><strong>Số đơn hàng:</strong> {report.CodReport.TotalOrders}</p>");

            // Thống kê hóa đơn theo trạng thái
            sb.Append("<h2>Thống kê hóa đơn theo trạng thái</h2>");
            sb.Append("<table border='1' style='border-collapse:collapse;width:100%;text-align:left;'>");
            sb.Append("<thead><tr><th>Trạng thái</th><th>Số lượng đơn hàng</th></tr></thead><tbody>");
            foreach (var statusReport in report.OrderStatusReports)
            {
                sb.Append($"<tr><td>{statusReport.Status}</td><td>{statusReport.OrderCount}</td></tr>");
            }
            sb.Append("</tbody></table>");

            // Chi tiết sản phẩm
            sb.Append("<h2>Chi tiết sản phẩm</h2>");
            sb.Append("<table border='1' style='border-collapse:collapse;width:100%;text-align:left;'>");
            sb.Append("<thead><tr><th>#</th><th>Sản phẩm</th><th>Số lượng bán</th><th>Doanh thu</th><th>Tồn kho</th></tr></thead><tbody>");

            int index = 1;
            foreach (var product in report.ProductReports)
            {
                sb.Append($"<tr><td>{index++}</td><td>{product.ProductName}</td><td>{product.QuantitySold}</td><td>{product.Revenue.ToString("#,##0 VNĐ")}</td><td>{product.StockRemaining}</td></tr>");
            }
            sb.Append("</tbody></table>");

            return sb.ToString();
        }




        public async Task UpdateIsFeatured()
        {
            // Lấy danh sách sản phẩm cần đặt IsFeatured = true
            var productsToFeature = await _context.Products
                .Where(p => (p.Sold >= 100 || p.ViewCount >= 100) && p.IsFeatured != true)
                .ToListAsync();

            // Lấy danh sách sản phẩm cần đặt IsFeatured = false
            var productsToUnfeature = await _context.Products
                .Where(p => (p.Sold < 100 && p.ViewCount < 100) &&  p.IsFeatured == true)
                .ToListAsync();

            if (productsToFeature.Any())
            {
                foreach (var product in productsToFeature)
                {
                    product.IsFeatured = true;
                }
            }

            if (productsToUnfeature.Any())
            {
                foreach (var product in productsToUnfeature)
                {
                    product.IsFeatured = false;
                }
            }

            if (productsToFeature.Any() || productsToUnfeature.Any())
            {
                await _context.SaveChangesAsync();
            }
        }


    }
}
