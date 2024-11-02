using ShopSolution.Application.Catalog.Products.DTO;
using ShopSolution.Application.Catalog.Products.DTO.Manage;
using ShopSolution.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        Task<List<ProductViewModel>> GetAll();

        Task<PagedResult<ProductViewModel>> GetAllPaging(GetProductPagingRequest request);
    }
}
