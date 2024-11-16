using Newtonsoft.Json;
using ShopSolution.Utilities.Constants;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ShopSolution.Admin.Services
{
    public class BaseApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        protected async Task<TResponse> GetAsync<TResponse>(string url)
        {
            // Lấy Token từ Session
            var token = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.Token);
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Token không tồn tại trong session.");
            }

            // Tạo HttpClient
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gửi request và đọc kết quả
            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // Deserialize và trả về
                return System.Text.Json.JsonSerializer.Deserialize<TResponse>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            // Log lỗi và throw Exception nếu không thành công
            var errorMessage = $"Request failed with status code {response.StatusCode}: {body}";
            throw new HttpRequestException(errorMessage);
        }

    }
}
