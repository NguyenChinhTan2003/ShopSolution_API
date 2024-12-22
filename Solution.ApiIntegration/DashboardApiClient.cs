using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using ShopSolution.ViewModels.Common;

namespace ShopSolution.ApiIntegration
{
    public class DashboardApiClient : IDashboardApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardApiClient(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DashboardViewModel> GetDashboardData()
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");

            _httpClient.BaseAddress = new Uri(_configuration["BaseAddress"]);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await _httpClient.GetAsync($"/api/dashboards");
            var body = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<DashboardViewModel>(body);

            return data;
        }
    }
}