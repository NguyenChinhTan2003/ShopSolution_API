using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSolution.Application.Catalog.Order;
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
        private readonly ShopDBContext _dbContext;

        public OrdersController(
            IOrderService orderService, ShopDBContext dbContext)
        {
            _orderService = orderService;
            _dbContext = dbContext;
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
            var order = await _dbContext.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound($"Không tìm thấy đơn hàng với ID: {orderId}");
            }

            // Cập nhật trạng thái
            order.Status = (OrderStatus)status; // Ép kiểu int sang OrderStatus

            // Lưu thay đổi
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}