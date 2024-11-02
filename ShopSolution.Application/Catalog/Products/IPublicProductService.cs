using ShopSolution.Application.Catalog.Products.DTO;
using ShopSolution.Application.Catalog.Products.DTO.Public;
using ShopSolution.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.Application.Catalog.Products
{
    public interface IPublicProductService
    {
        Task<PagedResult<ProductViewModel>> GetAllByCategoryId(GetProductPagingRequest request);    
    }
}
