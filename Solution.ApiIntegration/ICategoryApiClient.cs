using ShopSolution.ViewModels.Catalog.Categories;
using ShopSolution.ViewModels.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopSolution.ApiIntegration
{
    public interface ICategoryApiClient
    {
        Task<List<CategoryVm>> GetAll(string languageId);

        Task<CategoryVm> GetById(string languageId, int id);

        Task<PagedResult<CategoryVm>> GetAllPaging(GetManageCategoryPagingRequest request);

        Task<bool> CreateCategory(CategoryCreateRequest request);

        Task<bool> UpdateCategory(CategoryUpdateRequest request);

        Task<ApiResult<bool>> DeleteCategory(int id);
    }
}