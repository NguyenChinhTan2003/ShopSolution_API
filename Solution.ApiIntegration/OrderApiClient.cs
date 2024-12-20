using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopSolution.Utilities.Constants;
using ShopSolution.ViewModels.Catalog.Orders;
using ShopSolution.ViewModels.Common;
using ShopSolution.ViewModels.Catalog.Categories;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ShopSolution.ApiIntegration
{
    public class OrderApiClient : BaseApiClient, IOrderApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration) : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<OrderVm>> GetAll()
        {
            var data = await GetAsync<List<OrderVm>>(
                $"/api/orders");

            return data;
        }

        public async Task<OrderVm> GetById(Guid orderId)
        {
            var data = await GetAsync<OrderVm>($"/api/orders/{orderId}");
            return data;
        }

        public async Task<bool> UpdateStatus(Guid orderId, int status)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.PutAsync($"/api/orders/{orderId}/status?status={status}", null);
            return response.IsSuccessStatusCode;
        }
    }
}