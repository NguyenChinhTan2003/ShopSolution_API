using ShopSolution.Data.Entities;
using ShopSolution.ViewModels.Catalog.Products;
using ShopSolution.ViewModels.Sales;
using System.ComponentModel.DataAnnotations;

namespace ShopSolution.WebApp.Models
{
    public class CheckoutViewModel
    {
        public List<CartItemViewModel> CartItems { get; set; }
        public CheckoutRequest CheckoutModel { get; set; }
        public ProductVm Product { get; set; }
        public string Tinh { get; set; } // Tỉnh
        public string Quan { get; set; } // Quận
        public string Phuong { get; set; } // Phường
        public string FullAddress => $"{Phuong}, {Quan}, {Tinh}";

    }
}
