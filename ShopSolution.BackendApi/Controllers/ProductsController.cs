﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopSolution.Application.Catalog.Products;
using ShopSolution.Application.Service.Products;
using ShopSolution.Data.Entities;
using ShopSolution.ViewModels.Catalog.ProductImages;
using ShopSolution.ViewModels.Catalog.Products;

namespace ShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(
            IProductService productService)
        {
            _productService = productService;
        }

        //http://localhost:port/products?pageIndex=1&pageSize=10&CategoryId=
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetManageProductPagingRequest request)
        {
            var products = await _productService.GetAllPaging(request);
            return Ok(products);
        }

        [HttpGet("pagingCategory")]
        public async Task<IActionResult> GetProductById([FromQuery] GetManageProductPagingRequest request)
        {
            var products = await _productService.GetAllByCategoryId(request.LanguageId, new GetPublicProductPagingRequest
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                CategoryId = request.CategoryId,
                Keyword = request.Keyword
            });
            return Ok(products);
        }

        //http://localhost:port/product/1
        [HttpGet("{productId}/{languageId}")]
        public async Task<IActionResult> GetById(int productId, string languageId)
        {
            var product = await _productService.GetById(productId, languageId);
            if (product == null)
                return BadRequest("Cannot find product");
            return Ok(product);
        }

        [HttpGet("featured/{languageId}/{take}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeaturedProducts(int take, string languageId)
        {
            var products = await _productService.GetFeaturedProducts(languageId, take);
            return Ok(products);
        }

        [HttpGet("lasted/{languageId}/{take}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLastedProducts(int take, string languageId)
        {
            var products = await _productService.GetLastedProducts(languageId, take);
            return Ok(products);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var productId = await _productService.Create(request);
            if (productId == 0)
            {
                return BadRequest();
            }

            var product = await _productService.GetById(productId, request.LanguageId);

            return CreatedAtAction(nameof(GetById), new { id = productId }, product);
        }

        [HttpPut("{productId}")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int productId, [FromForm] ProductUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = productId;
            var affectedResult = await _productService.Update(request);
            if (affectedResult == 0)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpDelete("{productId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int productId)
        {
            var affectedResult = await _productService.Delete(productId);
            if (affectedResult == 0)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPatch("{productId}/{newPrice}")]
        [Authorize]
        public async Task<IActionResult> UpdatePrice(int productId, decimal newPrice)
        {
            var isSuccessful = await _productService.updatePrice(productId, newPrice);
            if (isSuccessful)
                return Ok();

            return BadRequest();
        }

        //Images
        [HttpPost("{productId}/images")]
        [Authorize]
        public async Task<IActionResult> CreateImage(int productId, [FromForm] ProductImageCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var imageId = await _productService.AddImages(productId, request);
            if (imageId == 0)
                return BadRequest("Cannot add image");

            var image = await _productService.GetImageById(imageId);

            // Tạo URL cho ảnh vừa thêm
            var imageUrl = Url.Content($"~/images/products/{image.ImagePath}");
            return CreatedAtAction(nameof(GetImageById), new { productId, imageId }, new { image.Id, imageUrl });
        }

        [HttpPut("{productId}/images/{imageId}")]
        [Authorize]
        public async Task<IActionResult> UpdateImage(int imageId, [FromForm] ProductImageUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productService.UpdateImages(imageId, request);
            if (result == 0)
                return BadRequest();

            return Ok();
        }

        [HttpDelete("{productId}/images/{imageId}")]
        [Authorize]
        public async Task<IActionResult> RemoveImage(int imageId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productService.RemoveImage(imageId);
            if (result == 0)
                return BadRequest();

            return Ok();
        }

        [HttpGet("{productId}/images/{imageId}")]
        public async Task<IActionResult> GetImageById(int productId, int imageId)
        {
            var image = await _productService.GetImageById(imageId);
            if (image == null)
                return NotFound("Cannot find image");

            // Trả về URL của ảnh
            var imageUrl = Url.Content($"~/images/products/{image.ImagePath}");
            return Ok(new { image.Id, image.ProductId, imageUrl });
        }

        // API tải ảnh trực tiếp
        [HttpGet("{productId}/images/{imageId}/download")]
        public async Task<IActionResult> DownloadImage(int productId, int imageId)
        {
            var image = await _productService.GetImageById(imageId);
            if (image == null || string.IsNullOrEmpty(image.ImagePath))
                return NotFound("Cannot find image");

            var ImagePath = Path.Combine("wwwroot/user-content", image.ImagePath);

            if (!System.IO.File.Exists(ImagePath))
                return NotFound("Image file not found");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(ImagePath);
            var contentType = "application/octet-stream"; // Bạn có thể dùng thư viện để xác định kiểu MIME

            return File(fileBytes, contentType, Path.GetFileName(ImagePath));
        }

        [HttpPut("{id}/categories")]
        [Authorize]
        public async Task<IActionResult> CategoryAssign(int id, [FromBody] CategoryAssignRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _productService.CategoryAssign(id, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}