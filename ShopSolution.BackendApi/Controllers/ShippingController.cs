using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopSolution.Application.Catalog.Categories;
using ShopSolution.Application.Utilities;
using ShopSolution.ViewModels.Catalog.Categories;
using ShopSolution.ViewModels.Utilities;

namespace ShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController : Controller
    {
        private readonly IShippingService _shippingService;

        public ShippingController(IShippingService shippingService)
        {
            _shippingService = shippingService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] ShippingRequest request) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shipId = await _shippingService.Create(request);
            if (shipId == 0)
            {
                return BadRequest();
            }
            return CreatedAtAction(nameof(GetById), new { id = shipId}, null);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var ship = await _shippingService.GetById(id);
            return Ok(ship);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var shippings = await _shippingService.GetAll();
            return Ok(shippings);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _shippingService.Delete(id);
            if (result==1)
            {
                return Ok(1);
            }

            return BadRequest("Không thể xóa mục này.");
        }
    }
}
