using System.ComponentModel.DataAnnotations;

namespace ShopSolution.ViewModels.Catalog.Categories
{
    public class CategoryCreateRequest
    {
        [Required(ErrorMessage = "Bạn phải nhập tên danh mục")]
        public string Name { get; set; }

        public string? SeoDescription { get; set; }

        public string? SeoTitle { get; set; }

        public string? SeoAlias { get; set; }

        [Required(ErrorMessage = "Ngôn ngữ là bắt buộc")]
        public string LanguageId { get; set; }

        public int? ParentId { get; set; }
    }
}