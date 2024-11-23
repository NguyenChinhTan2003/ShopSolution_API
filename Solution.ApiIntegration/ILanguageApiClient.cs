using ShopSolution.ViewModels.Common;
using ShopSolution.ViewModels.System.Languages;

namespace ShopSolution.ApiIntegration
{
    public interface ILanguageApiClient
    {
        Task<ApiResult<List<LanguageVm>>> GetAll();
    }
}