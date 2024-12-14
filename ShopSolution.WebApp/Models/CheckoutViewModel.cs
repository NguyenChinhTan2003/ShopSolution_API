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

    }
}
