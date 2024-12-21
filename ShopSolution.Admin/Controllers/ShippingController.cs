using Microsoft.AspNetCore.Mvc;
using ShopSolution.ApiIntegration;
using ShopSolution.ViewModels.Catalog.Categories;
using ShopSolution.ViewModels.Catalog.Products;
using ShopSolution.ViewModels.System.Users;
using ShopSolution.ViewModels.Utilities;

namespace ShopSolution.Admin.Controllers
{
    public class ShippingController : Controller
    {
        private readonly IShippingApiClient _shippingApiClient;
        private readonly IConfiguration _configuration;

        public ShippingController(IShippingApiClient shippingApiClient, IConfiguration configuration)
        {
            _shippingApiClient = shippingApiClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _shippingApiClient.GetAll(); 
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(model);

        }


        [HttpPost]
        public async Task<IActionResult> Delete(ShippingVm request)
        {
            if (ModelState.IsValid)
                return View();

            var result = await _shippingApiClient.Delete(request.Id);
            if (result==1)
            {
                TempData["result"] = "Xóa sản phẩm dùng thành công";
                return RedirectToAction("Index");
            }
            return View(request);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ShippingRequest request)
        {

            if (!ModelState.IsValid)
                return View(request);

            var result = await _shippingApiClient.CreateShip(request);
            if (result)
            {
                TempData["result"] = "Thêm mới sản phẩm thành công";
                    return RedirectToAction("Index");
            
            }
            ModelState.AddModelError("", "Thêm sản phẩm thất bại");
                return RedirectToAction("Index");
            }
    }
}
