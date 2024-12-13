using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopSolution.Application.Catalog.Categories;
using ShopSolution.ViewModels.Catalog.Categories;

namespace ShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(
            ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] CategoryCreateRequest request) // Sửa [FromBody] thành [FromForm]
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryId = await _categoryService.Create(request);
            if (categoryId == 0)
            {
                return BadRequest();
            }

            // Lấy thông tin category vừa tạo (nếu cần)
            // var category = await _categoryService.GetById(request.LanguageId, categoryId);

            return CreatedAtAction(nameof(GetById), new { id = categoryId, languageId = request.LanguageId }, null); // Trả về 201 Created, có thể thay null bằng category nếu cần
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] CategoryUpdateRequest request) // Sửa [FromBody] thành [FromForm]
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            request.Id = id;
            var affectedResult = await _categoryService.Update(request);
            if (affectedResult == 0)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var affectedResult = await _categoryService.Delete(id);
            if (affectedResult == 0)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string languageId)
        {
            var products = await _categoryService.GetAll(languageId);
            return Ok(products);
        }

        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetManageCategoryPagingRequest request)
        {
            var categories = await _categoryService.GetAllPaging(request.LanguageId, request); // Sửa tham số truyền vào
            return Ok(categories);
        }

        [HttpGet("{id}/{languageId}")]
        public async Task<IActionResult> GetById(string languageId, int id)
        {
            var category = await _categoryService.GetById(languageId, id);
            return Ok(category);
        }
    }
}