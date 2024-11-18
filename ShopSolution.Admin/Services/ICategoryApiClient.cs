using ShopSolution.ViewModels.Catalog.Categories;

namespace ShopSolution.Admin.Services
{
    public interface ICategoryApiClient
    {
        Task<List<CategoryVm>> GetAll(string languageId);
    }
}
