using ShopSolution.ViewModels.Catalog.Products;
using ShopSolution.ViewModels.Utilities.Slides;

namespace ShopSolution.WebApp.Models
{
    public class HomeViewModel
    {
        public List<SlideVm> Slides { get; set; }
        public List<ProductVm> FeaturedProducts { get; set; }

        public List<ProductVm> LatestProducts { get; set; }
    }
}