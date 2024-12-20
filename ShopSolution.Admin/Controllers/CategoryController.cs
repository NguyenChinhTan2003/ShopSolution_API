using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopSolution.Admin.Models;
using ShopSolution.ApiIntegration;
using ShopSolution.Utilities.Constants;
using ShopSolution.ViewModels.Catalog.Categories;
using ShopSolution.ViewModels.Common;
using System.Linq;
using System.Threading.Tasks;

namespace ShopSolution.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryApiClient _categoryApiClient;
        private readonly IConfiguration _configuration;

        public CategoryController(ICategoryApiClient categoryApiClient, IConfiguration configuration)
        {
            _categoryApiClient = categoryApiClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string keyword, int? parentId, int pageIndex = 1, int pageSize = 10)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);

            var request = new GetManageCategoryPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                LanguageId = languageId,
                ParentId = parentId
            };

            var data = await _categoryApiClient.GetAllPaging(request);

            if (data == null || data.Items == null)
            {
                // Xử lý lỗi, ví dụ: hiển thị thông báo lỗi
                ViewBag.ErrorMessage = "Không thể lấy danh sách danh mục.";
                return View(new PagedResult<CategoryVm>() { Items = new List<CategoryVm>() }); // Trả về danh sách rỗng
            }
            ViewBag.Keyword = keyword;

            // Lấy danh sách danh mục cha để hiển thị dropdownlist
            var categories = await _categoryApiClient.GetAll(languageId);
            ViewBag.Categories = categories.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = parentId.HasValue && parentId.Value == x.Id
            });
            // Tạo dictionary ParentId -> ParentName
            var parentCategoryNames = categories.Where(c => c.ParentId == null).ToDictionary(c => c.Id, c => c.Name);
            ViewBag.ParentCategoryNames = parentCategoryNames;

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateRequest request)
        {
            if (ModelState.IsValid)
                return View(request);

            try
            {
                var result = await _categoryApiClient.CreateCategory(request);
                if (result)
                {
                    TempData["result"] = "Thêm mới danh mục thành công";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm danh mục thất bại. API có thể không trả về lỗi.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Thêm danh mục thất bại: " + ex.Message);
            }
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);
            var category = await _categoryApiClient.GetById(languageId, id);
            if (category == null)
            {
                // Handle the case where the category is not found
                return NotFound(); // Or return a specific error view
            }

            var updateRequest = new CategoryUpdateRequest()
            {
                Id = category.Id,
                Name = category.Name,
                SeoAlias = category.SeoAlias,
                SeoDescription = category.SeoDescription,
                SeoTitle = category.SeoTitle,
                LanguageId = category.LanguageId,
                ParentId = category.ParentId
            };
            return View(updateRequest);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryUpdateRequest request)
        {
            if (ModelState.IsValid)
                return View(request);

            var result = await _categoryApiClient.UpdateCategory(request);
            if (result)
            {
                TempData["result"] = "Cập nhật danh mục thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Cập nhật danh mục thất bại");
            return View(request);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            return View(new CategoryDeleteRequest()
            {
                Id = id
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CategoryDeleteRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _categoryApiClient.DeleteCategory(request.Id);
            if (result == null)
            {
                TempData["result"] = "Xóa danh mục thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Xóa danh mục thất bại");
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);
            var category = await _categoryApiClient.GetById(languageId, id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
    }
}