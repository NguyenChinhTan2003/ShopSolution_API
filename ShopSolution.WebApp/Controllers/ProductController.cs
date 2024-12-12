using LazZiya.ExpressLocalization;
using Microsoft.AspNetCore.Mvc;
using ShopSolution.ApiIntegration;
using ShopSolution.Data.Entities;
using ShopSolution.Utilities.Constants;
using ShopSolution.ViewModels.Catalog.Products;
using ShopSolution.WebApp.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ShopSolution.WebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductApiClient _productApiClient;
        private readonly ICategoryApiClient _categoryApiClient;
        private readonly ISharedCultureLocalizer _loc;

        public ProductController(IProductApiClient productApiClient, ICategoryApiClient categoryApiClient, ISharedCultureLocalizer loc)
        {
            _productApiClient = productApiClient;
            _categoryApiClient = categoryApiClient;
            _loc = loc;
        }

        public async Task<IActionResult> Category(int id, string culture, int page = 1)
        {
            ViewData["ShowSideComponent"] = false;
            // Gọi API để lấy thông tin sản phẩm
            var product = await _productApiClient.GetPagings(new GetManageProductPagingRequest()
            {
                CategoryId = id,
                PageIndex = page,
                LanguageId = culture,
                PageSize = 10
            });
            return View(new ProductCategoryViewModel()
            {
                Category = await _categoryApiClient.GetById(culture, id),
                Products = product
            }); ;
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