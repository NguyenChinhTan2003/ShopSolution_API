using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ShopSolution.ApiIntegration;
using ShopSolution.Utilities.Constants;
using ShopSolution.ViewModels.Catalog.Products;
using ShopSolution.ViewModels.Common;
using ShopSolution.ViewModels.System.Users;
using System.Net.Http.Headers;
using System.Text;

namespace ShopSolution.ApiIntegration
{
    public class ProductApiClient : BaseApiClient, IProductApiClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ProductApiClient(IHttpClientFactory httpClientFactory,
                   IHttpContextAccessor httpContextAccessor,
                    IConfiguration configuration)
            : base(httpClientFactory, httpContextAccessor, configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> CreateProduct(ProductCreateRequest request)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.Token);
            var languageId = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();

            // Xử lý ảnh và thêm vào request
            if (request.ThumbnailImage != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ThumbnailImage.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ThumbnailImage.Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                bytes.Headers.ContentType = MediaTypeHeaderValue.Parse(request.ThumbnailImage.ContentType);

                // Tên file ảnh (kèm extension)
                requestContent.Add(bytes, "ThumbnailImage", request.ThumbnailImage.FileName);
            }

            // Thêm các trường khác vào request
            requestContent.Add(new StringContent(request.Price.ToString()), "Price");
            requestContent.Add(new StringContent(request.OriginalPrice.ToString()), "OriginalPrice");
            requestContent.Add(new StringContent(request.Stock.ToString()), "Stock");
            requestContent.Add(new StringContent(request.Name), "Name");
            requestContent.Add(new StringContent(request.Description ?? ""), "Description");
            requestContent.Add(new StringContent(request.Details ?? ""), "Details");
            requestContent.Add(new StringContent(request.SeoDescription ?? ""), "SeoDescription");
            requestContent.Add(new StringContent(request.SeoTitle ?? ""), "SeoTitle");
            requestContent.Add(new StringContent(request.SeoAlias ?? ""), "SeoAlias");
            requestContent.Add(new StringContent(languageId), "LanguageId");

            // Gửi request lên API
            var response = await client.PostAsync($"/api/products", requestContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateProduct(ProductUpdateRequest request)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.Token);
            var languageId = _httpContextAccessor.HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration[SystemConstants.AppSettings.BaseAddress]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);

            var requestContent = new MultipartFormDataContent();

            // Xử lý ảnh (nếu có) và thêm vào request
            if (request.ThumbnailImage != null)
            {
                byte[] data;
                using (var br = new BinaryReader(request.ThumbnailImage.OpenReadStream()))
                {
                    data = br.ReadBytes((int)request.ThumbnailImage.Length);
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                bytes.Headers.ContentType = MediaTypeHeaderValue.Parse(request.ThumbnailImage.ContentType);

                requestContent.Add(bytes, "ThumbnailImage", request.ThumbnailImage.FileName);
            }

            // Thêm các trường khác vào request
            requestContent.Add(new StringContent(request.Id.ToString()), "Id");
            requestContent.Add(new StringContent(request.Name), "Name");
            requestContent.Add(new StringContent(request.Description ?? ""), "Description");
            requestContent.Add(new StringContent(request.Details ?? ""), "Details");
            requestContent.Add(new StringContent(request.SeoDescription ?? ""), "SeoDescription");
            requestContent.Add(new StringContent(request.SeoTitle ?? ""), "SeoTitle");
            requestContent.Add(new StringContent(request.SeoAlias ?? ""), "SeoAlias");
            requestContent.Add(new StringContent(request.Price.ToString()), "Price");
            requestContent.Add(new StringContent(request.OriginalPrice.ToString()), "OriginalPrice");
            requestContent.Add(new StringContent(request.Stock.ToString()), "Stock");
            requestContent.Add(new StringContent(languageId), "LanguageId");

            // Gửi request lên API
            var response = await client.PutAsync($"/api/products/" + request.Id, requestContent);

            return response.IsSuccessStatusCode;
        }

        public async Task<PagedResult<ProductVm>> GetPagings(GetManageProductPagingRequest request)
        {
            var data = await GetAsync<PagedResult<ProductVm>>(
                $"/api/products/paging?pageIndex={request.PageIndex}" +
                $"&pageSize={request.PageSize}" +
               $"&keyword={request.Keyword}&languageId={request.LanguageId}&categoryId={request.CategoryId}");

            return data;
        }

        public async Task<PagedResult<ProductVm>> GetCategoryPagings(GetManageProductPagingRequest request)
        {
            var data = await GetAsync<PagedResult<ProductVm>>(
                $"/api/products/pagingCategory?pageIndex={request.PageIndex}" +
                $"&pageSize={request.PageSize}" +
               $"&keyword={request.Keyword}&languageId={request.LanguageId}&categoryId={request.CategoryId}");

            return data;
        }

        public async Task<PagedResult<ProductVm>> GetCategoryPagings(GetManageProductPagingRequest request)
        {
            var data = await GetAsync<PagedResult<ProductVm>>(
                $"/api/products/pagingCategory?pageIndex={request.PageIndex}" +
                $"&pageSize={request.PageSize}" +
               $"&keyword={request.Keyword}&languageId={request.LanguageId}&categoryId={request.CategoryId}");

            return data;
        }

        public async Task<ApiResult<bool>> CategoryAssign(int id, CategoryAssignRequest request)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/products/{id}/categories", httpContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(result);
            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(result);
        }

        //public async Task<ProductVm> GetById(int id, string languageId)
        //{
        //    var data = await GetAsync<ProductVm>($"/api/products/{id}/{languageId}");
        //    return data;
        //}
        public async Task<ProductVm> GetById(int id, string languageId)
        {
            var url = $"/api/products/{id}/{languageId}";

            return await GetAsync<ProductVm>(url);
        }

        public async Task<ApiResult<bool>> Delete(int id)
        {
            var sessions = _httpContextAccessor.HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_configuration["BaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", sessions);
            var response = await client.DeleteAsync($"/api/products/{id}");
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ApiSuccessResult<bool>>(body);

            return JsonConvert.DeserializeObject<ApiErrorResult<bool>>(body);
        }

        public async Task<List<ProductVm>> GetFeaturedProducts(string languageId, int take)
        {
            var data = await GetListAsync<ProductVm>($"/api/products/featured/{languageId}/{take}");
            return data;
        }

        public async Task<List<ProductVm>> GetLastedProducts(string languageId, int take)
        {
            var data = await GetListAsync<ProductVm>($"/api/products/lasted/{languageId}/{take}");
            return data;
        }
    }
}