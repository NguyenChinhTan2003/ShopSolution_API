
using ShopSolution.ViewModels.System.Users;

namespace ShopSolution.Admin.Services
{
    public interface IUserApiClient
    {
        Task<string> Authenticate(LoginRequest request);
    }
}
