using Hangfire;
using Microsoft.EntityFrameworkCore;
using ShopSolution.Application.Common;
using ShopSolution.Data.EF;
using ShopSolution.Utilities.Exceptions;
using ShopSolution.ViewModels.Catalog.Products;

namespace ShopSolution.BackendApi.Services
{
    public class JobService : IJobService
    {
        private readonly ShopDBContext _context;

        public JobService(ShopDBContext context)
        {
            _context = context;
        }

        public void SendEmail()
        {
            Console.WriteLine($"Sending email to  with subject: at: {DateTime.Now}");
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
