using Microsoft.AspNetCore.Http;
using ShopSolution.Data.Entities;
using ShopSolution.ViewModels.Catalog.ProductImages;
using ShopSolution.ViewModels.Catalog.Products;

using ShopSolution.ViewModels.Common;

namespace ShopSolution.Application.Service.Products
{
    public interface IProductService
    {
        Task<int> Create(ProductCreateRequest request);

        Task<int> Update(ProductUpdateRequest request);

        Task<int> Delete(int productId);

        Task<ProductVm> GetById(int productId, string languageId);

        Task<bool> updatePrice(int productId, decimal newPrice);

        Task<bool> updateStock(int productId, int addedQuantity);

        Task AddViewCount(int productId);

        Task<PagedResult<ProductVm>> GetAllPaging(GetManageProductPagingRequest request);

        Task<int> AddImages(int productId, ProductImageCreateRequest request);

        Task<int> UpdateImages(int imageId, ProductImageUpdateRequest request);

        Task<int> RemoveImage(int imageId);

        Task<ProductImageViewModel> GetImageById(int imageId);

        Task<List<ProductImageViewModel>> GetListImage(int productId);

        Task<PagedResult<ProductVm>> GetAllByCategoryId(string languageId, GetPublicProductPagingRequest request);
    }
}