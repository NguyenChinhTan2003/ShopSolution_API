using Microsoft.AspNetCore.Mvc;
using ShopSolution.ApiIntegration;
using ShopSolution.ViewModels.Catalog.Orders;

namespace ShopSolution.Admin.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderApiClient _orderApiClient;
        

        public OrderController(IOrderApiClient categoryApiClient)
        {
            _orderApiClient = categoryApiClient;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _orderApiClient.GetAll();
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid orderId)
        {
            var order = await _orderApiClient.GetById(orderId); // Giả sử bạn có phương thức GetById trong IOrderApiClient
            if (order == null)
            {
                return NotFound();
            }

            var editVm = new OrderEditVm
            {
                OrderId = order.OrderId,
                Status = order.Status
            };

            return View(editVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderEditVm request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var result = await _orderApiClient.UpdateStatus(request.OrderId, request.Status); // Giả sử bạn có phương thức UpdateStatus trong IOrderApiClient
            if (result)
            {
                TempData["result"] = "Cập nhật trạng thái đơn hàng thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Cập nhật trạng thái đơn hàng thất bại");
            return View(request);
        }
    }
}