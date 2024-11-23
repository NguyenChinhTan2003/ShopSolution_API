using ShopSolution.ViewModels.Catalog.Categories;

namespace ShopSolution.ApiIntegration
{
    public interface ICategoryApiClient
    {
        Task<List<CategoryVm>> GetAll(string languageId);
    }
}