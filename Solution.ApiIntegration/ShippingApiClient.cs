using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ShopSolution.Utilities.Constants;
using ShopSolution.ViewModels.Catalog.Categories;
using ShopSolution.ViewModels.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.ApiIntegration
{
    public class ShippingApiClient : BaseApiClient, IShippingApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShippingApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration) : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> CreateShip(ShippingRequest request)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.Token);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            // Serialize request object to JSON
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var requestContent = new MultipartFormDataContent();

            requestContent.Add(new StringContent(request.Price.ToString()), "Price");
            requestContent.Add(new StringContent(request.Ward ?? ""), "Ward");
            requestContent.Add(new StringContent(request.District ?? ""), "District");
            requestContent.Add(new StringContent(request.City ?? ""), "City");
            // Gửi request lên API
            var response = await client.PostAsync($"/api/shipping", requestContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<int> Delete(int id)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.Token);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var response = await client.DeleteAsync($"/api/shipping/{id}");
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // Kiểm tra nếu phản hồi có nội dung
                if (!string.IsNullOrWhiteSpace(body))
                {
                    return JsonConvert.DeserializeObject<int>(body);
                }

                return 1; // Trả về giá trị mặc định nếu API không gửi JSON (nhưng vẫn thành công)
            }

            return 0; // Trả về 0 khi lỗi xảy ra
        }


        public async Task<List<ShippingVm>> GetAll()
        {
            var data = await GetAsync<List<ShippingVm>>(
               $"/api/shipping");

            return data;
        }
    }
}
