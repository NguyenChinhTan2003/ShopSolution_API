using LazZiya.ExpressLocalization;
using Microsoft.AspNetCore.Mvc;
using ShopSolution.ApiIntegration;
using ShopSolution.Utilities.Constants;
using ShopSolution.WebApp.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ShopSolution.WebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductApiClient _productApiClient;


        public ProductController(IProductApiClient productApiClient)
        {
            _productApiClient = productApiClient;

        }

        public async Task<IActionResult> Detail(int id, string culture)
        {

             ViewData["ShowSideComponent"] = false;
            // Gọi API để lấy thông tin sản phẩm
            var product = await _productApiClient.GetById(id, culture);

            if (product == null)
            {
                return NotFound(); // Nếu không tìm thấy sản phẩm
            }

            // Trả về View với dữ liệu chi tiết sản phẩm
            return View(new ProductDetailViewModel()
            {
                Product = product
            });
        }
    }
}