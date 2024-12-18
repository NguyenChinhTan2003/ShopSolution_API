using Microsoft.EntityFrameworkCore;
using ShopSolution.Data.EF;
using ShopSolution.Data.Entities;
using ShopSolution.Utilities.Exceptions;
using ShopSolution.ViewModels.Catalog.Categories;
using ShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.Application.Catalog.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ShopDBContext _context;

        public CategoryService(ShopDBContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryVm>> GetAll(string languageId)
        {
            var query = from c in _context.Categories
                        join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
                        where ct.LanguageId == languageId
                        select new { c, ct };
            return await query.Select(x => new CategoryVm()
            {
                Id = x.c.Id,
                Name = x.ct.Name,
                ParentId = x.c.ParentId
            }).ToListAsync();
        }

        public async Task<CategoryVm> GetById(string languageId, int id)
        {
            var query = from c in _context.Categories
                        join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
                        where ct.LanguageId == languageId && c.Id == id
                        select new { c, ct };
            return await query.Select(x => new CategoryVm()
            {
                Id = x.c.Id,
                Name = x.ct.Name,
                ParentId = x.c.ParentId
            }).FirstOrDefaultAsync();
        }

        public async Task<int> Create(CategoryCreateRequest request)
        {
            var languages = await _context.Languages.ToListAsync();
            var translations = new List<CategoryTranslation>();
            foreach (var language in languages)
            {
                if (language.Id == request.LanguageId)
                {
                    translations.Add(new CategoryTranslation()
                    {
                        Name = request.Name,
                        SeoDescription = request.SeoDescription,
                        SeoAlias = request.SeoAlias,
                        SeoTitle = request.SeoTitle,
                        LanguageId = request.LanguageId
                    });
                }
                else
                {
                    translations.Add(new CategoryTranslation()
                    {
                        Name = request.Name,
                        SeoAlias = request.SeoAlias,
                        SeoDescription = request.SeoDescription,
                        SeoTitle = request.SeoTitle,
                        LanguageId = language.Id
                    });
                }
            }

            var category = new Category()
            {
                IsShowOnHome = true,
                //ParentId = request.ParentId,
                Status = Data.Enums.Status.Active, // Giả sử bạn có enum Status
                SortOrder = 1, // Hoặc giá trị mặc định khác
                CategoryTranslations = translations,
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category.Id;
        }

        public async Task<int> Update(CategoryUpdateRequest request)
        {
            var category = await _context.Categories.FindAsync(request.Id);
            var categoryTranslation = await _context.CategoryTranslations.FirstOrDefaultAsync(x => x.CategoryId == request.Id && x.LanguageId == request.LanguageId);

            if (category == null || categoryTranslation == null)
            {
                throw new ShopException($"Cannot find a category with id: {request.Id}");
            }

            categoryTranslation.Name = request.Name;
            categoryTranslation.SeoAlias = request.SeoAlias;
            categoryTranslation.SeoDescription = request.SeoDescription;
            categoryTranslation.SeoTitle = request.SeoTitle;
            category.ParentId = request.ParentId;
            // Cập nhật các thuộc tính khác của category nếu cần

            return await _context.SaveChangesAsync();
        }

        public async Task<int> Delete(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                throw new ShopException($"Cannot find a category with id: {categoryId}");
            }

            _context.Categories.Remove(category);
            return await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<CategoryVm>> GetAllPaging(string languageId, GetManageCategoryPagingRequest request)
        {
            var query = from c in _context.Categories
                        join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
                        where ct.LanguageId == languageId
                        select new { c, ct };

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.ct.Name.Contains(request.Keyword));
            }

            if (request.ParentId.HasValue && request.ParentId.Value > 0)
            {
                query = query.Where(x => x.c.ParentId == request.ParentId);
            }

            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new CategoryVm()
                {
                    Id = x.c.Id,
                    Name = x.ct.Name,
                    ParentId = x.c.ParentId,
                    SeoAlias = x.ct.SeoAlias,
                    SeoDescription = x.ct.SeoDescription,
                    SeoTitle = x.ct.SeoTitle,
                    LanguageId = x.ct.LanguageId
                }).ToListAsync();

            var pagedResult = new PagedResult<CategoryVm>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };

            return pagedResult;
        }
    }
}