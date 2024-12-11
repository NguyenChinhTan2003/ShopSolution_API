using LazZiya.ExpressLocalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSolution.ApiIntegration;
using ShopSolution.Data.EF;
using ShopSolution.Utilities.Constants;
using ShopSolution.WebApp.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ShopSolution.WebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductApiClient _productApiClient;
        private readonly ShopDBContext _shopDBContext;

        public ProductController(IProductApiClient productApiClient,ShopDBContext shopDBContext)
        {
            _productApiClient = productApiClient;
            _shopDBContext = shopDBContext;

        }

        public async Task<IActionResult> Detail(int id, string culture)
        {

             ViewData["ShowSideComponent"] = false;
            // Gọi API để lấy thông tin sản phẩm
            var product = await _productApiClient.GetById(id, culture);
            var products = await _shopDBContext.Products.FirstOrDefaultAsync(p => p.Id == id );
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