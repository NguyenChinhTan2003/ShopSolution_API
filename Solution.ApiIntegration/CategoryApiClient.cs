using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ShopSolution.Utilities.Constants;
using ShopSolution.ViewModels.Catalog.Categories;
using ShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ShopSolution.ApiIntegration
{
    public class CategoryApiClient : BaseApiClient, ICategoryApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration) : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> CreateCategory(CategoryCreateRequest request)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.Token);
            var languageId = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            // Serialize request object to JSON
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/categories/", httpContent);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                // Xử lý lỗi
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error creating category: {response.StatusCode} - {errorContent}"); // Log lỗi
                return false;
            }
        }

        public async Task<int> DeleteCategory(int id)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.Token);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.DeleteAsync($"/api/categories/{id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<int>(body);

            return 0; // Hoặc throw exception nếu bạn muốn xử lý lỗi
        }

        public async Task<List<CategoryVm>> GetAll(string languageId)
        {
            var data = await GetAsync<List<CategoryVm>>(
                $"/api/categories?languageId={languageId}");

            return data;
        }

        public async Task<PagedResult<CategoryVm>> GetAllPaging(GetManageCategoryPagingRequest request)
        {
            var sessions = _httpContextAccessor
                .HttpContext
                .Session
                .GetString(SystemConstants.AppSettings.Token);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var url = $"/api/categories/paging?pageIndex={request.PageIndex}" +
                      $"&pageSize={request.PageSize}" +
                      $"&keyword={request.Keyword}" +
                      $"&languageId={request.LanguageId}" +
                      $"&ParentId={request.ParentId}";

            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<PagedResult<CategoryVm>>(body);
                return result;
            }

            // Xử lý lỗi nếu cần
            return null;
        }

        public async Task<CategoryVm> GetById(string languageId, int id)
        {
            var data = await GetAsync<CategoryVm>(
                $"/api/categories/{id}/{languageId}");

            return data;
        }

        public async Task<bool> UpdateCategory(CategoryUpdateRequest request)
        {
            var sessions = _httpContextAccessor
                .HttpContext
                .Session
                .GetString(SystemConstants.AppSettings.Token);

            var languageId = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.Name) ? "" : request.Name.ToString()), "name");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.SeoDescription) ? "" : request.SeoDescription.ToString()), "seoDescription");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.SeoTitle) ? "" : request.SeoTitle.ToString()), "seoTitle");
            requestContent.Add(new StringContent(string.IsNullOrEmpty(request.SeoAlias) ? "" : request.SeoAlias.ToString()), "seoAlias");
            requestContent.Add(new StringContent(request.LanguageId.ToString()), "languageId");
            requestContent.Add(new StringContent(request.ParentId.HasValue ? request.ParentId.ToString() : "null"), "parentId");
            var response = await client.PutAsync($"/api/categories/{request.Id}", requestContent);
            return response.IsSuccessStatusCode;
        }

        protected async Task<TResponse> GetAsync<TResponse>(string url)
        {
            var sessions = _httpContextAccessor
                .HttpContext
                .Session
                .GetString(SystemConstants.AppSettings.Token);

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                TResponse myDeserializedObjList = JsonConvert.DeserializeObject<TResponse>(body); // Sửa deserialize trực tiếp
                return myDeserializedObjList;
            }
            return default(TResponse);
        }
    }
}