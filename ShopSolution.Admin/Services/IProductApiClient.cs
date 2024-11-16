using ShopSolution.ViewModels.Catalog.Products;
using ShopSolution.ViewModels.Common;

namespace ShopSolution.Admin.Services
{
    public interface IProductApiClient
    {
         Task<PagedResult<ProductVm>> GetPagings(GetManageProductPagingRequest request);

    }
}
