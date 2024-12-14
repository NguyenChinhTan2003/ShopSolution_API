using ShopSolution.ViewModels.Catalog.Categories;
using ShopSolution.ViewModels.Catalog.ProductImages;
using ShopSolution.ViewModels.Catalog.Products;

namespace ShopSolution.WebApp.Models
{
    public class ProductDetailViewModel
    {
        public CategoryVm? Category { get; set; }

        public ProductVm Product { get; set; }

        public List<ProductVm>? RelatedProducts { get; set; }

        public List<ProductImageViewModel>? ProductImages { get; set; }
        public List<ProductVm>? FeaturedProducts { get; set; }

    }
}
