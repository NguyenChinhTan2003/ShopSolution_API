using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.ViewModels.Catalog.Products
{
    public class ProductCreateRequest: IValidatableObject
    {
        public decimal Price { set; get; } = 0;
        public decimal OriginalPrice { set; get; } = 0;
        public int Stock { set; get; } = 0;

        [Required(ErrorMessage = "Bạn phải nhập tên sản phẩm")]
        public required string Name { set; get; }

        public string? Description { set; get; } = string.Empty;
        public string? Details { set; get; } = string.Empty;
        public string? SeoDescription { set; get; } = "NA";
        public string? SeoTitle { set; get; } = string.Empty;
        public string? SeoAlias { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngôn ngữ sản phẩm là bắt buộc")]
        public required string LanguageId { set; get; }
        public bool? IsFeatured { get; set; }
        public IFormFile? ThumbnailImage { set; get; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ThumbnailImage != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(ThumbnailImage.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    yield return new ValidationResult(
                        "Chỉ hỗ trợ các định dạng ảnh JPG, JPEG, PNG",
                        new[] { nameof(ThumbnailImage) }
                    );
                }
            }
        }
    }

}
