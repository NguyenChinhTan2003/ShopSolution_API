using LazZiya.ExpressLocalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSolution.ApiIntegration;
using ShopSolution.Data.EF;
using ShopSolution.Data.Entities;
using ShopSolution.Utilities.Constants;
using ShopSolution.ViewModels.Catalog.Products;
using ShopSolution.WebApp.Models;
using System.Globalization;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

namespace ShopSolution.WebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductApiClient _productApiClient;
        private readonly ShopDBContext _shopDBContext;
        private readonly ICategoryApiClient _categoryApiClient;
        private readonly ISharedCultureLocalizer _loc;

        public ProductController(IProductApiClient productApiClient, ICategoryApiClient categoryApiClient, ISharedCultureLocalizer loc, ShopDBContext shopDBContext)
        {
            _productApiClient = productApiClient;
            _shopDBContext = shopDBContext;
            _categoryApiClient = categoryApiClient;
            _loc = loc;
        }

        public async Task<IActionResult> Category(int id, string culture, int page = 1)
        {
            ViewData["ShowSideComponent"] = false;
            // Gọi API để lấy thông tin sản phẩm
            var product = await _productApiClient.GetCategoryPagings(new GetManageProductPagingRequest()
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
            var products = await _shopDBContext.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound(); // Nếu không tìm thấy sản phẩm
            }

            products.ViewCount++;
            _shopDBContext.Products.Update(products);
            await _shopDBContext.SaveChangesAsync();

            // Trả về View với dữ liệu chi tiết sản phẩm
            return View(new ProductDetailViewModel()
            {
                Product = product,
            });
        }
    }
}