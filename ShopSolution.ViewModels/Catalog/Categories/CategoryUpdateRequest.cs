using System.ComponentModel.DataAnnotations;

namespace ShopSolution.ViewModels.Catalog.Categories
{
    public class CategoryUpdateRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Bạn phải nhập tên danh mục")]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; }

        [Display(Name = "Danh mục cha")]
        public int? ParentId { get; set; }

        [Display(Name = "Mô tả SEO")]
        public string? SeoDescription { get; set; }

        [Display(Name = "Tiêu đề SEO")]
        public string? SeoTitle { get; set; }

        [Display(Name = "Alias SEO")]
        public string? SeoAlias { get; set; }

        [Display(Name = "Ngôn ngữ")]
        [Required(ErrorMessage = "Ngôn ngữ là bắt buộc")]
        public string LanguageId { get; set; }
    }
}