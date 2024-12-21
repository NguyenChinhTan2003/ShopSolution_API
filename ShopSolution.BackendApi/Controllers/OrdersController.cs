using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSolution.Application.Catalog.Order;
using ShopSolution.BackendApi.Services;
using ShopSolution.Data.EF;
using ShopSolution.Data.Enums;
using ShopSolution.ViewModels.Catalog.Orders;

namespace ShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IEmailSender _emailSender;
        private readonly ShopDBContext _dbContext;

        public OrdersController(
            IOrderService orderService, ShopDBContext dbContext, IEmailSender emailSender)
        {
            _orderService = orderService;
            _dbContext = dbContext;
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _orderService.GetAll();
            return Ok(products);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetById(Guid orderId)
        {
            var order = await _dbContext.Orders
                .Include(o => o.OrderDetails) // Include OrderDetails nếu bạn cần thông tin chi tiết đơn hàng
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound($"Không tìm thấy đơn hàng với ID: {orderId}");
            }

            // Mapping từ Order entity sang OrderVm
            var orderVm = new OrderVm
            {
                Id = order.Id,
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                UserName = order.UserName,
                Email = order.Email,
                Address = order.Address,
                Phone = order.Phone,
                PaymentMethod = order.PaymentMethod,
                Status = (int)order.Status,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailVm
                {
                    Id = od.Id,
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList()
            };

            return Ok(orderVm);
        }

        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateStatus(Guid orderId, [FromQuery] int status)
        {
            // Tìm order bằng orderId
            var order = await _dbContext.Orders
                        .Include(o => o.OrderDetails) // Include OrderDetails để lấy thông tin chi tiết
                        .FirstOrDefaultAsync(o => o.OrderId == orderId); // Sửa FindAsync thành FirstOrDefaultAsync
            if (order == null)
            {
                return NotFound($"Không tìm thấy đơn hàng với ID: {orderId}");
            }

            // Cập nhật trạng thái
            order.Status = (OrderStatus)status; // Ép kiểu int sang OrderStatus

            // Lưu thay đổi
            await _dbContext.SaveChangesAsync();
            // Gửi email dựa trên trạng thái đơn hàng
            var email = order.Email;
            if (!string.IsNullOrEmpty(email))
            {
                string subject = "";
                string message = "";

                // Tạo bảng HTML cho chi tiết đơn hàng (dùng chung cho các trạng thái)
                var orderDetailsHtml = "<table border='1' cellpadding='5' style='border-collapse:collapse;'>";
                orderDetailsHtml += "<tr><th>Mã sản phẩm</th><th>Số lượng</th><th>Đơn giá</th><th>Thành tiền</th></tr>";
                foreach (var detail in order.OrderDetails)
                {
                    orderDetailsHtml += $"<tr>";
                    orderDetailsHtml += $"<td>{detail.ProductId}</td>";
                    orderDetailsHtml += $"<td>{detail.Quantity}</td>";
                    orderDetailsHtml += $"<td>{detail.Price.ToString("N0")} đ</td>";
                    orderDetailsHtml += $"<td>{(detail.Quantity * detail.Price).ToString("N0")} đ</td>";
                    orderDetailsHtml += $"</tr>";
                }
                orderDetailsHtml += "</table>";

                switch (order.Status)
                {
                    case OrderStatus.InProgress:
                        subject = $"Thông báo đơn hàng #{order.OrderId} đang chờ xác nhận";
                        message = $"<p>Xin chào {order.UserName},</p>" +
                                  $"<p>Cảm ơn bạn đã đặt hàng tại cửa hàng của chúng tôi.</p>" +
                                  $"<p>Đơn hàng #{order.OrderId} của bạn đang chờ xác nhận.</p>" +
                                  $"<p><b>Thông tin đơn hàng:</b></p>" +
                                  $"<ul>" +
                                  $"<li>Mã đơn hàng: {order.OrderId}</li>" +
                                  $"<li>Ngày đặt hàng: {order.OrderDate.ToString("dd/MM/yyyy HH:mm")}</li>" +
                                  $"<li>Tên khách hàng: {order.UserName}</li>" +
                                  $"<li>Email: {order.Email}</li>" +
                                  $"<li>Địa chỉ: {order.Address}</li>" +
                                  $"<li>Số điện thoại: {order.Phone}</li>" +
                                  $"</ul>" +
                                  $"<p><b>Chi tiết đơn hàng:</b></p>" +
                                  orderDetailsHtml +
                                  $"<p>Chúng tôi sẽ liên hệ với bạn trong thời gian sớm nhất.</p>" +
                                  $"<p>Trân trọng,</p>" +
                                  $"<p>Đội ngũ hỗ trợ</p>";
                        break;

                    case OrderStatus.Confirmed:
                        subject = $"Thông báo đơn hàng #{order.OrderId} đã được xác nhận";
                        message = $"<p>Xin chào {order.UserName},</p>" +
                                  $"<p>Cảm ơn bạn đã đặt hàng tại cửa hàng của chúng tôi.</p>" +
                                  $"<p>Đơn hàng #{order.OrderId} của bạn đang chờ xác nhận.</p>" +
                                  $"<p><b>Thông tin đơn hàng:</b></p>" +
                                  $"<ul>" +
                                  $"<li>Mã đơn hàng: {order.OrderId}</li>" +
                                  $"<li>Ngày đặt hàng: {order.OrderDate.ToString("dd/MM/yyyy HH:mm")}</li>" +
                                  $"<li>Tên khách hàng: {order.UserName}</li>" +
                                  $"<li>Email: {order.Email}</li>" +
                                  $"<li>Địa chỉ: {order.Address}</li>" +
                                  $"<li>Số điện thoại: {order.Phone}</li>" +
                                  $"</ul>" +
                                  $"<p><b>Chi tiết đơn hàng:</b></p>" +
                                  orderDetailsHtml +
                                  $"<p>Chúng tôi sẽ liên hệ với bạn trong thời gian sớm nhất.</p>" +
                                  $"<p>Trân trọng,</p>" +
                                  $"<p>Đội ngũ hỗ trợ</p>";
                        break;

                    case OrderStatus.Success:
                        subject = $"Thông báo đơn hàng #{order.OrderId} đã giao thành công";
                        message = $"<p>Xin chào {order.UserName},</p>" +
                                  $"<p>Đơn hàng #{order.OrderId} của bạn đã được giao thành công.</p>" +
                                  $"<p>Cảm ơn bạn đã mua sắm tại cửa hàng của chúng tôi.</p>" +
                                  $"<p><b>Thông tin đơn hàng:</b></p>" +
                                  $"<ul>" +
                                  $"<li>Mã đơn hàng: {order.OrderId}</li>" +
                                  $"<li>Ngày đặt hàng: {order.OrderDate.ToString("dd/MM/yyyy HH:mm")}</li>" +
                                  $"<li>Tên khách hàng: {order.UserName}</li>" +
                                  $"<li>Email: {order.Email}</li>" +
                                  $"<li>Địa chỉ: {order.Address}</li>" +
                                  $"<li>Số điện thoại: {order.Phone}</li>" +
                                  $"</ul>" +
                                  $"<p><b>Chi tiết đơn hàng:</b></p>" +
                                  orderDetailsHtml +
                                  $"<p>Vui lòng liên hệ với chúng tôi nếu bạn có bất kỳ câu hỏi nào.</p>" +
                                  $"<p>Trân trọng,</p>" +
                                  $"<p>Đội ngũ hỗ trợ</p>";
                        break;

                    case OrderStatus.Canceled:
                        subject = $"Thông báo hủy đơn hàng #{order.OrderId}";
                        message = $"<p>Xin chào {order.UserName},</p>" +
                                      $"<p>Chúng tôi rất tiếc phải thông báo rằng đơn hàng #{order.OrderId} của bạn đã bị hủy.</p>" +
                                      $"<p><b>Thông tin đơn hàng:</b></p>" +
                                      $"<ul>" +
                                      $"<li>Mã đơn hàng: {order.OrderId}</li>" +
                                      $"<li>Ngày đặt hàng: {order.OrderDate.ToString("dd/MM/yyyy HH:mm")}</li>" +
                                      $"<li>Tên khách hàng: {order.UserName}</li>" +
                                      $"<li>Email: {order.Email}</li>" +
                                      $"<li>Địa chỉ: {order.Address}</li>" +
                                      $"<li>Số điện thoại: {order.Phone}</li>" +
                                      $"</ul>" +
                                      $"<p><b>Chi tiết đơn hàng:</b></p>" +
                                      orderDetailsHtml +
                                      $"<p>Vui lòng liên hệ với chúng tôi nếu bạn có bất kỳ câu hỏi nào.</p>" +
                                      $"<p>Trân trọng,</p>" +
                                      $"<p>Đội ngũ hỗ trợ</p>";
                        break;
                }

                if (!string.IsNullOrEmpty(subject) && !string.IsNullOrEmpty(message))
                {
                    try
                    {
                        await _emailSender.SendEmailAsync(email, subject, message);
                    }
                    catch (Exception ex)
                    {
                        // Log lỗi gửi email ở đây
                        Console.WriteLine($"Gửi email thất bại: {ex.Message}");
                    }
                }
            }

            return Ok();
        }
    }
}