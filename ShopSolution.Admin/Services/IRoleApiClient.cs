using ShopSolution.ViewModels.Common;
using ShopSolution.ViewModels.System.Roles;

namespace ShopSolution.Admin.Services
{
    public interface IRoleApiClient
    {
        Task<ApiResult<List<RoleVm>>> GetAll();
    }
}