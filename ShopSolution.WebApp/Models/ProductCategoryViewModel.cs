using ShopSolution.ViewModels.Catalog.Categories;
using ShopSolution.ViewModels.Catalog.Products;
using ShopSolution.ViewModels.Common;

namespace ShopSolution.WebApp.Models
{
    public class ProductCategoryViewModel
    {
        public CategoryVm Category { get; set; }
        public PagedResult<ProductVm> Products { get; set; }
    }
}