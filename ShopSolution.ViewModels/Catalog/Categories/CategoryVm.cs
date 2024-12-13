using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.ViewModels.Catalog.Categories
{
    public class CategoryVm
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? SeoDescription { set; get; }

        public string? SeoTitle { set; get; }

        public string? SeoAlias { get; set; }

        public string LanguageId { set; get; }

        public int? ParentId { get; set; }
    }
}