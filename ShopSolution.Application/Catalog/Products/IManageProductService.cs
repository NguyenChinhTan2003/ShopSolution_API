
using Microsoft.AspNetCore.Http;
using ShopSolution.Application.Catalog.Products;
using ShopSolution.Data.Entities;
using ShopSolution.ViewModels.Catalog.Products;

using ShopSolution.ViewModels.Common;

namespace ShopSolution.Application.Service.Products
{
    public interface IManageProductService
    {
        Task<int> Create(ProductCreateRequest request);

        Task<int> Update(ProductUpdateRequest request);

        Task<int> Delete(int productId);

        Task<bool> updatePrice(int productId, decimal newPrice);

        Task<bool> updateStock(int productId, int addedQuantity);

        Task AddViewCount(int productId);

        Task<PagedResult<ProductViewModel>> GetAllPaging(GetManageProductPagingRequest request);

        Task<int> AddImages(int productId, List<IFormFile> files);
        Task<int> UpdateImages(int imageId, string caption, bool isDefault);
        Task<int> DeleteImages(int imageId);

        Task<List<ProductImageViewModel>> GetListImage(int productId);

    }
}
