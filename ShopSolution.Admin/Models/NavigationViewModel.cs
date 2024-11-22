using Microsoft.AspNetCore.Mvc.Rendering;
using ShopSolution.ViewModels.System.Languages;
namespace ShopSolution.Admin.Models
{
    public class NavigationViewModel
    {
        public List<SelectListItem>? Languages { get; set; }
        public string? CurrentLanguageId { get; set; }
        public string? ReturnUrl { set; get; }
    }
}
