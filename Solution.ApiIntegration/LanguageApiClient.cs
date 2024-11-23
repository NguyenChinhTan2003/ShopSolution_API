using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ShopSolution.ApiIntegration;
using ShopSolution.ViewModels.Common;
using ShopSolution.ViewModels.System.Languages;

namespace ShopSolution.ApiIntegration
{
    public class LanguageApiClient : BaseApiClient, ILanguageApiClient
    {
        public LanguageApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
        }

        public async Task<ApiResult<List<LanguageVm>>> GetAll()
        {
            return await GetAsync<ApiResult<List<LanguageVm>>>("/api/languages");
        }
    }
}