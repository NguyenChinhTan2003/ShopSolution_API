
using Newtonsoft.Json;
using ShopSolution.ViewModels.System.Users;
using System.Text;

namespace ShopSolution.Admin.Services
{
    public class UserApiClient : IUserApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

    public UserApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> Authenticate(LoginRequest request)
    {
        var json = JsonConvert.SerializeObject(request);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri("https://localhost:7084");
        var response = await client.PostAsync("/api/users/authenticate", httpContent);
        var token = await response.Content.ReadAsStringAsync();
        return token;
    }
}
}
