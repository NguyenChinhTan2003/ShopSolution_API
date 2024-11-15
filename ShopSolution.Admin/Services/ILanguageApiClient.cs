using ShopSolution.ViewModels.Common;
using ShopSolution.ViewModels.System.Languages;

namespace ShopSolution.Admin.Services
{
    public interface ILanguageApiClient
    {
        Task<ApiResult<List<LanguageVm>>> GetAll();
    }
}
